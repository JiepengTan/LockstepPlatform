#define DEBUG_FRAME_DELAY
using System;
using System.Collections;
using LiteNetLib;
using Lockstep.Serialization;
using NetMsg.Game;
using NetMsg.Lobby;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Game {
    public class BaseNetProxy {
        protected string _ip;
        protected int _port;
        protected string _key;
        protected EventBasedNetListener _listener;
        protected NetManager _client;
        protected NetPeer _peer;
        protected float _autoConnTimer;

        public bool Connected => _client.FirstPeer?.ConnectionState == ConnectionState.Connected;
        public float AutoConnInterval = 1;

        //所有的消息处理函数
        protected OnNetMsgHandler[] _allMsgDealFuncs;

        public delegate void OnNetMsgHandler(Deserializer reader);

        public Action OnConnected;

        public void RegisterMsgHandler(int msgType, OnNetMsgHandler handler){
            _allMsgDealFuncs[msgType] = handler;
        }


        public void Init(string ip, int port, string key, int maxMsgHandlerIdx){
            _allMsgDealFuncs = new OnNetMsgHandler[maxMsgHandlerIdx];
            _key = key;
            this._ip = ip;
            this._port = port;
            _listener = new EventBasedNetListener();
            _listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) => {
                OnNetMsg(dataReader.GetRemainingBytes());
                dataReader.Recycle();
            };
            _listener.PeerConnectedEvent += (peer) => {
                Debug.Log("Connected!!");
                _peer = peer;
                OnConnected?.Invoke();
            };
            _client = new NetManager(_listener) {
                DisconnectTimeout = 300000
            };
        }

        public void DoStart(){
            _client.Start();
            _client.Connect(_ip, _port, _key);
        }

        public void DoDestroy(){
            _client.Stop();
        }

        public void DoUpdate(){
            AutoConnect();
            _client.PollEvents();
        }

        private void AutoConnect(){
            _autoConnTimer += Time.deltaTime;
            if (_autoConnTimer > AutoConnInterval && !Connected) {
                _autoConnTimer = 0;
                _client.Connect(_ip, _port, _key);
            }
        }

        private void OnNetMsg(byte[] rawData){
            var data = Compressor.Decompress(rawData);
            var reader = new Deserializer(data);
            var msgTypeID = reader.GetByte();
            if (msgTypeID >= _allMsgDealFuncs.Length) {
                Debug.LogError("Recv error msg type" + msgTypeID);
                return;
            }

            var func = _allMsgDealFuncs[msgTypeID];
            if (func != null) {
                func(reader);
            }
            else {
                Debug.LogError("ErrorMsg type :no msgHandler" + msgTypeID);
            }
        }

        public void Send(byte[] data){
            _client.FirstPeer.Send(data, DeliveryMethod.ReliableOrdered);
        }
    }
}