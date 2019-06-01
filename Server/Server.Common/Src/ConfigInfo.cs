using LitJson;
using Lockstep.Server.Common;

namespace Server.Common {
    public class ServerConfigInfo {
        public EServerType type;
        public string path;
        public int servePort;
        public int tcpPort;
        public int udpPort;
        public override string ToString(){
            return JsonMapper.ToJson(this);
        }
    }
    public class ConfigInfo {
        public string daemonMasterIpPort;
        public bool isMaster;
        public int daemonPort;
        public ServerConfigInfo[] servers;

        public bool IsMaster(){
            return isMaster;
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