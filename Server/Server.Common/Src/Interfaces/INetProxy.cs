using System.Net;
using LiteNetLib;
using Lockstep.Serialization;

namespace Lockstep.Server.Common {
    public interface INetProxy {
        void SendMsg(byte[] data);
        void SendMsg(object type, BaseFormater msg);
    }

    public class NetProxy : INetProxy {
        private NetPeer _peer;
        public NetProxy(NetPeer peer){
            _peer = peer;
        }

        public void SendMsg(byte[] data){
            _peer?.Send(data, DeliveryMethod.ReliableOrdered);
        }

        public void SendMsg(object type, BaseFormater data){
            var writer = new Serializer();
            writer.PutInt16((short) (object) type);
            data.Serialize(writer);
            var bytes = Compressor.Compress(writer.Data);
            SendMsg(bytes);
        }
    }

    public interface IServerProxy : INetProxy {
        IPEndPoint EndPoint { get; }
        EServerType ServerType { get; set; }
    }

    public class ServerProxy : NetProxy, IServerProxy {
        public ServerProxy(NetPeer peer) : base(peer){
            Peer = peer;
            EndPoint = peer.EndPoint;
        }
        public EServerType ServerType { get; set; }
        public IPEndPoint EndPoint { get; }
        public NetPeer Peer;
    }
}