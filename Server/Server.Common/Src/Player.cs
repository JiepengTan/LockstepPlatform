using System.Runtime.Serialization;
using LiteNetLib;
using Lockstep.Serialization;

namespace Server.Common {
    public enum EPlayerStatus {
        Unconnected,
        Idle,
        Sit,
        ReadyToPlay,
        Playing,
    }

    public class Player {
        public string name;
        public long PlayerId;
        public EPlayerStatus status = EPlayerStatus.Idle;
        public IRoom room;
        public int RoomId => room == null ? 0 : room.RoomId;
        public NetPeer lobbySock;
        public NetPeer gameSock;
        
        public int lastActiveTime; // in sec
        public byte localId;
        
        public void SendLobby(byte[] data){
            lobbySock?.Send(data, DeliveryMethod.ReliableOrdered);
        }

        public void SendRoom(byte[] data){
            gameSock?.Send(data, DeliveryMethod.ReliableOrdered);
        }

        public override string ToString(){
            return $"playerID:{PlayerId} name:{name} roomID:{RoomId} localID:{localId}";
        }
    }
}