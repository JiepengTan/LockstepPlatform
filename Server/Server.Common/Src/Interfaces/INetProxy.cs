using System.Net;
using LiteNetLib;
using Lockstep.Networking;
using Lockstep.Serialization;
using NetMsg.Common;

namespace Lockstep.Server.Common {
    public interface INetProxy {
        void SendMsg(object type, BaseMsg msg);
    }

    public class NetProxy : INetProxy {
        private IPeer _peer;
        public NetProxy(IPeer peer){
            _peer = peer;
        }

        public void SendMsg(object type, BaseMsg data){
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