using Lockstep.Serialization;

namespace Lockstep.Server.Common {
    public interface IGameServer {
        bool IsMaster { get; set; }

        bool IsCandidateMaster { get; set; }

        //life cycle
        void DoStart(ushort tcpPort, ushort udpPort);
        void DoUpdate(int deltaTime);
        void DoDestroy();
        void PollEvents();

        void OnRecvMsg(byte[] msg);
    }
}