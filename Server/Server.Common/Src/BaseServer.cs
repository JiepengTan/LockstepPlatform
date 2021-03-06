using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Lockstep.Logging;
using Lockstep.Networking;
using Lockstep.Serialization;
using NetMsg.Server;
using Lockstep.Server.Common;
using Lockstep.Util;
using NetMsg.Common;

namespace Lockstep.Server.Common {

    public class BaseServer :NetworkProxy{
        protected ConfigInfo _allConfig;
        protected ServerConfigInfo _serverConfig;

        public ServerProxy MasterServer;
        public ServerProxy CandidateMasterServer;

        public IPEndPoint ipInfo;
        public EServerType serverType;
        protected EMasterType masterType;
        public bool IsMaster => masterType == EMasterType.Master;
        public bool IsCandidateMaster => masterType == EMasterType.CandidateMaster;

        public string Ip;

        public virtual void DoAwake(ServerConfigInfo info){
            Debug = new DebugInstance(GetType().Name + ": ");
            HasInit = true;

            serverType = info.type;
            _allConfig = ServerUtil.LoadConfig();
            _allConfig.DeamonPort = _allConfig.GetServerConfig(EServerType.DaemonServer).serverPort;
            _serverConfig = info;
            Ip = NetworkHelper.GetLocalIP();
            if (_allConfig.isDebugMode) {
                Ip = "127.0.0.1";
            }
            masterType = _allConfig.isMaster ? EMasterType.Master : EMasterType.Slave;
        }

        public virtual void OnMasterCrash(){ }
        public virtual void OnBecomeMaster(){ }
        public virtual void OnBecomeCandidateMaster(){ }

        public virtual void SendToMaster(EMsgMS type, BaseMsg data){ }
        public virtual void OnRecvMsg(byte[] data){ }
    }
}