using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using LiteNetLib;
using Lockstep.Game;
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

        private void InitServerYX(){
            if (!_serverConfig.isMaster) return;
            _netServerYX = new NetServer<EMsgYX, IDaemonProxy>(Define.MSKey, (int) EMsgYX.EnumCount, "X2Y", this,
                (peer) => new DaemonProxy(peer));
            _netServerYX.Run(_serverConfig.masterPort);
            Debug.Log("InitServerYX RunServer " + serverType + " port" + _serverConfig.masterPort);
        }

        //Server XS

        private void InitServerXS(){
            _netServerXS = new NetServer<EMsgXS, IServerProxy>(Define.MSKey, (int) EMsgXS.EnumCount, "S2X", this,
                (peer) => new ServerProxy(peer));
            _netServerXS.Run(_serverConfig.servePort);
            Debug.Log("InitServerXS RunServer " + serverType + " port" + _serverConfig.servePort);
        }

        //Client XS

        private void InitClientYX(){
            _netClientYX = new NetClient<EMsgYX>((int) EMsgYX.EnumCount, "Y2X", this);
            _netClientYX.OnConnected += OnConnectedDaemonY;
            _netClientYX.Init(_serverConfig.masterIp, _serverConfig.masterPort, Define.MSKey);
        }

        void OnConnectedDaemonY(){
            _netClientYX.Send(EMsgYX.X2Y_RegisterDaemon, new Msg_RegisterDaemon() {type = (byte) serverType});
        }

        #endregion

        public override void DoStart(){
            _curState = new DaemonState();
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _memCounter = new PerformanceCounter("Memory", "Available MBytes");
            InitServerXS();
            InitServerYX();
            InitClientYX();
            foreach (var serverConfig in _allConfig.servers) {
                if (serverConfig.type == EServerType.DaemonServer) continue;
                if (!serverConfig.isMaster) continue;
                LunchProgram(serverConfig);
            }
        }


        public override void PollEvents(){
            base.PollEvents();
            _netServerYX?.PollEvents();
            _netServerXS?.PollEvents();
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


        public struct IpInfo {
            public string ip;
            public int port;
        }

        public Dictionary<EServerType, IpInfo> type2MasterInfo = new Dictionary<EServerType, IpInfo>();
        public Queue<IServerProxy> pendingReqMasterInfoServers = new Queue<IServerProxy>();

        public void OnMsg_S2X_ReqMasterInfo(IServerProxy net, Deserializer reader){
            var msg = reader.Parse<Msg_ReqMasterInfo>();
            Debug.Log("OnMsg_S2X_ReqMasterInfo " + msg.ToString());
            net.ServerType = (EServerType) msg.serverType;
            msg.ip = net.EndPoint.Address.ToString();
            _netClientYX.Send(EMsgYX.X2Y_ReqMasterInfo, msg);
        }

        
        public void OnMsg_Y2X_RepMasterInfo(Deserializer reader){
            var msg = reader.Parse<Msg_RepMasterInfo>();
            Debug.Log("OnMsg_Y2X_RepMasterInfo " + msg.ToString());
            _netServerXS.Border(EMsgXS.X2S_RepMasterInfo, msg);
        }

        public void OnMsg_S2X_StartServer(IServerProxy net, Deserializer reader){ }
        public void OnMsg_S2X_ShutdownServer(IServerProxy net, Deserializer reader){ }
        
        
        public void OnMsg_X2Y_RegisterDaemon(IDaemonProxy net,Deserializer reader){
            var msg = reader.Parse<Msg_RegisterDaemon>();
            Debug.Log("OnMsg_X2Y_RegisterDaemon " + msg.ToString());
            //_netServerXS.Border(EMsgXS.X2S_RepMasterInfo, msg);
        }
        public void OnMsg_X2Y_ReqMasterInfo(IDaemonProxy net, Deserializer reader){
            var msg = reader.Parse<Msg_ReqMasterInfo>();
            Debug.Log("OnMsg_X2Y_ReqMasterInfo " + msg.ToString());
            var type = (EServerType) msg.serverType;
            if (type2MasterInfo.TryGetValue(type, out var end)) {
                var retMsg = new Msg_RepMasterInfo();
                retMsg.ip = msg.ip;
                retMsg.port = msg.masterPort;
                retMsg.serverType = msg.serverType;
                net.SendMsg(EMsgYX.Y2X_RepMasterInfo, retMsg);
            }
            else {
                if (msg.isMaster) {
                    type2MasterInfo.Add(type, new IpInfo() {ip = msg.ip, port = msg.masterPort});
                    var retMsg = new Msg_RepMasterInfo();
                    retMsg.ip = msg.ip;
                    retMsg.port = msg.masterPort;
                    retMsg.serverType = msg.serverType;
                    _netServerYX.Border(EMsgYX.Y2X_RepMasterInfo, retMsg);
                }
            }
        }

        public override void DoUpdate(int deltaTime){
            base.DoUpdate(deltaTime);
            _netClientYX?.DoUpdate();
            _reportTimer -= deltaTime;
            if (_reportTimer <= 0) {
                _reportTimer = _reportInterval;
                ReportState();
            }
        }

        void LunchProgram(ServerConfigInfo configInfo){
            if (configInfo.type == EServerType.DaemonServer) return;
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configInfo.path);
            Process proc = Process.Start(path);
            if (proc != null) {
                proc.EnableRaisingEvents = true;
                proc.Exited += new EventHandler(OnProcExited);
            }
        }

        void OnProcExited(object sender, EventArgs e){
            var proc = (Process) sender;
            Debug.Log($"type   name {proc.ProcessName} Exit！");
        }
    }
}