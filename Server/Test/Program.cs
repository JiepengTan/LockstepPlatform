using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Lockstep.FakeClient;
using Lockstep.Networking;
using Lockstep.Server.Common;

namespace Test {
    internal class Program {
        public class TestServer {
            public ServerSocketLn ServerSocket;
            protected Dictionary<short, IPacketHandler> Handlers = new Dictionary<short, IPacketHandler>();
            public List<IPeer> peers = new List<IPeer>();

            public void PollEvents(){
                ServerSocket?.PollEvents();
            }

            public void Init(){
                ServerSocket = new ServerSocketLn();
                ServerSocket.Connected += Connected;
                ServerSocket.Disconnected += Disconnected;

                SetHandler(0, HandleEnterWorldRequest);
                SetHandler(1, HandleGetZoneAccess);
                ServerSocket.Listen(7155);
            }

            public virtual void HandleEnterWorldRequest(IIncommingMessage message){
                Console.WriteLine(" HandleEnterWorldRequest");
                message.Respond("heheh resp HandleEnterWorldRequest", EResponseStatus.Success);
            }

            public virtual void HandleGetZoneAccess(IIncommingMessage message){
                Console.WriteLine(" HandleGetZoneAccess");
                message.Respond("heheh resp HandleGetZoneAccess", EResponseStatus.Success);
            }


            public void SendTest(short id){
                foreach (var peer in peers) {
                    peer.SendMessage(MessageHelper.Create((short) id, new byte[4] {1, 2, 3, 4}),
                        EDeliveryMethod.Reliable);
                }
            }

            public void SetHandler(short opCode, IncommingMessageHandler handler){
                Handlers[opCode] = new PacketHandler(opCode, handler);
            }

            private void Connected(IPeer peer){
                // Listen to messages
                peer.MessageReceived += OnMessageReceived;
                peers.Add(peer);
                OnPeerConnected(peer);
                SendTest(2);
            }

            private void Disconnected(IPeer peer){
                // Remove listener to messages
                peer.MessageReceived -= OnMessageReceived;
                peers.Remove(peer);
                OnPeerDisconnected(peer);
                SendTest(3);
            }

            protected virtual void OnPeerConnected(IPeer peer){
                Console.WriteLine(" OnPeerConnected" + peer.Id);
            }

            protected virtual void OnPeerDisconnected(IPeer peer){
                Console.WriteLine(" OnPeerDisconnected" + peer.Id);
            }

            protected virtual void OnMessageReceived(IIncommingMessage message){
                try {
                    IPacketHandler handler;
                    Handlers.TryGetValue(message.OpCode, out handler);
                    handler?.Handle(message);
                }
                catch (Exception e) { }
            }
        }

        public class TestClient {
            public ClientSocketLn ClientSocket;

            public void Update(){
                ClientSocket?.Update();
            }

            public void Init(){
                ClientSocket = new ClientSocketLn();
                ClientSocket.SetHandler((short) 2, HandleMemberPropertyChanged);
                ClientSocket.SetHandler((short) 3, HandleLeftLobbyMsg);
                ClientSocket.Connected += () => { SendTest(0); };
                ClientSocket.Disconnected += () => { SendTest(1); };
                ClientSocket.Connect("127.0.0.1", 7155);
            }

            private void HandleMemberPropertyChanged(IIncommingMessage message){
                Console.WriteLine(" HandleMemberPropertyChanged");
                var bytes = message.AsBytes();
                foreach (var _byte in bytes) {
                    Console.Write(_byte + " ");
                }

                Console.WriteLine();
            }

            private void HandleLeftLobbyMsg(IIncommingMessage message){
                Console.WriteLine(" HandleLeftLobbyMsg");
                var bytes = message.AsBytes();
                foreach (var _byte in bytes) {
                    Console.Write(_byte + " ");
                }

                Console.WriteLine();
            }

            public void SendTest(short id){
                ClientSocket.SendMessage(MessageHelper.Create((short) id, new byte[4] {1, 2, 3, 4}),
                    (status, response) => {
                        Console.WriteLine(id + "respon " + (response == null ? "null" : response.ToString())
                        );
                    }
                );
            }
        }

        private static List<TestServer> servers = new List<TestServer>();
        private static List<TestClient> clients = new List<TestClient>();

        public static void TestNetwork(){
            TestServer ts = new TestServer();
            ts.Init();
            servers.Add(ts);
            Thread.Sleep(TimeSpan.FromSeconds(2));
            TestClient tc = new TestClient();
            tc.Init();
            clients.Add(tc);
            long lastTick = 1;
            int tickInterval = 40;
            var sw = new Stopwatch();
            sw.Start();
            {
                while (true) {
                    var curTick = sw.ElapsedMilliseconds;
                    var elapse = curTick - lastTick;
                    if (elapse >= tickInterval) {
                        lastTick = curTick;
                        foreach (var svr in servers) {
                            svr.PollEvents();
                        }

                        foreach (var client in clients) {
                            client.Update();
                        }
                    }

                    Thread.Sleep(1);
                }
            }
        }

        public static void Main(string[] args){
            //TestNetwork();
            ServerUtil.RunServerInThread(typeof(Lockstep.Server.Servers.Program).Assembly, EServerType.DaemonServer);
            Thread.Sleep(TimeSpan.FromSeconds(3));

            ClientUtil.RunClient();
            while (true) {
                Thread.Sleep(30);
            }
        }
    }
}