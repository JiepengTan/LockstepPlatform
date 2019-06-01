using System.Net;
using Lockstep.Serialization;
using NetMsg.Server;
using Server.Common;

namespace Lockstep.Server.Common {

    
    public class BaseServer : IGameServer {
        protected ConfigInfo _config;
        
        public ServerProxy masterServer;
        public ServerProxy candidateMasterServer;

        public IPEndPoint ipInfo;
        private EMasterType masterType;
        public EServerType serverType;
        public bool IsMaster { get; set; }
        public bool IsCandidateMaster { get; set; }

        public virtual void DoAwake(ServerConfigInfo info){
            serverType = info.type;
            _config = ServerUtil.LoadConfig();
        }
        public virtual void DoStart(ServerConfigInfo info){ }
        public virtual void DoStart(ushort tcpPort, ushort udpPort){ }
        public virtual void DoUpdate(int deltaTime){ }
        public virtual void DoDestroy(){ }
        public virtual void PollEvents(){ }

        public virtual void OnMasterCrash(){ }
        public virtual void OnBecomeMaster(){ }
        public virtual void OnBecomeCandidateMaster(){ }

        public virtual void SendToMaster(EMsgMS type, BaseFormater data){ }
        public virtual void OnRecvMsg(byte[] data){ }
    }
}