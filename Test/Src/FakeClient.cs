using System.Collections.Generic;
using System.Runtime.InteropServices;
using Lockstep.Client;
using Lockstep.Networking;

namespace Test {
    public class FakeClient : NetworkEntity {
        public static int _RandomSeed = 1175;

        private LoginManager _loginMgr;
        private RoomMsgManager _roomMsgMgr;

        public FakeClient(){
            _roomMsgMgr = AddComponent<RoomMsgManager>();
            _roomMsgMgr.Init(new DebugGameMsgHandler());
            _loginMgr = AddComponent<LoginManager>();
            _loginMgr.Init(_roomMsgMgr, new DebugLoginHandler() {RandomSeed = _RandomSeed}, "127.0.0.1", 7250);
        }
    }
}