using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using LiteNetLib;
using Lockstep.Game;
using Lockstep.Serialization;
using NetMsg.Server;
using Server.Common;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Server.Common {
    public class MasterMessageHandler { }

    public class Server : BaseServer {
        protected ConfigInfo _config;

        #region Server MS

        //Server MS
        protected NetServer<EMsgMS, IServerProxy> _netServerMS;
        protected List<IServerProxy> _slaveMS = new List<IServerProxy>();

        protected virtual IServerProxy OnSlaveConnectMS(NetPeer peer){
            var server = new ServerProxy(peer);
            _slaveMS.Add(server);
            return server;
        }

        protected virtual void OnSlaveDisconnectMS(NetPeer peer, IServerProxy param){
            _slaveMS.Remove(param);
        }

        void RegisterMsgHandlerS2M(){
            ServerUtil.RegisterEvent<EMsgMS, NetServer<EMsgMS, IServerProxy>.MsgHandler>("OnMsg_S2M", "OnMsg_".Length,
                _netServerMS.RegisterMsgHandler, this);
        }

        private void InitServerMS(ServerConfigInfo info){
            if (_config.IsMaster()) {
                _netServerMS = new NetServer<EMsgMS, IServerProxy>(Define.MSKey, (int) EMsgMS.EnumCount,
                    OnSlaveConnectMS,
                    OnSlaveDisconnectMS);
                RegisterMsgHandlerS2M();
                _netServerMS.Run(info.servePort);
            }
        }

        #endregion

        #region Client MS

        //Client MS
        protected NetClient<EMsgMS> _netClientMS;

        private void InitClientMS(Msg_RepMasterInfo msg){
            _netClientMS = new NetClient<EMsgMS>((int) EMsgMS.EnumCount);
            _netClientMS.OnConnected += OnConnectedMaster;
            RegisterMsgHandlerM2S();
            _netClientMS.Init(msg.ip, msg.port, Define.MSKey);
        }

        void RegisterMsgHandlerM2S(){
            ServerUtil.RegisterEvent<EMsgMS, NetClientMsgHandler>("OnMsg_M2S", "OnMsg_".Length,
                _netClientMS.RegisterMsgHandler, this);
        }

        void OnConnectedMaster(){
            _netClientMS.Send(EMsgMS.RegisterServer, new Msg_RegisterServer() {type = (byte) serverType});
        }

        #endregion

        #region Client DS

        //Client DS
        protected NetClient<EMsgDS> _netClientDS;

        private void InitClientDS(){
            _netClientDS = new NetClient<EMsgDS>((int) EMsgDS.EnumCount);
            _netClientDS.OnConnected += OnConnectedDaemon;
            RegisterMsgHandlerD2S();
            _netClientDS.Init("127.0.0.1", _config.daemonPort, Define.DSKey);
        }

        void RegisterMsgHandlerD2S(){
            ServerUtil.RegisterEvent<EMsgDS, NetClientMsgHandler>("OnMsg_D2S", "OnMsg_".Length,
                _netClientDS.RegisterMsgHandler, this);
        }

        void OnConnectedDaemon(){
            _netClientDS.Send(EMsgDS.S2D_ReqMasterInfo, new Msg_ReqMasterInfo() {type = (byte) serverType});
        }

        #endregion

        public override void DoStart(ServerConfigInfo info){
            base.DoStart(info);
            serverType = info.type;
            _config = ServerUtil.LoadConfig();
            InitClientDS();
            InitServerMS(info);
        }


        void OnMsg_D2S_RepMasterInfo(Deserializer reader){
            if (_netClientMS != null) return;
            var msg = reader.Parse<Msg_RepMasterInfo>();
            InitClientMS(msg);
        }


        void OnMsg_S2M_RegisterServer(IServerProxy server, Deserializer reader){
            Debug.Log("Add a server " + server.EndPoint.ToString());
            var msg = reader.Parse<Msg_RegisterServer>();
            server.ServerType = (EServerType)msg.type;
        }


        public void BorderToSlaves(EMsgMS type, BaseFormater data){
            var writer = new Serializer();
            writer.PutByte((byte) type);
            data.Serialize(writer);
            var bytes = writer.CopyData();
            foreach (var server in _slaveMS) {
                server.SendMsg(bytes);
            }
        }

        public void SendToCandidate(EMsgMS type, BaseFormater data){
            var writer = new Serializer();
            writer.PutByte((byte) EMsgMS.MasterToCandidate);
            writer.PutByte((byte) type);
            data.Serialize(writer);
            var bytes = writer.CopyData();
            candidateMasterServer?.SendMsg(bytes);
        }

        #region Candidate Master

        /*
        private Queue<byte[]> pendingSyncMsgs = new Queue<byte[]>();
        private IServerProxy candidateMasterServer;
        public void OnRecvServerMsg(Deserializer reader){
            
            var masterType = (EMsgMS) reader.GetByte();
            var type = (EMsgMS) reader.GetByte();
            if (masterType == EMsgMS.SlaveToMaster) {
                if (IsMaster) {
                    
                    if (candidateMasterServer != null) {
                        while (pendingSyncMsgs.Count > 0) {
                            var msg = pendingSyncMsgs.Dequeue();
                            candidateMasterServer.SendMsg(msg);
                        }

                        candidateMasterServer.SendMsg(reader.RawData); //同步给备份服务器 
                    }
                    else {
                        pendingSyncMsgs.Enqueue(reader.RawData);
                    }
                }
            }

            DispatcherMsg(type, reader);
        }
        //public virtual void DispatcherMsg(EMsgMS type, Deserializer reader){ }
            */

        #endregion
    }
}