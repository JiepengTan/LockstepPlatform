using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using LiteNetLib;
using LitJson;
using Lockstep.Networking;
using Lockstep.Serialization;
using NetMsg.Server;
using Lockstep.Server.Common;
using NetMsg.Common;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Server.Common {
    public class Server : BaseServer {
        //Server MS
        protected NetServer<EMsgMS> _netServerMS;
        protected NetClient<EMsgMS> _netClientMS; //同类型的Master
        protected NetClient<EMsgYM> _netClientYM;
        protected NetClient<EMsgXS> _netClientXS;
        public override void DoStart(){
            base.DoStart();
            InitClientYM();
            InitServerMS(_serverConfig);
            InitClientXS();
        }


        #region ServerMS_ClientMS_ClientXS

        private void InitServerMS(ServerConfigInfo info){
            if (_serverConfig.isMaster) {
                InitNetServer(ref _netServerMS, info.masterPort);
            }
        }

        private void InitClientMS(ServerIpInfo msg){
            InitNetClient(ref _netClientMS, msg.Ip, msg.Port, () => {
                    _netClientMS.SendMessage(EMsgMS.S2M_RegisterServer, new Msg_RegisterServer() {
                        ServerInfo = new ServerIpInfo() {ServerType = (byte) serverType}
                    });
                }
            );
        }

        private void InitClientYM(){
            if (serverType == EServerType.DaemonServer) return;
            InitNetClient(ref _netClientYM, _allConfig.YMIp, _allConfig.YMPort, () => {
                    _netClientYM.SendMessage(EMsgYM.M2Y_RegisterServerInfo, new Msg_RegisterServer() {
                        ServerInfo = new ServerIpInfo() {
                            ServerType = (byte) serverType
                        }
                    });
                }
            );
        }

        private void InitClientXS(){
            InitNetClient(ref _netClientXS, "127.0.0.1", _allConfig.DeamonPort,
                () => {
                    _netClientXS.SendMessage(EMsgXS.S2X_ReqMasterInfo, new Msg_ReqMasterInfo() {
                            ServerInfo = new ServerIpInfo() {
                                IsMaster = _serverConfig.isMaster,
                                Port = _serverConfig.masterPort,
                                ServerType = (byte) serverType
                            }
                        }, (status, respond) => {
                            var msg = respond.Parse<Msg_RepMasterInfo>();
                            if (msg.ServerInfo != null) {
                                if (msg.ServerInfo.ServerType == (byte) serverType) {
                                    InitClientMS(msg.ServerInfo);
                                }

                                OnMasterServerInfo(msg.ServerInfo);
                            }
                        }
                    );
                });
        }

        #endregion

        protected void ReqOtherServerInfo(EServerType type, ResponseCallback callback,
            EServerDetailPortType detailPortType = EServerDetailPortType.ServerPort){
            _netClientXS.SendMessage(EMsgXS.S2X_ReqOtherServerInfo, new Msg_ReqOtherServerInfo() {
                    ServerType = (byte) type,
                    DetailType = (byte) detailPortType
                }, callback
            );
        }

        protected void Y2M_ReqOtherServerInfo(IIncommingMessage reader){
            var msg = reader.Parse<Msg_ReqOtherServerInfo>();
            var type = (EServerType) msg.ServerType;
            if (type == serverType) {
                //TODO 
                var info = GetSlaveServeInfo(msg.DetailType);
                reader.Respond(EMsgMS.M2S_RepOtherServerInfo, new Msg_RepOtherServerInfo() {
                    ServerInfo = info
                });
            }
        }

        protected void S2M_ReqOtherServerInfo(IIncommingMessage reader){
            var msg = reader.Parse<Msg_ReqOtherServerInfo>();
            var type = (EServerType) msg.ServerType;
            if (type == serverType) {
                //TODO 
                var info = GetSlaveServeInfo(msg.DetailType);
                reader.Respond(EMsgMS.M2S_RepOtherServerInfo, new Msg_RepOtherServerInfo() {
                    ServerInfo = info
                });
            }
        }

        /// 更加负载情况 自动分配相遇的服务器
        protected virtual ServerIpInfo GetSlaveServeInfo(byte detailType){
            return new ServerIpInfo() {
                Port = _serverConfig.GetDetailPort(detailType),
                Ip = Ip,
                ServerType = (byte) this.serverType,
            };
        }

        protected void S2M_RegisterServer(IIncommingMessage reader){
            var net = new ServerProxy(reader.Peer);
            reader.Peer.AddExtension(net);
            var msg = reader.Parse<Msg_RegisterServer>();
            net.ServerType = (EServerType) msg.ServerInfo.ServerType;
        }


        public void X2S_BorderMasterInfo(IIncommingMessage reader){
            var msg = reader.Parse<Msg_BorderMasterInfo>();
            if (msg.ServerInfo.ServerType == (byte) serverType) {
                InitClientMS(msg.ServerInfo);
            }

            OnMasterServerInfo(msg.ServerInfo);
        }

        protected virtual void OnMasterServerInfo(ServerIpInfo info){ }

        #region Candidate Master

        /*
         
        public void SendToCandidate(EMsgMS type, BaseFormater data){
            var writer = new Serializer();
            writer.PutInt16((short) EMsgMS.M2S_MasterToCandidate);
            writer.PutInt16((short) type);
            data.Serialize(writer);
            var bytes = writer.CopyData();
            CandidateMasterServer?.SendMsg(bytes);
        }
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