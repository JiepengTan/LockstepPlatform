#define DEBUG_FRAME_DELAY
using System;
using System.Collections;
using LiteNetLib;
using Lockstep.Core.Logic.Interfaces;
using Lockstep.Serialization;
using NetMsg.Game.Tank;
using NetMsg.Lobby;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;
using Input = NetMsg.Game.Tank.Input;

namespace Lockstep.Game {
    public class NetHelper :CommandQueue{
        private Simulation _simulation;
        private string ServerIp;
        private int ServerPort;

        private readonly EventBasedNetListener _listener = new EventBasedNetListener();
        private NetManager _client;
        private NetPeer _peer;
        public bool Connected => _client.FirstPeer?.ConnectionState == ConnectionState.Connected;

        public float AutoConnInterval = 1;
        private float _autoConnTimer;
        private string _netKey;
        private int _roomId;
        private long _playerID;

        public void Init(string ip, int port, Simulation simulation, string key){
            _netKey = key;
            this.ServerIp = ip;
            this.ServerPort = port;
            _simulation = simulation;
            _listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) => {
                OnNetMsg(dataReader.GetRemainingBytes());
                dataReader.Recycle();
            };
            _listener.PeerConnectedEvent += (peer) => {
                Debug.Log("Connected!!");
                _peer = peer;
                SendInitMsg();
            };
            _client = new NetManager(_listener) {
                DisconnectTimeout = 30000
            };
        }

        public void DoStart(){
            _client.Start();
            _client.Connect(ServerIp, ServerPort, _netKey);
        }

        void SendInitMsg(){
            var writer = new Serializer();
            writer.Put((byte) EMsgCL.C2L_InitMsg);
            writer.Put(-1L);
            writer.Put("FishMan");
            Send(Compressor.Compress(writer));
        }

        void OnNet_ReqInit(Deserializer reader){
            _playerID = reader.GetLong();
            Debug.LogError("PlayerID " + _playerID);
            SendCreateRoomMsg();
        }

        void SendCreateRoomMsg(){
            var writer = new Serializer();
            writer.Put((byte) EMsgCL.C2L_CreateRoom);
            writer.Put(_playerID); //playerID
            writer.Put(1); //roomtype
            writer.Put("FishMan"); //name
            Send(Compressor.Compress(writer));
        }

        public void DoDestroy(){
            _client.Stop();
        }

        public void DoUpdate(){
            AutoConnect();
            _client.PollEvents();
        }

        private void AutoConnect(){
            _autoConnTimer += Time.deltaTime;
            if (_autoConnTimer > AutoConnInterval && !Connected) {
                _autoConnTimer = 0;
                //_client.Connect(ServerIp, ServerPort, _netKey);
            }
        }

        private void Send(byte[] data){
            _client.FirstPeer.Send(data, DeliveryMethod.ReliableOrdered);
        }

        private void OnNetMsg(byte[] rawData){
            var data = Compressor.Decompress(rawData);
            var reader = new Deserializer(data);
            var msgTypeID = reader.GetByte();
            //Handler Lobby msg
            if (msgTypeID < (byte) EMsgCL.EnumCount) {
                var messageTag = (EMsgCL) msgTypeID;
                switch (messageTag) {
                    case EMsgCL.L2C_ReqInit:
                        OnNet_ReqInit(reader);
                        break;
                    case EMsgCL.L2C_RoomMsg:
                        OnRoomMsg(reader);
                        break;
                }
            }
            
        }

        void OnRoomMsg(Deserializer reader){
            var msgTypeID = reader.GetByte();
            var messageTag = (EMsgCS) msgTypeID;
            Debug.Log("Deal room msg " + messageTag);
            switch (messageTag) {
                case EMsgCS.S2C_StartGame:
                    OnNet_StartGame(reader);
                    break;
                case EMsgCS.S2C_FrameData:
                    OnNet_FrameData(reader);
                    break;
            }
        }

        public void OnNet_FrameData(Deserializer reader){
            Debug.Log("Recv Frame Data");
            ServerFrames frames = new ServerFrames();
            frames.Deserialize(reader);
            _simulation.OnNetFrame(frames.frames);
        }

        public void OnNet_StartGame(Deserializer reader){
            var init = new InitServerFrame();
            init.Deserialize(reader);
            _roomId = init.RoomID;
            Debug.Log($"Starting simulation. Total actors: {init.AllActors.Length}. Local ActorID: {init.ActorID}");
            _simulation.StartGame(init.RoomID, init.SimulationSpeed, init.ActorID, init.AllActors);
        }

        public void SendPackRequire(uint[] missFrames){
            var data = new ReqMissFrame();
            data.missFrames = missFrames;
            SendRoomMsg(EMsgCS.C2S_ReqMissPack, data);
        }

        public void SendInput(EDir dir, bool isFire){
            var data = new Input();
            //var cmd = new InputCmd();
            //cmd.key = isFire ? (byte) EInputKeys.Fire : (byte) dir;
            //cmd.val1 = isFire ? 1 : 0;
            //data.Commands.Add(cmd);
            data.ActorId = _simulation.LocalActorId;
#if DEBUG_FRAME_DELAY
            data.timeSinceStartUp = Time.realtimeSinceStartup;
#endif
            SendRoomMsg(EMsgCS.C2S_PlayerInput, data);
        }

        private void SendRoomMsg(EMsgCS type, ISerializable body){
            var writer = new Serializer();
            writer.Put((byte) EMsgCL.C2L_RoomMsg);
            writer.Put(_playerID);
            writer.Put((byte) type);
            body.Serialize(writer);
            Send(Compressor.Compress(writer));
        }
    }
}