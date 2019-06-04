using System;
using System.Collections.Generic;
using LiteNetLib;
using Lockstep.Networking;
using Lockstep.Server.Common;
using Lockstep.Util;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Lockstep.Networking
{
    /// <summary>
    /// Server socket, which accepts websocket connections
    /// </summary>
    public class ServerSocketLn : IServerSocket
    {
        public event PeerActionHandler Connected;
        public event PeerActionHandler Disconnected;

        private NetManager _server;
        private EventBasedNetListener _listener;
        public Dictionary<int ,PeerLn> id2Peer = new Dictionary<int, PeerLn>();

        private float _initialDelay = 0;

        public ServerSocketLn()
        {
            
        }

        public event PeerActionHandler OnConnected
        {
            add { Connected += value; }
            remove { Connected -= value; }
        }

        public event PeerActionHandler OnDisconnected
        {
            add { Disconnected += value; }
            remove { Disconnected -= value; }
        }
        
        /// <summary>
        /// Opens the socket and starts listening to a given port
        /// </summary>
        /// <param name="port"></param>
        public void Listen(int port)
        {
            _listener = new EventBasedNetListener();
            _server = new NetManager(_listener) {
                DisconnectTimeout = 300000,
            };
            _listener.ConnectionRequestEvent += request => { request.AcceptIfKey(Define.XSKey); };

            _listener.PeerConnectedEvent += pe => {
                var speer = new PeerLn(pe);
                id2Peer[pe.Id] = speer;
                Connected?.Invoke(speer);
            };

            _listener.NetworkReceiveEvent += (pe, reader, method) => {
                var peer = id2Peer[pe.Id];
                peer.HandleDataReceived(reader.GetRemainingBytes(), 0);
            };

            _listener.PeerDisconnectedEvent += (pe, info) => {
                var peer = id2Peer[pe.Id];
                Disconnected?.Invoke(peer);
                peer.NotifyDisconnectEvent();
                id2Peer.Remove(pe.Id);
            };

            _server.Start(port);

        }

        /// <summary>
        /// Stops listening
        /// </summary>
        public void Stop()
        {
            _server.Stop();
        }

        public void Update()
        {
        }

        public void PollEvents(){
            
            _server?.PollEvents();
        }
    }
}