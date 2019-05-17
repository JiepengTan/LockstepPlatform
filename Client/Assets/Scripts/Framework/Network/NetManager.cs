using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using Lockstep.Core;
using Lockstep.Serialization;
using NetMsg.Game.Tank;
using NetMsg.Lobby;
using UnityEngine;

namespace Lockstep.Game {
    public class NetMgr {
        private NetProxyLobby _netProxyLobby;
        private NetProxyRoom _netProxyRoom;

        private long _playerID;
        private int _roomId;
        private Simulation _simulation;
        private string key;
        public bool IsConnected {
            get { return _netProxyLobby!=null && _netProxyLobby.Connected; }
        }

        public void Init(Simulation simulation, string ip, int port, string key){
            this._simulation = simulation;
            InitLobby(ip, port, key);
            this.key = key;
        }

        private void InitLobby(string ip, int port, string key){
            _netProxyLobby = new NetProxyLobby();
            _netProxyLobby.OnConnected += OnConnectedLobby;
            _netProxyLobby.Init(ip, port, key, (int) EMsgCL.EnumCount);
            _netProxyLobby.RegisterMsgHandler((byte) EMsgCL.L2C_ReqInit, OnMsgLobby_ReqInit);
            _netProxyLobby.RegisterMsgHandler((byte) EMsgCL.L2C_RoomMsg, OnMsgLobby_CreateRoom);
            
        }

        public void InitRoom(string ip, int port, string key){
            _netProxyRoom = new NetProxyRoom();
            _netProxyRoom.OnConnected += OnConnectedRoom;
            _netProxyRoom.Init(ip, port, key, (int) EMsgCS.EnumCount);
            //register msgs

            _netProxyRoom.RegisterMsgHandler((byte) EMsgCS.S2C_StartGame, OnMsgRoom_StartGame);
            _netProxyRoom.RegisterMsgHandler((byte) EMsgCS.S2C_FrameData, OnMsgRoom_FrameData);
            //_netProxyRoom.RegisterMsgHandler((byte) EMsgCS.S2C_RepMissPack,OnNet_ReqInit);
        }


        public void StartRoom(){
            _netProxyRoom.DoStart();
        }

        public void StartLobby(){
            _netProxyLobby.DoStart();
        }


        public void DoDestroy(){
            _netProxyLobby?.DoDestroy();
            _netProxyRoom?.DoDestroy();
        }

        public void DoUpdate(float elapsedMilliseconds){
            _netProxyLobby?.DoUpdate();
            _netProxyRoom?.DoUpdate();
        }


        public void OnConnectedLobby(){
            SendInitMsg();
        }

        public void OnConnectedRoom(){
            Logging.Debug.Log("OnConnected room");
            SendMsgRoom(EMsgCS.C2S_PlayerReady, new Msg_PlayerReady(){roomId = _roomId});
        }

        public void SendInput(Msg_PlayerInput msg){
            SendMsgRoom(EMsgCS.C2S_PlayerInput, msg);
        }

        public void SendMsgLobby(EMsgCL msgId, ISerializable body){
            var writer = new Serializer();
            writer.Put((byte) msgId);
            writer.Put(_playerID);
            body.Serialize(writer);
            _netProxyLobby.Send(Compressor.Compress(writer));
        }

        public void SendMsgRoom(EMsgCS msgId, ISerializable body){
            var writer = new Serializer();
            writer.Put(_playerID);
            writer.Put((byte) msgId);
            body.Serialize(writer);
            _netProxyRoom.Send(Compressor.Compress(writer));
        }
        
        void OnMsgLobby_CreateRoom(Deserializer reader){
            var msg = reader.Parse<Msg_CreateRoomResult>();
            _roomId = msg.roomId;
            UnityEngine.Debug.Log("OnMsgLobby_CreateRoom " + msg.port);
            InitRoom(msg.ip, msg.port, key);
            StartRoom();
        }
        
        void OnMsgLobby_ReqInit(Deserializer reader){
            var msg = reader.Parse<Msg_RepInit>();
            _playerID = msg.playerId;
            Debug.Log("PlayerID " + _playerID);
            SendCreateRoomMsg();
        }

        void SendInitMsg(){
            SendMsgLobby(EMsgCL.C2L_InitMsg, new Msg_RoomInitMsg() {name = "FishMan"});
        }

        void SendCreateRoomMsg(){
            UnityEngine.Debug.Log("SendCreateRoomMsg");
            SendMsgLobby(EMsgCL.C2L_CreateRoom, new Msg_CreateRoom() {type = 1, name = "FishManRoom"});
        }


        private void OnMsgRoom_FrameData(Deserializer reader){
            var msg = reader.Parse<Msg_ServerFrames>();
            EventHelper.Trigger(EEvent.OnServerFrame, msg);
        }

        public void OnMsgRoom_StartGame(Deserializer reader){
            var msg = reader.Parse<Msg_StartGame>();
            _roomId = msg.RoomID;
            Debug.Log($"Starting simulation. Total actors: {msg.AllActors.Length}. Local ActorID: {msg.ActorID}");

            EventHelper.Trigger(EEvent.OnRoomGameStart, msg);
        }
    }
}