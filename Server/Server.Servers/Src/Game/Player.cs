using Lockstep.Networking;
using Lockstep.Serialization;
using Lockstep.Util;
using NetMsg.Common;

namespace Lockstep.Server.Game {
    public enum EPlayerStatus {
        Unconnected,
        Idle,
        Sit,
        ReadyToPlay,
        Playing,
    }


    public class Player : BaseRecyclable {
        public EPlayerStatus Status = EPlayerStatus.Idle;
        public long UserId;
        public string Account;
        public string LoginHash;
        public byte LocalId;
        public IPeer PeerTcp;
        public IPeer PeerUdp;
        private Room _room;
        public Room Room => _room;
        public GameData GameData;
        public bool HasRecvData = false;
        public int RoomId => _room?.RoomId ?? -1;

        public void SetRoom(Room room){
            _room = room;
        }

        public void SendTcp(IMessage message){
            PeerTcp.SendMessage(message);
        }  

        public void SendUdp(byte[] data){
            PeerUdp.SendMessage((short) EMsgSC.G2C_UdpMessage,data);
            //PeerUdp.SendMessage((short) EMsgSC.G2C_UdpMessage, data,EDeliveryMethod.Unreliable);
        }

        public override void OnRecycle(){
            PeerTcp = null;
            PeerUdp = null;
        }
    }
}