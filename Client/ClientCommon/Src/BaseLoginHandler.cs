using System.Collections.Generic;
using Lockstep.Logging;
using NetMsg.Common;

namespace Lockstep.Client {
    public class BaseLoginHandler : BaseLogger {

        public virtual void OnTickPlayer(byte reason){ }
        public virtual void OnConnectedLoginServer(){ }
        public virtual void OnConnLobby(RoomInfo[] roomInfos){ }
        public virtual void OnRoomInfo(RoomInfo[] roomInfos){ }
        public virtual void OnCreateRoom(RoomInfo info, RoomPlayerInfo[] playerInfos){ }

        public virtual void OnRoomInfoUpdate(RoomInfo[] addInfo, int[] deleteInfos, RoomChangedInfo[] changedInfos){ }

        public virtual void OnStartRoomResult(int reason){ }
        public virtual void OnGameStart(int mapId, byte localId){ }
        public virtual void OnGameStart(Msg_C2G_Hello msg, IPEndInfo tcpEnd){ }

        public virtual void OnLoginFailed(ELoginResult result){
            Log("Login failed reason " + result);
        }

        public virtual void OnGameStartFailed(){ }
        public virtual void OnPlayerJoinRoom(RoomPlayerInfo info){ }
        public virtual void OnPlayerLeaveRoom(long userId){ }
        public virtual void OnRoomChatInfo(RoomChatInfo info){ }
        public virtual void OnPlayerReadyInRoom(long userId, byte state){ }
        public virtual void OnLeaveRoom(){ }
    }
}