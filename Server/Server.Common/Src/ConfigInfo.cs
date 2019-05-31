using LitJson;
using Lockstep.Server.Common;

namespace Server.Common {
    public class ServerConfigInfo {
        public EServerType type;
        public string path;
        public string serveIpPort;
        public string tcpClientIpPort;
        public string udpClientIpPort;

        public override string ToString(){
            return JsonMapper.ToJson(this);
        }
    }
    public class ConfigInfo {
        public string daemonMasterIpPort;
        public ServerConfigInfo daemon;
        public ServerConfigInfo[] servers;

        public bool IsMaster(){
            return daemonMasterIpPort == daemon.serveIpPort;
        }

        public ServerConfigInfo GetServerConfig(EServerType type){
            foreach (var info in servers) {
                if (info.type == type) {
                    return info;
                }
            }
            return null;
        }
    }
}