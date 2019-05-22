using Lockstep.Serialization;
using NetMsg.Game;

namespace Lockstep.Game {
    public interface INetworkService :IService {
        void SendInput(Msg_PlayerInput msg);
        void SendMsgRoom(EMsgCS msgId, ISerializable body);
    }
}