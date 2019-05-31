using System;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib;
using Lockstep.Logging;
using Lockstep.Serialization;
using NetMsg.Server;

namespace Lockstep.Server.Common {
    public class NetServer<TMsgType, TParam> : IServer where TParam : class {
        public event Action<object> ClientConnected;
        public event Action<object> ClientDisconnected;
        public event Action<NetPeer, byte[]> DataReceived;

        private readonly NetManager _server;
        private readonly EventBasedNetListener _listener;

        private string _clientKey;

        //所有的消息处理函数
        protected MsgHandler[] _allMsgDealFuncs;
        private CreateParamFromPeer FuncCreateParamOnConnect;
        private RemoveParamFromPeer FuncRemoveParamOnDisconnect;
        public Dictionary<int, TParam> netId2Param = new Dictionary<int, TParam>();
        private int maxMsgIdx;
        public NetServer(string clientKey, int maxMsgIdx,
            CreateParamFromPeer funcCreateParamOnConnectOnConnect
            , RemoveParamFromPeer funcRemoveParamOnDisconnect){
            _clientKey = clientKey;
            _listener = new EventBasedNetListener();
            _server = new NetManager(_listener) {
                DisconnectTimeout = 300000,
            };
            this.maxMsgIdx = maxMsgIdx;
            _allMsgDealFuncs = new MsgHandler[maxMsgIdx];
            FuncCreateParamOnConnect = funcCreateParamOnConnectOnConnect;
            FuncRemoveParamOnDisconnect = funcRemoveParamOnDisconnect;
        }

        public void Distribute(byte[] data){
            _server.SendToAll(data, DeliveryMethod.ReliableOrdered);
        }

        public void Distribute(int clientId, byte[] data){
            _server.SendToAll(data, DeliveryMethod.ReliableOrdered,
                _server.ConnectedPeerList.First(peer => peer.Id == clientId));
        }

        public void Send(int clientId, byte[] data){
            _server.ConnectedPeerList.First(peer => peer.Id == clientId).Send(data, DeliveryMethod.ReliableOrdered);
        }

        public void Run(int port){
            _listener.ConnectionRequestEvent += request => { request.AcceptIfKey(_clientKey); };

            _listener.PeerConnectedEvent += peer => {
                var param = FuncCreateParamOnConnect(peer);
                netId2Param.Add(peer.Id, param);
                ClientConnected?.Invoke(peer);
            };

            _listener.NetworkReceiveEvent += (peer, reader, method) => {
                OnDataReceived(peer, reader.GetRemainingBytes());
            };

            _listener.PeerDisconnectedEvent += (peer, info) => {
                var param = netId2Param[peer.Id];
                FuncRemoveParamOnDisconnect?.Invoke(peer,param);
                netId2Param.Remove(peer.Id);
                ClientDisconnected?.Invoke(peer);
            };

            _server.Start(port);
        }

        public void PollEvents(){
            _server.PollEvents();
        }

        public delegate void MsgHandler(TParam param, Deserializer reader);

        public delegate TParam CreateParamFromPeer(NetPeer peer);

        public delegate void RemoveParamFromPeer(NetPeer peer,TParam param);

        public void RegisterMsgHandler(TMsgType msgType, MsgHandler handler){
            _allMsgDealFuncs[(int) (object) msgType] = handler;
        }

        private void OnDataReceived(NetPeer peer, byte[] data){
            int netID = peer.Id;
            try {
                var reader = new Deserializer(Compressor.Decompress(data));
                var msgType = reader.GetByte();
                if (msgType >= maxMsgIdx) {
                    Debug.LogError("msgType out of range " + msgType);
                    return;
                }

                {
                    TParam param = null;
                    if (netId2Param.TryGetValue(netID, out TParam _server)) {
                        param = _server;
                    }
                    else {
                        return;
                    }

                    var _func = _allMsgDealFuncs[msgType];
                    if (_func != null) {
                        _func(param, reader);
                    }
                    else {
                        Debug.LogError("ErrorMsg type :no msgHnadler" + msgType);
                    }
                }
            }
            catch (Exception e) {
                Debug.LogError($"netID{netID} parse msg Error:{e.ToString()}");
            }
        }
    }
}