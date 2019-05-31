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

        #region Server XD

        //Server XD
        private List<IDaemonProxy> _slaveXD = new List<IDaemonProxy>();
        private NetServer<EMsgXD, IDaemonProxy> _netServerXD;

        protected virtual IDaemonProxy OnSlaveConnectXD(NetPeer peer){
            var server = new DaemonProxy(peer);
            _slaveXD.Add(server);
            return server;
        }

        protected virtual void OnSlaveDisconnectXD(NetPeer peer, IDaemonProxy param){
            _slaveXD.Remove(param);
        }

        private void InitServerXD(){
            if (!_config.IsMaster()) return;
            _netServerXD = new NetServer<EMsgXD, IDaemonProxy>(Define.MSKey, (int) EMsgXD.EnumCount,
                OnSlaveConnectXD,
                OnSlaveDisconnectXD);
            RegisterMsgHandlerD2X();
        }

        void RegisterMsgHandlerD2X(){
            ServerUtil.RegisterEvent<EMsgMS, NetClientMsgHandler>("OnMsg_M2S", "OnMsg_".Length,
                _netClientMS.RegisterMsgHandler, this);
        }

        #endregion

        #region Server DS

        //Server DS
        private List<IServerProxy> _slaveDS = new List<IServerProxy>();
        private NetServer<EMsgDS, IServerProxy> _netServerDS;

        private void InitServerDS(){
            _netServerDS = new NetServer<EMsgDS, IServerProxy>(Define.MSKey, (int) EMsgDS.EnumCount,
                OnSlaveConnectDS,
                OnSlaveDisconnectDS);
            RegisterMsgHandlerS2D();
        }

        protected virtual IServerProxy OnSlaveConnectDS(NetPeer peer){
            var server = new ServerProxy(peer);
            _slaveDS.Add(server);
            return server;
        }

        protected virtual void OnSlaveDisconnectDS(NetPeer peer, IServerProxy param){
            _slaveDS.Remove(param);
        }

        void RegisterMsgHandlerS2D(){
            ServerUtil.RegisterEvent<EMsgDS, NetServer<EMsgDS, IServerProxy>.MsgHandler>("OnMsg_D2X", "OnMsg_".Length,
                _netServerDS.RegisterMsgHandler, this);
        }

        #endregion

        #region ClientDS

        //Client DS
        protected NetClient<EMsgXD> _netClientXD;


        private void InitClientXD(){
            if (_config.IsMaster()) return;
            _netClientXD = new NetClient<EMsgXD>((int) EMsgXD.EnumCount);
            _netClientXD.OnConnected += OnConnectedDaemonX;
            RegisterMsgHandlerX2D();
            _netClientXD.Init("127.0.0.1", _config.daemonPort, Define.MSKey);
        }

        void RegisterMsgHandlerX2D(){
            ServerUtil.RegisterEvent<EMsgXD, NetServer<EMsgXD, IDaemonProxy>.MsgHandler>("OnMsg_D2X", "OnMsg_".Length,
                _netServerXD.RegisterMsgHandler, this);
        }

        void OnConnectedDaemonX(){
            _netClientXD.Send(EMsgXD.D2X_RegisterDaemon, new Msg_RegisterDaemon() {type = (byte) serverType});
        }

        #endregion

        public override void DoStart(ServerConfigInfo info){
            base.DoStart(info);
            _curState = new DaemonState();
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _memCounter = new PerformanceCounter("Memory", "Available MBytes");
            InitServerDS();
            InitClientXD();
            InitServerXD();
            if (_config.IsMaster()) {
                _netServerXD.Run(info.servePort);
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
            writer.PutByte((byte) EMsgXD.D2X_ReportState);
            state.Serialize(writer);
            var bytes = writer.CopyData();
            masterServer?.SendMsg(bytes);
        }

        public void ReportState(){
            _curState.cpu = _cpuCounter.NextValue();
            _curState.memory = _memCounter.NextValue();
            var serves = _slaveDS;
            _curState.localServers = new byte[serves.Count];
            int i = 0;
            foreach (var server in serves) {
                _curState.localServers[i++] = (byte) server.ServerType;
            }

            ReportState(_curState);
        }


        public void OnMsg_S2D_RegisterServer(IServerProxy server, Deserializer reader){ }
        public void OnMsg_S2D_StartServer(IServerProxy server, Deserializer reader){ }
        public void OnMsg_S2D_ShutdownServer(IServerProxy server, Deserializer reader){ }

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