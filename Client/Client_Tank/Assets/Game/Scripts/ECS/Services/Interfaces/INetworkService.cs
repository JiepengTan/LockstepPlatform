using System.Collections.Generic;
using Lockstep.Serialization;
using NetMsg.Game;

namespace Lockstep.Game {
    public interface INetworkService :IService {
        int Ping { get; }
        void SendInput(Msg_PlayerInput msg);
        void SendHashCodes(int startTick,List<long> hashCodes,int startIdx,int count);
        void SendMissFrameReq(int missFrameTick);
        void SendMissFrameRepAck(int missFrameTick);
    }
}