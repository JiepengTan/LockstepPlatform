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
        public int netID;
        public long PlayerId;
        public EPlayerStatus status;
        public IRoom room;
        public int RoomId => room == null ? 0 : room.RoomId;
        public NetPeer socket;
        public int lastActiveTime; // in sec
        public byte localId;

        public void Send(byte[] data){
            socket?.Send(data, DeliveryMethod.ReliableOrdered);
        }

        public void Send(byte type, Lockstep.Serialization.ISerializable body){
            var writer = new Serializer();
            writer.Put((byte) type);
            body.Serialize(writer);
            var bytes = Compressor.Compress(writer);
            Send(bytes);
        }

        public override string ToString(){
            return $"playerID:{PlayerId} name:{name} roomID:{RoomId} localID:{localId}";
        }
    }
}