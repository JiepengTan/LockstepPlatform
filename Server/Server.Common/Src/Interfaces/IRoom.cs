using Lockstep.Serialization;

namespace Lockstep.Server.Common {
    public interface IRecyclable {
        void OnReuse();
        void OnRecycle();
    }

    public interface IRoom : IRecyclable {
        int TypeId { get; }
        int RoomId { get; }
        int CurPlayerCount { get; }
        int MaxPlayerCount { get; }

        long[] GetAllPlayerIDs();
        byte[] GetReconnectMsg(Player player);

        //room life cycle
        void DoStart(int type,int roomId, ILobby server, int size, string name);
        void DoUpdate(int deltaTime);
        void DoDestroy();

        //net status
        void OnReconnect(Player player);
        void OnDisconnect(Player player);

        void OnPlayerEnter(Player player);
        void OnPlayerReady(Player player);
        void OnPlayerLeave(Player player);

        //game status
        void StartGame();
        void FinishedGame();

        //net msg
        void SendTo(Player player, byte[] data);
        void SendToAll(byte[] data);
        void OnRecvMsg(Player player, Deserializer reader);
    }
}