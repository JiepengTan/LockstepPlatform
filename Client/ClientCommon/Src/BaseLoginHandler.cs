using NetMsg.Common;

namespace Lockstep.Client {
    public class BaseLoginHandler {
        protected LoginManager _loginMgr;

        public void Init(LoginManager loginMgr){
            this._loginMgr = loginMgr;
        }

        public virtual void OnConnectedLoginServer(){ }
        public virtual void OnRoomInfo(RoomInfo[] roomInfos){ }
        public virtual void OnCreateRoom(RoomInfo info){ }
        public virtual void OnRoomInfoUpdate(){ }
        public virtual void OnStartRoomResult(int reason){ }
        public virtual void OnGameStart(int mapId, byte localId){ }

        public virtual void OnLoginFailed(ELoginResult result){
            _loginMgr.Log("Login failed reason " + result);
        }

        public virtual void OnGameStartFailed(){ }
    }
}