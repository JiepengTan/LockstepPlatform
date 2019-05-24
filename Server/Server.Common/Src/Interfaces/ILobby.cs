using System.Collections.Generic;

namespace Server.Common {
    public interface ILobby {
        
        //life cycle
        void DoStart(ushort tcpPort,ushort udpPort);
        void DoUpdate(int deltaTime);
        void DoDestroy();
        
        //room operator
        List<IRoom> GetRooms(int roomType);
        IRoom GetRoom(int roomId);
        IRoom GetRoomByUserID(int id);
        IRoom CreateRoom(int type, Player master, string roomName,byte size);
        void RemoveRoom(IRoom room);
        bool JoinRoom(Player player, int roomID);
        bool LeaveRoom(Player player);
        
        //players
        void TickOut(Player player,int reason);
        Player GetPlayer(long playerId);
        
        //Net status
        void OnClientConnected(object peer);
        void OnCilentDisconnected(object peer);
        //msg handle 
    }
}