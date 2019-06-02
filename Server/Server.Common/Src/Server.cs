using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using LiteNetLib;
using LitJson;
using Lockstep.Game;
using Lockstep.Serialization;
using NetMsg.Server;
using Lockstep.Server.Common;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Server.Common {
    public class MasterMessageHandler { }

    public class Server : BaseServer {
        //Server MS
        protected NetServer<EMsgMS, IServerProxy> _netServerMS;
        protected NetClient<EMsgMS> _netClientMS;
        protected NetClient<EMsgXS> _netClientXS;
        #region ServerMS_ClientMS_ClientXS


        private void InitServerMS(ServerConfigInfo info){
            if (_serverConfig.isMaster) {
                _netServerMS = new NetServer<EMsgMS, IServerProxy>(Define.MSKey, (int) EMsgMS.EnumCount,
                    "S2M", this, (peer) => new ServerProxy(peer));
                _netServerMS.Run(info.masterPort);
            }
        }

        //Client MS

        private void InitClientMS(Msg_RepMasterInfo msg){
            _netClientMS = new NetClient<EMsgMS>((int) EMsgMS.EnumCount, "M2S", this);
            _netClientMS.OnConnected += OnConnectedMaster;
            _netClientMS.Init(msg.ip, msg.port, Define.MSKey);
        }

        void OnConnectedMaster(){
            _netClientMS.Send(EMsgMS.S2M_RegisterServer, new Msg_RegisterServer() {type = (byte) serverType});
        }

        //Client XS

        private void InitClientXS(){
            _netClientXS = new NetClient<EMsgXS>((int) EMsgXS.EnumCount, "X2S", this);
            _netClientXS.OnConnected += OnConnectedDaemon;
            _netClientXS.Init("127.0.0.1", _allConfig.daemonPort, Define.XSKey);
            Debug.Log("InitClientXS " + _allConfig.daemonPort);
        }

        void OnConnectedDaemon(){
            Debug.Log("OnConnectedDaemon " + _allConfig.daemonPort);
            _netClientXS.Send(EMsgXS.S2X_ReqMasterInfo, new Msg_ReqMasterInfo() {
                isMaster = _serverConfig.isMaster,
                masterPort = _serverConfig.masterPort,
                serverType = (byte) serverType
            });
        }

        #endregion


        public override void DoStart(){
            base.DoStart();
            InitServerMS(_serverConfig);
            InitClientXS();
        }

        public override void DoUpdate(int deltaTime){
            base.DoUpdate(deltaTime);
            _netClientXS?.DoUpdate();
            _netClientMS?.DoUpdate();
        }

        public override void PollEvents(){
            _netServerMS?.PollEvents();
        }

        protected void OnMsg_X2S_RepMasterInfo(Deserializer reader){
            if (_netClientMS != null) return;
            var msg = reader.Parse<Msg_RepMasterInfo>();
            if (msg.serverType == (byte) serverType) {
                Debug.Log("OnMsg_X2S_RepMasterInfo " + msg.ToString());
                InitClientMS(msg);
            }
        }

        protected void OnMsg_S2M_RegisterServer(IServerProxy net, Deserializer reader){
            Debug.Log("Add a server " + net.EndPoint.ToString());
            var msg = reader.Parse<Msg_RegisterServer>();
            net.ServerType = (EServerType) msg.type;
        }

        public void SendToCandidate(EMsgMS type, BaseFormater data){
            var writer = new Serializer();
            writer.PutInt16((short) EMsgMS.M2S_MasterToCandidate);
            writer.PutInt16((short) type);
            data.Serialize(writer);
            var bytes = writer.CopyData();
            CandidateMasterServer?.SendMsg(bytes);
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