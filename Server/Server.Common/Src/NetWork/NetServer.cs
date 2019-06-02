using System;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib;
using Lockstep.Logging;
using Lockstep.Serialization;
using NetMsg.Server;

namespace Lockstep.Server.Common {
    public class NetServer<TMsgType, TParam> : IServer
        where TParam : class, INetProxy
        where TMsgType : struct {
        public event Action<object> ClientConnected;
        public event Action<object> ClientDisconnected;
        public event Action<NetPeer, byte[]> DataReceived;

        private readonly NetManager _server;
        private readonly EventBasedNetListener _listener;

        private string _clientKey;

        public delegate void MsgHandler(TParam param, Deserializer reader);

        public delegate TParam CreateParamFromPeer(NetPeer peer);

        public delegate void RemoveParamFromPeer(NetPeer peer, TParam param);

        //所有的消息处理函数
        protected MsgHandler[] _allMsgDealFuncs;
        private CreateParamFromPeer FuncCreateParamOnConnect;
        private RemoveParamFromPeer FuncRemoveParamOnDisconnect;
        private Dictionary<int, TParam> netId2Peer = new Dictionary<int, TParam>();
        private int maxMsgIdx;
        private List<TParam> _peers = new List<TParam>();
        public List<TParam> Peers => _peers;

        public TParam GetClientFromNetId(int netId){
            if (netId2Peer.TryGetValue(netId, out var par)) {
                return par;
            }

            return null;
        }

        public NetServer(string clientKey, int maxMsgIdx,
            string msgFlag, object msgHandlerObj,
            CreateParamFromPeer funcCreateParamOnConnectOnConnect
            , RemoveParamFromPeer funcRemoveParamOnDisconnect = null){
            _clientKey = clientKey;
            _listener = new EventBasedNetListener();
            _server = new NetManager(_listener) {
                DisconnectTimeout = 300000,
            };
            this.maxMsgIdx = maxMsgIdx;
            _allMsgDealFuncs = new MsgHandler[maxMsgIdx];
            FuncCreateParamOnConnect = funcCreateParamOnConnectOnConnect;
            FuncRemoveParamOnDisconnect = funcRemoveParamOnDisconnect;
            ServerUtil.RegisterEvent<TMsgType, MsgHandler>("OnMsg_" + msgFlag, "OnMsg_".Length, RegisterMsgHandler,
                msgHandlerObj);
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

        public void Border(TMsgType type, BaseFormater msg){
            var writer = new Serializer();
            writer.PutInt16((short) (object) type);
            msg.Serialize(writer);
            var bytes = Compressor.Compress(writer.CopyData());
            var peers = _peers;
            foreach (var peer in peers) {
                peer.SendMsg(bytes);
            }
        }

        public void Run(int port){
            _listener.ConnectionRequestEvent += request => { request.AcceptIfKey(_clientKey); };

            _listener.PeerConnectedEvent += peer => {
                var param = FuncCreateParamOnConnect(peer);
                _peers.Add(param);
                netId2Peer.Add(peer.Id, param);
                ClientConnected?.Invoke(peer);
            };

            _listener.NetworkReceiveEvent += (peer, reader, method) => {
                OnDataReceived(peer, reader.GetRemainingBytes());
            };

            _listener.PeerDisconnectedEvent += (peer, info) => {
                var param = netId2Peer[peer.Id];
                FuncRemoveParamOnDisconnect?.Invoke(peer, param);
                _peers.Remove(param);
                netId2Peer.Remove(peer.Id);
                ClientDisconnected?.Invoke(peer);
            };

            _server.Start(port);
        }

        public void PollEvents(){
            _server.PollEvents();
        }


        public void RegisterMsgHandler(TMsgType msgType, MsgHandler handler){
            var idx = (short) (object) msgType;
            _allMsgDealFuncs[idx] = handler;
        }

        private void OnDataReceived(NetPeer peer, byte[] data){
            int netID = peer.Id;
            try {
                var reader = new Deserializer(Compressor.Decompress(data));
                var msgType = reader.GetInt16();
                if (msgType >= maxMsgIdx) {
                    Debug.LogError("msgType out of range " + msgType);
                    return;
                }

                {
                    TParam param = null;
                    if (netId2Peer.TryGetValue(netID, out TParam _server)) {
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
                        Debug.LogError("ErrorMsg type :no msg handler " + (TMsgType) (object) msgType);
                    }
                }
            }
            catch (Exception e) {
                Debug.Log($"netID{netID} parse msg Error:{e.ToString()}");
            }
        }
    }
}