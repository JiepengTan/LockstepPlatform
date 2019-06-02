using System.Net;
using Lockstep.Serialization;
using NetMsg.Server;
using Lockstep.Server.Common;

namespace Lockstep.Server.Common {
    public class BaseServer : IGameServer {
        protected ConfigInfo _allConfig;
        protected ServerConfigInfo _serverConfig;

        public ServerProxy MasterServer;
        public ServerProxy CandidateMasterServer;

        public IPEndPoint ipInfo;
        public EServerType serverType;
        protected EMasterType masterType;
        public bool IsMaster => masterType == EMasterType.Master;
        public bool IsCandidateMaster => masterType == EMasterType.CandidateMaster;

        public virtual void DoAwake(ServerConfigInfo info){
            serverType = info.type;
            _allConfig = ServerUtil.LoadConfig();
            _allConfig.daemonPort = _allConfig.GetServerConfig(EServerType.DaemonServer).servePort;
            _serverConfig = info;
            masterType = _allConfig.isMaster ? EMasterType.Master : EMasterType.Slave;
        }

        public virtual void DoStart(){ }
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