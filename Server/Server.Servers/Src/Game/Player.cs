using Lockstep.Networking;
using Lockstep.Serialization;
using Lockstep.Util;
using NetMsg.Common;

namespace Lockstep.Server.Game {
    public class Player : BaseRecyclable {
        public long UserId;
        public string Account;
        public string LoginHash;
        public byte LocalId;
        public IPeer PeerTcp;
        public IPeer PeerUdp;
        public BaseRoom Room;
        public GameData GameData;
        public int RoomId => Room?.RoomId ?? -1;

        public void OnLeave(){
            PeerTcp = null;
            PeerUdp = null;
        }
        public void SendTcp(EMsgSC type, BaseFormater msg){
            PeerTcp?.SendMessage((short) type, msg);
        }

        public void SendTcp(IMessage message){
            PeerTcp?.SendMessage(message);
        }

        public void SendUdp(byte[] data){
            PeerUdp?.SendMessage((short) EMsgSC.G2C_UdpMessage, data);
            //PeerUdp.SendMessage((short) EMsgSC.G2C_UdpMessage, data,EDeliveryMethod.Unreliable);
        }

        public override void OnRecycle(){
            PeerTcp = null;
            PeerUdp = null;
        }
    }
}