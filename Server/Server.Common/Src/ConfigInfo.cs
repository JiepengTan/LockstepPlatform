using LitJson;
using Lockstep.Server.Common;

namespace Lockstep.Server.Common {
    public class ServerConfigInfo {
        public EServerType type;
        public bool isMaster;
        public string path;
        public int servePort;
        public string masterIp;
        public int masterPort;
        public int tcpPort;
        public int udpPort;
        public override string ToString(){
            return JsonMapper.ToJson(this);
        }
    }
    public class ConfigInfo {
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