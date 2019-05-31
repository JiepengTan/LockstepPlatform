#define DEBUG_FRAME_DELAY
using System;
using System.Collections;
using LiteNetLib;
using Lockstep.Serialization;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Game {
    public delegate void NetClientMsgHandler(Deserializer reader);

    public class NetClient<TMsgType> {
        protected string _ip;
        protected int _port;
        protected string _key;
        protected EventBasedNetListener _listener; 
        protected NetManager _client;
        protected NetPeer _peer;
        protected float _autoConnTimer;
        public bool Connected => _client?.FirstPeer?.ConnectionState == ConnectionState.Connected;

        public float AutoConnInterval = 1;

        //所有的消息处理函数
        protected NetClientMsgHandler[] AllClientMsgDealFuncs;

        private bool _isInit = false;
        public Action OnConnected;

        public void RegisterMsgHandler(TMsgType msgType, NetClientMsgHandler handler){
            AllClientMsgDealFuncs[(int) (object) msgType] = handler;
        }

        public NetClient(int maxMsgHandlerIdx){
            AllClientMsgDealFuncs = new NetClientMsgHandler[maxMsgHandlerIdx];
        }

        public void Init(string ip, int port, string key){
            _key = key;
            this._ip = ip;
            this._port = port;
            _listener = new EventBasedNetListener();
            _listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) => {
                OnNetMsg(dataReader.GetRemainingBytes());
                dataReader.Recycle();
            };
            _listener.PeerConnectedEvent += (peer) => {
                _peer = peer;
                OnConnected?.Invoke();
            };
            _listener.PeerDisconnectedEvent += (peer, disconnectInfo) => { };
            _client = new NetManager(_listener) {
                DisconnectTimeout = 300000
            };
            _isInit = true;
        }

        public void DoStart(){
            if (!_isInit) return;
            _client.Start();
            _client.Connect(_ip, _port, _key);
        }

        public void DoDestroy(){
            _isInit = false;
            _client?.Stop();
        }

        public void DoUpdate(){
            if (!_isInit) return;
            AutoConnect();
            _client.PollEvents();
        }

        private void AutoConnect(){
            _autoConnTimer += 0.016f;
            if (_autoConnTimer > AutoConnInterval && !Connected) {
                _autoConnTimer = 0;
                _client.Connect(_ip, _port, _key);
            }
        }

        private void OnNetMsg(byte[] rawData){
            var reader = new Deserializer(Compressor.Decompress(rawData));
            var msgTypeId = reader.GetByte();
            if (msgTypeId >= AllClientMsgDealFuncs.Length) {
                Debug.LogError("Recv error msg type" + msgTypeId);
                return;
            }

            var func = AllClientMsgDealFuncs[msgTypeId];
            if (func != null) {
                func(reader);
            }
            else {
                Debug.LogError("ErrorMsg type :no msgHandler" + msgTypeId);
            }
        }

        public void Send(TMsgType type, BaseFormater data){
            var writer = new Serializer();
            writer.PutByte((byte)(object)type);
            data.Serialize(writer);
            var bytes = Compressor.Compress(writer.Data);
            Send(bytes);
        }

        public void Send(byte[] data){
            _peer?.Send(data, DeliveryMethod.ReliableOrdered);
        }
    }
}