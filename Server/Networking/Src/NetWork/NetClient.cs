#define DEBUG_FRAME_DELAY
using System;
using Lockstep.Networking;
using Lockstep.Serialization;


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

        private string _ip;
        private int _port;
        private string _key;

        public void RegisterMsgHandler(TMsgType msgType, IncommingMessageHandler handler){
            _allDealFuncs[(short) (object) msgType] = handler;
        }

        public NetClient(int maxMsgHandlerIdx, string[] msgFlags, object msgHandlerObj){
            _allDealFuncs = new IncommingMessageHandler[maxMsgHandlerIdx];
            foreach (var msgFlag in msgFlags) {
                NetworkUtil.RegisterEvent<TMsgType, IncommingMessageHandler>("" + msgFlag, "".Length,
                    RegisterMsgHandler,msgHandlerObj);
            }
        }

        public void Connect(string ip, int port, string key){
            this._ip = ip;
            this._port = port;
            this._key = key;
            _client = new ClientSocketLn();
            _isInit = true;
            for (short i = 0; i < _allDealFuncs.Length; i++) {
                var func = _allDealFuncs[i];
                if (func != null) {
                    _client.SetHandler(i, func);
                }
            }
            _client.Connected += OnConnected;
            _client.Connect(_ip, _port, _key);
        }

        public void DoDestroy(){
            _client.Connected -= OnConnected;
            _isInit = false;
        }

        public void DoUpdate(){
            if (!_isInit) return;
            _client?.Update();
        }

        public void SendMessage(TMsgType type, BaseFormater data){
            _client?.SendMessage((short) (object) type, data);
        }

        public void SendMessage(TMsgType type, BaseFormater data, ResponseCallback responseCallback){
            _client?.SendMessage((short) (object) type, data, responseCallback);
        }
    }
}