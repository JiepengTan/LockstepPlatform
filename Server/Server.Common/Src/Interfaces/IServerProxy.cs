using System.Net;
using LiteNetLib;

namespace Lockstep.Server.Common {
    public interface IServerProxy {
        IPEndPoint EndPoint { get; }
        EServerType ServerType { get; set; }
        void SendMsg(byte[] data);
    }

    public class ServerProxy : IServerProxy {
        public ServerProxy(NetPeer peer){
            Peer = peer;
            EndPoint = peer.EndPoint;
        }

        public EServerType ServerType { get; set; }
        public IPEndPoint EndPoint { get; }
        public NetPeer Peer;

        public void SendMsg(byte[] data){
            Peer?.Send(data, DeliveryMethod.ReliableOrdered);
        }
    }
}