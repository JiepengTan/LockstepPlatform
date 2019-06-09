#define DEBUG_FRAME_DELAY
using System;
using Lockstep.Logging;
using Lockstep.Networking;
using Lockstep.Serialization;
using Lockstep.Util;


namespace Lockstep.Networking {
    public interface IPollEvents {
        void PollEvents();
    }

    public interface IUpdate {
        void DoUpdate();
    }

    public class NetClient<TMsgType> : IUpdate where TMsgType : struct {
        protected ClientSocketLn _client;

        //所有的消息处理函数
        protected IncommingMessageHandler[] _allDealFuncs;

        private bool _isInit = false;
        public Action OnConnected;
        public bool IsConnected => _client?.IsConnected ?? false;

        private float nextCheckConnectTimeStamp = 0;
        public float ReconnectInterval = 1;

        public void RegisterMsgHandler(TMsgType msgType, IncommingMessageHandler handler){
            _allDealFuncs[(short) (object) msgType] = handler;
        }

        public NetClient(int maxMsgHandlerIdx, string[] msgFlags, object msgHandlerObj){
            _allDealFuncs = new IncommingMessageHandler[maxMsgHandlerIdx];
            foreach (var msgFlag in msgFlags) {
                NetworkUtil.RegisterEvent<TMsgType, IncommingMessageHandler>("" + msgFlag, "".Length,
                    RegisterMsgHandler, msgHandlerObj);
            }
        }

        public void Connect(string ip, int port, string key = ""){
            _client = new ClientSocketLn();
            _isInit = true;
            for (short i = 0; i < _allDealFuncs.Length; i++) {
                var func = _allDealFuncs[i];
                if (func != null) {
                    _client.SetHandler(i, func);
                }
            }

            _client.Connected += OnConnected;
            _client.Connect(ip, port, key);
            nextCheckConnectTimeStamp = Time.timeSinceLevelLoad + ReconnectInterval;
        }


        public void DoDestroy(){
            _client.Disconnect();
            _client.Connected -= OnConnected;
            _client = null;
            _isInit = false;
        }

        public void DoUpdate(){
            if (!_isInit) return;
            if (_client != null) {
                //Reconnect
                if (nextCheckConnectTimeStamp < Time.timeSinceLevelLoad) {
                    nextCheckConnectTimeStamp = Time.timeSinceLevelLoad + ReconnectInterval;
                    if (!_client.IsConnected) {
                        _client.Reconnect();
                    }
                }

                _client.Update();
            }
        }

        public void SendMessage(TMsgType type, BaseFormater data){
            _client?.SendMessage((short) (object) type, data);
        }

        public void SendMessage(TMsgType type, BaseFormater data, ResponseCallback responseCallback){
            _client?.SendMessage((short) (object) type, data, responseCallback);
        }
    }
}