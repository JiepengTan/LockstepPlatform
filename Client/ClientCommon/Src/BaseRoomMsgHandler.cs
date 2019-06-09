using NetMsg.Common;

namespace Lockstep.Client {
    public interface IRoomMsgHandler {
        void OnServerFrames(Msg_ServerFrames msg);
        void OnMissFrames(Msg_ServerFrames msg);
        void OnGameEvent(byte[] data);
        void OnLoadingProgress(byte[] progresses);
        void OnGameInfo(Msg_G2C_GameStartInfo msg);
    }

    public class BaseRoomMsgHandler : IRoomMsgHandler {
        public virtual void OnServerFrames(Msg_ServerFrames msg){ }
        public virtual void OnMissFrames(Msg_ServerFrames msg){ }
        public virtual void OnGameEvent(byte[] data){ }
        public virtual void OnGameStartInfo(Msg_G2C_GameStartInfo data){ }
        public virtual void OnLoadingProgress(byte[] progresses){ }
        public virtual void OnGameInfo(Msg_G2C_GameStartInfo msg){ }
        public virtual void OnTcpHello(int mapId, byte localId){ }
        public virtual void OnGameStartFailed(){ }
    }
}