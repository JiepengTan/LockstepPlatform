using System;
using System.Linq;
using LiteNetLib;
namespace Lockstep.Server.Common
{
    public class NetServer : IServer
    {
        public event Action<object> ClientConnected;
        public event Action<object> ClientDisconnected;
        public event Action<NetPeer, byte[]> DataReceived;

        private readonly NetManager _server;
        private readonly EventBasedNetListener _listener;
        private string _clientKey;
        public NetServer(string clientKey){
            _clientKey = clientKey;
            _listener = new EventBasedNetListener();
            _server = new NetManager(_listener)
            {
                DisconnectTimeout = 300000,
            };
        }

        public void Distribute(byte[] data)
        {
            _server.SendToAll(data, DeliveryMethod.ReliableOrdered);
        }

        public void Distribute(int clientId, byte[] data)
        {
            _server.SendToAll(data, DeliveryMethod.ReliableOrdered, _server.ConnectedPeerList.First(peer => peer.Id == clientId));
        }

        public void Send(int clientId, byte[] data)
        {
            _server.ConnectedPeerList.First(peer => peer.Id == clientId).Send(data, DeliveryMethod.ReliableOrdered);
        }

        public void Run(int port)
        {
            _listener.ConnectionRequestEvent += request =>
            {       
                request.AcceptIfKey(_clientKey);      
            };

            _listener.PeerConnectedEvent += peer =>
            {
                ClientConnected?.Invoke(peer);
            };

            _listener.NetworkReceiveEvent += (peer, reader, method) =>
            {
                DataReceived?.Invoke(peer, reader.GetRemainingBytes());
            };

            _listener.PeerDisconnectedEvent += (peer, info) =>
            {
                ClientDisconnected?.Invoke(peer);
            };

            _server.Start(port);
        }

        public void PollEvents()
        {
            _server.PollEvents();
        }
    }
}
