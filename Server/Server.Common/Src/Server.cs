using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using LiteNetLib;
using LitJson;
using Lockstep.Serialization;
using NetMsg.Server;
using Lockstep.Server.Common;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Server.Common {
    public class MasterMessageHandler { }

    public class Server : BaseServer {
        //Server MS
        protected NetServer<EMsgMS, IServerProxy> _netServerMS;
        protected NetClient<EMsgMS> _netClientMS; //同类型的Master
        protected NetClient<EMsgMS> _netClientOMS; //其他类型的Master 用于提供服务
        protected NetClient<EMsgXS> _netClientXS;
        public List<IPollEvents> _allServerNet = new List<IPollEvents>();
        private IPollEvents[] _cachedAllServerNet;
        public List<IUpdate> _allClientNet = new List<IUpdate>();
        private IUpdate[] _cachedAllClientNet;

        public override void DoStart(){
            base.DoStart();
            InitServerMS(_serverConfig);
            InitClientXS();
        }

        public override void DoUpdate(int deltaTime){
            base.DoUpdate(deltaTime);
            if (_cachedAllClientNet == null) {
                _cachedAllClientNet = _allClientNet.ToArray();
            }

            foreach (var net in _cachedAllClientNet) {
                net.DoUpdate();
            }
        }

        public override void PollEvents(){
            if (_cachedAllServerNet == null) {
                _cachedAllServerNet = _allServerNet.ToArray();
            }

            foreach (var net in _cachedAllServerNet) {
                net.PollEvents();
            }
        }


        #region ServerMS_ClientMS_ClientXS

        //Client MS
        protected void InitNetServer<TMsgType, TParam>(ref NetServer<TMsgType, TParam> refServer, int port,
            NetServer<TMsgType, TParam>.CreateParamFromPeer funcCreateParamFromPeer)
            where TParam : class, INetProxy
            where TMsgType : struct{
            if (refServer != null) return;
            var maxIdx = (short) (object) (TMsgType) Enum.Parse(typeof(TMsgType), "EnumCount");
            var name = typeof(TMsgType).Name;
            var tag = name.Replace("EMsg", "");
            tag = tag.Substring(1, 1) + "2" + tag.Substring(0, 1);
            refServer = new NetServer<TMsgType, TParam>(Define.MSKey, maxIdx, tag, this, funcCreateParamFromPeer);
            refServer.Run(port);
            _allServerNet.Add(refServer);
            _cachedAllServerNet = null;
        }

        protected void InitNetClient<TMsgType>(ref NetClient<TMsgType> refClient, string ip, int port,
            Action onConnCallback = null) where TMsgType : struct{
            if (refClient != null) return;
            var maxIdx = (short) (object) (TMsgType) Enum.Parse(typeof(TMsgType), "EnumCount");
            var name = typeof(TMsgType).Name;
            var tag = name.Replace("EMsg", "");
            tag = tag.Substring(0, 1) + "2" + tag.Substring(1, 1);
            refClient = new NetClient<TMsgType>(maxIdx, tag, this);
            if (onConnCallback != null) {
                refClient.OnConnected += onConnCallback;
            }

            refClient.Init(ip, port, Define.MSKey);
            _allClientNet.Add(refClient);
            _cachedAllClientNet = null;
        }

        private void InitServerMS(ServerConfigInfo info){
            if (_serverConfig.isMaster) {
                InitNetServer(ref _netServerMS, info.masterPort, (peer) => new ServerProxy(peer));
            }
        }

        private void InitClientMS(ServerIpInfo msg){
            InitNetClient(ref _netClientMS, msg.ip, msg.port, () => {
                    Debug.Log("Connect Master " + serverType);
                    _netClientMS.Send(EMsgMS.S2M_RegisterServer, new Msg_RegisterServer() {
                        serverInfo = new ServerIpInfo() {serverType = (byte) serverType}
                    });
                }
            );
        }

        private void InitClientXS(){
            Debug.Log("InitClientXS " + _allConfig.daemonPort);
            InitNetClient(ref _netClientXS, "127.0.0.1", _allConfig.daemonPort,
                () => {
                    Debug.Log("OnConnectedDaemon " + _allConfig.daemonPort);
                    _netClientXS.Send(EMsgXS.S2X_ReqMasterInfo, new Msg_ReqMasterInfo() {
                        serverInfo = new ServerIpInfo() {
                            isMaster = _serverConfig.isMaster,
                            port = _serverConfig.masterPort,
                            serverType = (byte) serverType
                        }
                    });
                });
        }

        #endregion

        protected void OnMsg_S2M_ReqOtherServerInfo(IServerProxy net, Deserializer reader){
            var msg = reader.Parse<Msg_ReqOtherServerInfo>();
            var type = (EServerType) msg.serverType;
            Debug.Log("OnMsg_S2M_ReqOtherServerInfo " + type);
            if (type == serverType) {
                //TODO 
                var info = GetSlaveServeInfo();
                Debug.Log("GetSlaveServeInfo " + info);
                net.SendMsg(EMsgMS.M2S_RepOtherServerInfo,new Msg_RepOtherServerInfo() {
                    serverInfo = info
                });
            }
        }

        /// 更加负载情况 自动分配相遇的服务器
        protected virtual ServerIpInfo GetSlaveServeInfo(){
            return null;
        }

        protected void OnMsg_S2M_RegisterServer(IServerProxy net, Deserializer reader){
            Debug.Log("Add a server " + net.EndPoint.ToString());
            var msg = reader.Parse<Msg_RegisterServer>();
            net.ServerType = (EServerType) msg.serverInfo.serverType;
        }

        protected void OnMsg_X2S_RepMasterInfo(Deserializer reader){
            if (_netClientMS != null) return;
            var msg = reader.Parse<Msg_RepMasterInfo>();
            foreach (var serverInfo in msg.serverInfos) {
                if (serverInfo.serverType == (byte) serverType) {
                    Debug.Log("OnMsg_X2S_RepMasterInfo " + msg.ToString());
                    InitClientMS(serverInfo);
                }

                OnMasterServerInfo(serverInfo);
            }
        }

        public void OnMsg_X2S_BorderMasterInfo(Deserializer reader){
            var msg = reader.Parse<Msg_BorderMasterInfo>();
            Debug.Log("OnMsg_X2S_BorderMasterInfo " + msg.ToString());
            if (msg.serverInfo.serverType == (byte) serverType) {
                Debug.Log("OnMsg_X2S_RepMasterInfo " + msg.ToString());
                InitClientMS(msg.serverInfo);
            }

            OnMasterServerInfo(msg.serverInfo);
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