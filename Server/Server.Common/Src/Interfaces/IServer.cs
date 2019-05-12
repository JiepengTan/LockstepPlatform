using System;
using LiteNetLib;

namespace Server.Common {
    public interface IServer {
        event Action<object> ClientConnected;
        event Action<object> ClientDisconnected;
        event Action<int, byte[]> DataReceived;

        void Distribute(byte[] data);
        void Distribute(int sourceClientId, byte[] data);
        void Send(int clientId, byte[] data);
        void Run(int port);
    }
}