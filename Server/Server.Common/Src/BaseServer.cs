using System.Net;
using Lockstep.Serialization;
using Server.Common;

namespace Lockstep.Server.Common {

    
    public class BaseServer : IGameServer {
        public ServerProxy masterServer;
        public ServerProxy candidateMasterServer;

        public IPEndPoint ipInfo;
        private EMasterType masterType;
        public EServerType serverType;
        public bool IsMaster { get; set; }
        public bool IsCandidateMaster { get; set; }
        public virtual void DoStart(ServerConfigInfo config){ }
        public virtual void DoStart(ushort tcpPort, ushort udpPort){ }
        public virtual void DoUpdate(int deltaTime){ }
        public virtual void DoDestroy(){ }
        public virtual void PollEvents(){ }

        public virtual void OnMasterCrash(){ }
        public virtual void OnBecomeMaster(){ }
        public virtual void OnBecomeCandidateMaster(){ }

        public virtual void SendToMaster(EServerMsg type, BaseFormater data){ }
        public virtual void OnRecvMsg(byte[] data){ }
    }

}