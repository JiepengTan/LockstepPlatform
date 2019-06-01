using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LiteNetLib;
using Lockstep.Game;
using Lockstep.Serialization;
using Lockstep.Server.Common;
using NetMsg.Server;
using Server.Common;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Server.Daemon {
    public interface IDaemonProxy {
        DaemonState state { get; set; }
        void SendMsg(byte[] data);
    }

    public class DaemonProxy : IDaemonProxy {
        private NetPeer _peer;

        public DaemonProxy(NetPeer peer){
            _peer = peer;
        }

        public DaemonState state { get; set; }

        public void SendMsg(byte[] data){
            _peer?.Send(data, DeliveryMethod.ReliableSequenced);
        }
    }


    public class DaemonServer : Common.Server, IDaemonServer {
        private int _reportInterval = 1000;
        private int _reportTimer = 0;
        private DaemonState _curState;
        private PerformanceCounter _cpuCounter;
        private PerformanceCounter _memCounter;

        #region Server YX

        //Server YX
        private List<IDaemonProxy> _slaveYX = new List<IDaemonProxy>();
        private NetServer<EMsgYX, IDaemonProxy> _netServerYX;

        protected virtual IDaemonProxy OnSlaveConnectYX(NetPeer peer){
            var server = new DaemonProxy(peer);
            _slaveYX.Add(server);
            return server;
        }

        protected virtual void OnSlaveDisconnectYX(NetPeer peer, IDaemonProxy param){
            _slaveYX.Remove(param);
        }

        private void InitServerYX(){
            if (!_config.IsMaster()) return;
            _netServerYX = new NetServer<EMsgYX, IDaemonProxy>(Define.MSKey, (int) EMsgYX.EnumCount,
                OnSlaveConnectYX,
                OnSlaveDisconnectYX);
            RegisterMsgHandlerX2Y();
        }

        void RegisterMsgHandlerX2Y(){
            ServerUtil.RegisterEvent<EMsgMS, NetClientMsgHandler>("OnMsg_M2S", "OnMsg_".Length,
                _netClientMS.RegisterMsgHandler, this);
        }

        #endregion

        #region Server XS

        //Server XS
        private List<IServerProxy> _slaveXS = new List<IServerProxy>();
        private NetServer<EMsgXS, IServerProxy> _netServerXS;

        private void InitServerXS(){
            _netServerXS = new NetServer<EMsgXS, IServerProxy>(Define.MSKey, (int) EMsgXS.EnumCount,
                OnSlaveConnectXS,
                OnSlaveDisconnectXS);
            RegisterMsgHandlerS2X();
        }

        protected virtual IServerProxy OnSlaveConnectXS(NetPeer peer){
            var server = new ServerProxy(peer);
            _slaveXS.Add(server);
            return server;
        }

        protected virtual void OnSlaveDisconnectXS(NetPeer peer, IServerProxy param){
            _slaveXS.Remove(param);
        }

        void RegisterMsgHandlerS2X(){
            ServerUtil.RegisterEvent<EMsgXS, NetServer<EMsgXS, IServerProxy>.MsgHandler>("OnMsg_X2Y", "OnMsg_".Length,
                _netServerXS.RegisterMsgHandler, this);
        }

        #endregion

        #region ClientXS

        //Client XS
        protected NetClient<EMsgYX> _netClientYX;


        private void InitClientYX(){
            if (_config.IsMaster()) return;
            _netClientYX = new NetClient<EMsgYX>((int) EMsgYX.EnumCount);
            _netClientYX.OnConnected += OnConnectedDaemonX;
            RegisterMsgHandlerY2X();
            _netClientYX.Init("127.0.0.1", _config.daemonPort, Define.MSKey);
        }

        void RegisterMsgHandlerY2X(){
            ServerUtil.RegisterEvent<EMsgYX, NetServer<EMsgYX, IDaemonProxy>.MsgHandler>("OnMsg_X2Y", "OnMsg_".Length,
                _netServerYX.RegisterMsgHandler, this);
        }

        void OnConnectedDaemonX(){
            _netClientYX.Send(EMsgYX.X2Y_RegisterDaemon, new Msg_RegisterDaemon() {type = (byte) serverType});
        }

        #endregion

        public override void DoStart(ServerConfigInfo info){
            base.DoStart(info);
            _curState = new DaemonState();
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _memCounter = new PerformanceCounter("Memory", "Available MBytes");
            InitServerXS();
            InitClientYX();
            InitServerYX();
            if (_config.IsMaster()) {
                _netServerYX.Run(info.servePort);
                foreach (var serverConfig in _config.servers) {
                    LunchProgram(serverConfig);
                }
            }
        }


        public void ReqStartServer(EServerType type){
            //TODO 根据个Daemon 的cpu 占用决定究竟是哪一个改启动新的服务
            StartServer(type);
        }

        public void StartServer(EServerType type){
            var serverConfig = _config.GetServerConfig(type);
            if (serverConfig != null) {
                LunchProgram(serverConfig);
            }
        }

        public void ReportState(DaemonState state){
            var writer = new Serializer();
            writer.PutByte((byte) EMsgYX.X2Y_ReportState);
            state.Serialize(writer);
            var bytes = writer.CopyData();
            masterServer?.SendMsg(bytes);
        }

        public void ReportState(){
            _curState.cpu = _cpuCounter.NextValue();
            _curState.memory = _memCounter.NextValue();
            var serves = _slaveXS;
            _curState.localServers = new byte[serves.Count];
            int i = 0;
            foreach (var server in serves) {
                _curState.localServers[i++] = (byte) server.ServerType;
            }

            ReportState(_curState);
        }


        public void OnMsg_S2X_RegisterServer(IServerProxy server, Deserializer reader){ }
        public void OnMsg_S2X_StartServer(IServerProxy server, Deserializer reader){ }
        public void OnMsg_S2X_ShutdownServer(IServerProxy server, Deserializer reader){ }

        public override void DoUpdate(int deltaTime){
            base.DoUpdate(deltaTime);
            _reportTimer -= deltaTime;
            if (_reportTimer <= 0) {
                _reportTimer = _reportInterval;
                ReportState();
            }
        }

        void LunchProgram(ServerConfigInfo configInfo){
            Process proc = Process.Start(configInfo.path);
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