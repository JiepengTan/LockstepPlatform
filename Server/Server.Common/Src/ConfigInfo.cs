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

        public ushort GetDetailPort(byte detailPort){
            var type = (EServerDetailPortType) detailPort;
            switch (type) {
                case EServerDetailPortType.ServerPort: return serverPort ; break;
                case EServerDetailPortType.TcpPort: return tcpPort ; break;
                case EServerDetailPortType.UdpPort: return udpPort ; break;
            }

            return serverPort;
        }
    }
    public class ConfigInfo {
        public bool isDebugMode;//所有的服务器都在同一个机器上 方便调试
        public bool isMaster;
        public string YMIp;
        public ushort YMPort;
        public ushort DeamonPort;
         
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