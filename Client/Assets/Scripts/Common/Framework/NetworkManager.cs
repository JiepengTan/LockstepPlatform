using Lockstep.Core;
using Lockstep.Serialization;
using NetMsg.Game;
using NetMsg.Lobby;
using UnityEngine;

namespace Lockstep.Game {
    public partial class NetworkManager : SingletonManager<NetworkManager>,INetworkService {
        public string ServerIp = "127.0.0.1";
        public int ServerPort = 9050;
        public const string ClientKey = "LockstepPlatform";

        private BaseNetProxy _netProxyLobby;
        private BaseNetProxy _netProxyRoom;

        private long _playerID;
        private int _roomId;

        public bool IsConnected {
            get { return _netProxyLobby != null && _netProxyLobby.Connected; }
        }

        public override void DoAwake(IServiceContainer services){
            _netProxyRoom = new BaseNetProxy((int) EMsgCS.EnumCount);
            _netProxyLobby = new BaseNetProxy((int) EMsgCL.EnumCount);
            _eventRegisterService.RegisterEvent<EMsgCL, NetMsgHandler>("OnMsg_L2C", "OnMsg_".Length,
                RegisterMsgHandler);
            _eventRegisterService.RegisterEvent<EMsgCS, NetMsgHandler>("OnMsg_S2C", "OnMsg_".Length,
                RegisterMsgHandler);
            InitLobby(ServerIp, ServerPort, ClientKey);
        }

        public override void DoStart(){
            StartLobby();
        }

        public override void DoUpdate(float elapsedMilliseconds){
            _netProxyLobby?.DoUpdate();
            _netProxyRoom?.DoUpdate();
        }

        public override void DoDestroy(){
            _netProxyLobby?.DoDestroy();
            _netProxyRoom?.DoDestroy();
        }

        public void RegisterMsgHandler(EMsgCL type, NetMsgHandler handler){
            _netProxyLobby.RegisterMsgHandler((byte) type, handler);
        }

        public void RegisterMsgHandler(EMsgCS type, NetMsgHandler handler){
            _netProxyRoom.RegisterMsgHandler((byte) type, handler);
        }

        private void InitLobby(string ip, int port, string key){
            _netProxyLobby.OnConnected += OnConnectedLobby;
            _netProxyLobby.Init(ip, port, key);
            //_netProxyLobby.RegisterMsgHandler((byte) EMsgCL.L2C_ReqInit, OnMsg_L2C_ReqInit);
            //_netProxyLobby.RegisterMsgHandler((byte) EMsgCL.L2C_RoomMsg, OnMsg_L2C_RoomMsg);
        }

        public void InitRoom(string ip, int port, string key){
            _netProxyRoom.OnConnected += OnConnectedRoom;
            _netProxyRoom.Init(ip, port, ClientKey);
            //register msgs

            //_netProxyRoom.RegisterMsgHandler((byte) EMsgCS.S2C_StartGame, OnMsg_S2C_StartGame);
            //_netProxyRoom.RegisterMsgHandler((byte) EMsgCS.S2C_FrameData, OnMsg_S2C_FrameData);
            //_netProxyRoom.RegisterMsgHandler((byte) EMsgCS.S2C_RepMissPack,OnNet_ReqInit);
        }


        public void StartRoom(){
            _netProxyRoom.DoStart();
        }


        public void StartLobby(){
            _netProxyLobby.DoStart();
        }


        public void OnConnectedLobby(){
            SendInitMsg();
        }

        public void OnConnectedRoom(){
            Logging.Debug.Log("OnConnected room");
            SendMsgRoom(EMsgCS.C2S_PlayerReady, new Msg_PlayerReady() {roomId = _roomId});
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

        void OnMsg_L2C_RoomMsg(Deserializer reader){
            var msg = reader.Parse<Msg_CreateRoomResult>();
            _roomId = msg.roomId;
            UnityEngine.Debug.Log("OnMsgLobby_CreateRoom " + msg.port);
            InitRoom(msg.ip, msg.port, ClientKey);
            StartRoom();
        }

        void OnMsg_L2C_ReqInit(Deserializer reader){
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
            SendMsgLobby(EMsgCL.C2L_CreateRoom, new Msg_CreateRoom() {type = 1, name = "FishManRoom", size = 2});
        }


        private void OnMsg_S2C_FrameData(Deserializer reader){
            var msg = reader.Parse<Msg_ServerFrames>();
            EventHelper.Trigger(EEvent.OnServerFrame, msg);
        }

        public void OnMsg_S2C_StartGame(Deserializer reader){
            var msg = reader.Parse<Msg_StartGame>();
            _roomId = msg.RoomID;
            Debug.Log($"Starting simulation. Total actors: {msg.AllActors.Length}. Local ActorID: {msg.ActorID}");
            EventHelper.Trigger(EEvent.OnRoomGameStart, msg);
        }

        public void OnMsg_S2C_LoadingProgress(Deserializer reader){
            var msg = reader.Parse<Msg_AllLoadingProgress>();
            var isDone = msg.isAllDone;
            if (isDone) {
                EventHelper.Trigger(EEvent.OnAllPlayerFinishedLoad, null);
            }
            else {
                EventHelper.Trigger(EEvent.OnLoadingProgress, msg);
            }
            
        }

        public void OnEvent_LoadMapDone(object param){
            var level = (int) param;
            _constStateService.curLevel = level;
            SendMsgRoom(EMsgCS.C2S_LoadingProgress, new Msg_LoadingProgress() {progress = 100});
        }
    }
}