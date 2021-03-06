using Lockstep.Serialization;

namespace Lockstep.Server.Common {
    public interface IServer {
        bool IsMaster { get;  }

        bool IsCandidateMaster { get;  }

        //life cycle
        void DoStart();
        void DoUpdate(int deltaTime);
        void DoDestroy();
        void PollEvents();

        void OnRecvMsg(byte[] msg);
    }
}