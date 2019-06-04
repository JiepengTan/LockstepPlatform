using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using LiteNetLib;
using Lockstep.Serialization;
using Lockstep.Server.Common;
using NetMsg.Server;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Server.Daemon {
    public interface IDaemonProxy : IServerProxy {
        DaemonState state { get; set; }
    }

    public class DaemonProxy : ServerProxy, IDaemonProxy {
        public DaemonProxy(NetPeer peer) : base(peer){ }
        public DaemonState state { get; set; }
    }


    public class DaemonServer : Common.Server, IDaemonServer {
        private int _reportInterval = 1000;
        private int _reportTimer = 0;
        private DaemonState _curState;
        private PerformanceCounter _cpuCounter;
        private PerformanceCounter _memCounter;

        #region Server YX

        //Server YX
        private NetServer<EMsgYX, IDaemonProxy> _netServerYX;
        private NetServer<EMsgXS, IServerProxy> _netServerXS;
        protected NetClient<EMsgYX> _netClientYX;
        public Dictionary<EServerType, ServerIpInfo> _type2MasterInfo = new Dictionary<EServerType, ServerIpInfo>();

        private void InitServerYX(){
            if (!_serverConfig.isMaster) return;
            InitNetServer(ref _netServerYX, _serverConfig.masterPort, (peer) => new DaemonProxy(peer));
        }

        private void InitServerXS(){
            InitNetServer(ref _netServerXS, _serverConfig.serverPort, (peer) => new ServerProxy(peer));
        }

        private void InitClientYX(){
            InitNetClient(ref _netClientYX, _serverConfig.masterIp, _serverConfig.masterPort,
                () => {
                    _netClientYX.Send(EMsgYX.X2Y_RegisterDaemon, new Msg_RegisterDaemon() {type = (byte) serverType});
                }
            );
        }

        void OnConnectedDaemonY(){ }

        #endregion

        public override void DoStart(){
            _curState = new DaemonState();
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _memCounter = new PerformanceCounter("Memory", "Available MBytes");
            InitServerXS();
            InitServerYX();
            InitClientYX();
            foreach (var serverConfig in _allConfig.servers.ToArray()) {
                Debug.Log("LunchProgram " + serverConfig.type);
                if (serverConfig.type == EServerType.DaemonServer) continue;
                if (!serverConfig.isMaster) continue;
                LunchProgram(serverConfig);
            }
        }

        public override void DoUpdate(int deltaTime){
            base.DoUpdate(deltaTime);
            _reportTimer -= deltaTime;
            if (_reportTimer <= 0) {
                _reportTimer = _reportInterval;
                ReportState();
            }
        }

        public void ReqStartServer(EServerType type){
            //TODO 根据个Daemon 的cpu 占用决定究竟是哪一个改启动新的服务
            StartServer(type);
        }

        public void StartServer(EServerType type){
            var serverConfig = _allConfig.GetServerConfig(type);
            if (serverConfig != null) {
                LunchProgram(serverConfig);
            }
        }

        public void ReportState(DaemonState state){
            var writer = new Serializer();
            writer.PutByte((byte) EMsgYX.X2Y_ReportState);
            state.Serialize(writer);
            var bytes = writer.CopyData();
            MasterServer?.SendMsg(bytes);
        }

        public void ReportState(){
            _curState.cpu = _cpuCounter.NextValue();
            _curState.memory = _memCounter.NextValue();
            var servers = _netServerXS.Peers;
            _curState.localServers = new byte[servers.Count];
            int i = 0;
            foreach (var server in servers) {
                _curState.localServers[i++] = (byte) server.ServerType;
            }

            ReportState(_curState);
        }


        public void OnMsg_S2X_ReqMasterInfo(IServerProxy net, Deserializer reader){
            var msg = reader.Parse<Msg_ReqMasterInfo>();
            Debug.Log("OnMsg_S2X_ReqMasterInfo " + msg.ToString());
            net.ServerType = (EServerType) msg.serverInfo.serverType;
            msg.serverInfo.ip = net.EndPoint.Address.ToString();
            _netClientYX.Send(EMsgYX.X2Y_ReqMasterInfo, msg);
        }


        public void OnMsg_Y2X_RepMasterInfo(Deserializer reader){
            var msg = reader.Parse<Msg_RepMasterInfo>();
            Debug.Log("OnMsg_Y2X_RepMasterInfo " + msg.ToString());
            _netServerXS.Border(EMsgXS.X2S_RepMasterInfo, msg);
        }

        public void OnMsg_Y2X_BorderMasterInfo(Deserializer reader){
            var msg = reader.Parse<Msg_BorderMasterInfo>();
            Debug.Log("OnMsg_Y2X_RepMasterInfo " + msg.ToString());
            _netServerXS.Border(EMsgXS.X2S_BorderMasterInfo, msg);
        }


        public void OnMsg_S2X_StartServer(IServerProxy net, Deserializer reader){ }
        public void OnMsg_S2X_ShutdownServer(IServerProxy net, Deserializer reader){ }


        public void OnMsg_X2Y_RegisterDaemon(IDaemonProxy net, Deserializer reader){
            var msg = reader.Parse<Msg_RegisterDaemon>();
            Debug.Log("OnMsg_X2Y_RegisterDaemon " + msg.ToString());
            //_netServerXS.Border(EMsgXS.X2S_RepMasterInfo, msg);
        }

        public void OnMsg_X2Y_ReqMasterInfo(IDaemonProxy net, Deserializer reader){
            var serverInfo = reader.Parse<Msg_ReqMasterInfo>().serverInfo;
            Debug.Log("OnMsg_X2Y_ReqMasterInfo " + serverInfo.ToString());
            var type = (EServerType) serverInfo.serverType;
            if (serverInfo.isMaster) {
                _type2MasterInfo[type] = serverInfo;
                _netServerYX.Border(EMsgYX.Y2X_BorderMasterInfo, new Msg_BorderMasterInfo() {serverInfo = serverInfo});
            }

            if (_type2MasterInfo.Count > 0) {
                var infos = _type2MasterInfo.Values.ToArray();
                net.SendMsg(EMsgYX.Y2X_RepMasterInfo, new Msg_RepMasterInfo() {serverInfos = infos});
            }
        }


        void LunchProgram(ServerConfigInfo configInfo){
            if (configInfo.type == EServerType.DaemonServer) return;
            if (_allConfig.isAllInOne) {
                ServerUtil.RunServer(GetType().Assembly, configInfo.type, _allConfig);
            }
            else {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Server.Servers.exe");
                Debug.Log("Start Program " + configInfo.type.ToString());
                Process proc = Process.Start(path, configInfo.type.ToString());
                if (proc != null) {
                    proc.EnableRaisingEvents = true;
                    proc.Exited += new EventHandler(OnProcExited);
                }
            }
        }

        void OnProcExited(object sender, EventArgs e){
            var proc = (Process) sender;
            Debug.Log($"type   name  Exit！");
        }
    }
}