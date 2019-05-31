using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Text;
using Lockstep.Logging;
using Lockstep.Serialization;

namespace Lockstep.Server.Common {
    public interface ILobby : IGameServer {
        //room operator
        List<IRoom> GetRooms(int roomType);
        IRoom GetRoom(int roomId);
        IRoom GetRoomByUserID(int id);
        IRoom CreateRoom(int type, Player master, string roomName, byte size);
        void RemoveRoom(IRoom room);
        bool JoinRoom(Player player, int roomID);
        bool LeaveRoom(Player player);

        //players
        void TickOut(Player player, int reason);
        Player GetPlayer(long playerId);

        //Net status
        void OnClientConnected(object peer);

        void OnCilentDisconnected(object peer);
        //msg handle 
    }
}