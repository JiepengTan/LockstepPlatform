using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using LiteNetLib;
using Lockstep.Networking;
using Lockstep.Serialization;
using Lockstep.Server.Common;
using NetMsg.Server;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Server.Daemon {
    public interface IDaemonProxy : IServerProxy {
        DaemonState state { get; set; }
    }

    public class DaemonProxy : ServerProxy, IDaemonProxy {
        public DaemonProxy(IPeer peer) : base(peer){ }
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
            InitNetServer(ref _netServerYX, _serverConfig.masterPort);
        }

        private void InitServerXS(){
            InitNetServer(ref _netServerXS, _serverConfig.serverPort);
        }

        private void InitClientYX(){
            InitNetClient(ref _netClientYX, _serverConfig.masterIp, _serverConfig.masterPort,
                () => {
                    _netClientYX.SendMessage(EMsgYX.X2Y_RegisterDaemon,
                        new Msg_RegisterDaemon() {type = (byte) serverType});
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
            MasterServer?.SendMsg(EMsgYX.X2Y_ReportState, state);
        }

        public void ReportState(){
            _curState.cpu = _cpuCounter.NextValue();
            _curState.memory = _memCounter.NextValue();
            //var servers = _netServerXS.Peers;
            //_curState.localServers = new byte[servers.Count];
            //int i = 0;
            //foreach (var server in servers) {
            //    _curState.localServers[i++] = (byte) server.ServerType;
            //}
//
            //ReportState(_curState);
        }


        public void S2X_ReqMasterInfo(IIncommingMessage reader){
            var msg = reader.Parse<Msg_ReqMasterInfo>();
            var proxy = new ServerProxy(reader.Peer);
            reader.Peer.AddExtension(proxy);
            proxy.ServerType = (EServerType) msg.serverInfo.serverType;
            msg.serverInfo.ip = proxy.EndPoint.Address.ToString();
            _netClientYX.SendMessage(EMsgYX.X2Y_ReqMasterInfo, msg,
                (status, respond) => {
                    var respondMsg = respond.Parse<Msg_RepMasterInfo>();
                    reader.Respond(respondMsg);
                });
        }

        public void Y2X_BorderMasterInfo(IIncommingMessage reader){
            var msg = reader.Parse<Msg_BorderMasterInfo>();
            _netServerXS.BorderMessage(EMsgXS.X2S_BorderMasterInfo, msg);
        }


        public void S2X_StartServer(IIncommingMessage reader){ }
        public void S2X_ShutdownServer(IIncommingMessage reader){ }


        public void X2Y_RegisterDaemon(IIncommingMessage reader){
            var msg = reader.Parse<Msg_RegisterDaemon>();
            //_netServerXS.Border(EMsgXS.X2S_RepMasterInfo, msg);
        }

        public void X2Y_ReqMasterInfo(IIncommingMessage reader){
            var serverInfo = reader.Parse<Msg_ReqMasterInfo>().serverInfo;
            var type = (EServerType) serverInfo.serverType;
            if (serverInfo.isMaster) {
                _type2MasterInfo[type] = serverInfo;
                _netServerYX.BorderMessage(EMsgYX.Y2X_BorderMasterInfo,
                    new Msg_BorderMasterInfo() {serverInfo = serverInfo});
            }

            var infos = _type2MasterInfo.Values.ToArray();
            reader.Respond(EMsgYX.Y2X_RepMasterInfo, new Msg_RepMasterInfo() {serverInfos = infos});
        }


        void LunchProgram(ServerConfigInfo configInfo){
            if (configInfo.type == EServerType.DaemonServer) return;
            if (_allConfig.isDebugMode) {
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