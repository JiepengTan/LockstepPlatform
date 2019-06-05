using System.Net;
using LiteNetLib;
using Lockstep.Networking;
using Lockstep.Serialization;

namespace Lockstep.Server.Common {
    public interface INetProxy {
        void SendMsg(object type, BaseFormater msg);
    }

    public class NetProxy : INetProxy {
        private IPeer _peer;
        public NetProxy(IPeer peer){
            _peer = peer;
        }

        public void SendMsg(object type, BaseFormater data){
            _peer?.SendMessage((short)type, data);
        }
    }

    public interface IServerProxy : INetProxy {
        IPEndPoint EndPoint { get; }
        EServerType ServerType { get; set; }
    }

    public class ServerProxy : NetProxy, IServerProxy {
        public ServerProxy(IPeer peer) : base(peer){
            EndPoint = peer.EndPoint;
        }
        public EServerType ServerType { get; set; }
        public IPEndPoint EndPoint { get; }
    }
}