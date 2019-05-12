using System.Collections.Generic;

namespace Server.Common {
    public interface ILobby {
        
        //life cycle
        void DoStart(int port);
        void DoUpdate(int deltaTime);
        void DoDestroy();
        
        //room operator
        List<IRoom> GetRooms(int roomType);
        IRoom GetRoom(int roomId);
        IRoom GetRoomByUserID(int id);
        IRoom CreateRoom(int type, Player master, string roomName);
        void RemoveRoom(IRoom room);
        bool JoinRoom(long playerID, int roomID);
        bool LeaveRoom(long playerID);
        
        //players
        void TickOut(Player player,int reason);
        Player GetPlayer(long playerId);
        
        //Net status
        void OnClientConnected(object peer);
        void OnCilentDisconnected(object peer);
        //msg handle 
        void OnDataReceived(int netID, byte[] arr);
    }
}