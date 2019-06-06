using LitJson;
using Lockstep.Server.Common;

namespace Lockstep.Server.Common {
    public class ServerConfigInfo {
        public EServerType type;
        public bool isMaster;
        public ushort serverPort;
        public string masterIp;
        public ushort masterPort;
        public ushort tcpPort;
        public ushort udpPort;
        public override string ToString(){
            return JsonMapper.ToJson(this);
        }
    }
    public class ConfigInfo {
        public bool isDebugMode;//所有的服务器都在同一个机器上 方便调试
        public bool isMaster;
        public string YMIp;
        public ushort YMPort;
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