using System;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib;
using Lockstep.Serialization;
using Lockstep.Logic.Share;
using Lockstep.Logic.Share.NetMsg;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Logic.Server {
    public class Room : IRoom {
        public int TypeId { get; protected set; }
        public int RoomId { get; protected set; }

        public int CurPlayerCount {
            get { return _allPlayers.Count; }
        }

        public int MaxPlayerCount { get; private set; }

        public bool IsRunning { get; private set; }
        public string name;
        public uint Tick = 0;

        private readonly Dictionary<int, byte> netId2LocalId = new Dictionary<int, byte>();
        private readonly Dictionary<byte, Player> _localId2Player = new Dictionary<byte, Player>();
        private readonly List<Player> _allPlayers = new List<Player>();

        public List<ServerFrame> _historyFrames = new List<ServerFrame>();
        private Dictionary<ulong, long> _hashCodes = new Dictionary<ulong, long>();
        private List<PlayerInput> _tempInputBuffer = new List<PlayerInput>();

        private ILobby _lobby;
        private byte curPlayerLocalId = 0;

        private DealNetMsg[] allMsgDealFuncs = new DealNetMsg[(int) EMsgCS.EnumCount];
        private ParseNetMsg[] allMsgParsers = new ParseNetMsg[(int) EMsgCS.EnumCount];

        private delegate void DealNetMsg(Player player, NetMsgBase data);

        private delegate NetMsgBase ParseNetMsg(Deserializer reader);

        #region  life cycle

        public void DoStart(int type, int roomId, ILobby server, int size, string name){
            this.RoomId = roomId;
            this.MaxPlayerCount = size;
            this.name = name;
            _lobby = server;
            Tick = 0;
            TypeId = type;
            RegisterMsgHandlers();
        }

        public void DoUpdate(int deltaTime){
            var frame = new ServerFrame();
            frame.tick = Tick++;
            frame.inputs = new PlayerInput[_allPlayers.Count];
            foreach (var playerInput in _tempInputBuffer) {
                var idx = playerInput.playerID;
                var input = frame.inputs[idx];
                if (input != null) {
                    input.Combine(playerInput);
                }
                else {
                    frame.inputs[idx] = playerInput;
                }
            }

            _tempInputBuffer.Clear();
            var allFrames = new ServerFrames();
            var frames = new List<ServerFrame>();
            frames.Add(frame);
            var count = _historyFrames.Count;
            for (int i = count - 1, j = 0; i >= 0 && j < 2; i--, j++) {
                frames.Add(_historyFrames[i]);
            }

            allFrames.frames = frames.ToArray();
            SendToAll(EMsgCS.S2C_FrameData, allFrames);
            _historyFrames.Add(frame);
        }

        public void DoDestroy(){
            Debug.Log($"Room {RoomId} Destroy");
        }

        #endregion


        #region net msg

        public void OnRecvMsg(Player player, Deserializer reader){
            if (reader.IsEnd) {
                DealMsgHandlerError(player,$"{player.PlayerId} send a Error:Net Msg");
                return;
            }

            var msgType = reader.GetByte();
            if (msgType >= Lobby.MAX_HANDLER_IDX) {
                DealMsgHandlerError(player,$"{player.PlayerId} send a Error msgType out of range {msgType}");
                return;
            }

            //Debug.Log($"OnDataReceived netID = {netID}  type:{(EMsgCS)msgType}");
            {
                var _func = allMsgDealFuncs[msgType];
                var _parser = allMsgParsers[msgType];
                if (_func != null && _parser != null) {
                    var data = _parser(reader);
                    if (data == null) {
                        DealMsgHandlerError(player,$"ErrorMsg type :parser data error playerID = {player.PlayerId} msgType:{msgType}");
                        return;
                    }

                    _func(player, data);
                }
                else {
                    DealMsgHandlerError(player,$" {player.PlayerId} ErrorMsg type :no msg handler or parser {msgType}");
                }
            }
        }

        void DealMsgHandlerError(Player player, string msg){
            Debug.LogError(msg);
            TickOut(player, 0);
        }

        public void TickOut(Player player,int reason){
            _lobby.TickOut(player,reason);
        }

        private void RegisterMsgHandlers(){
            RegisterNetMsgHandler(EMsgCS.C2S_PlayerInput, OnNet_PlayerInput,
                (reader) => { return ParseData<PlayerInput>(reader); });
            RegisterNetMsgHandler(EMsgCS.C2S_ReqMissPack, OnNet_ReqMissPack, null);
            RegisterNetMsgHandler(EMsgCS.C2S_HashCode, OnNet_HashCode, null);
        }

        private void RegisterNetMsgHandler(EMsgCS type, DealNetMsg func, ParseNetMsg parseFunc){
            allMsgDealFuncs[(int) type] = func;
            allMsgParsers[(int) type] = parseFunc;
        }

        T ParseData<T>(Deserializer reader) where T : NetMsgBase, new(){
            T data = null;
            try {
                data = reader.Parse<T>();
                if (!reader.IsEnd) {
                    data = null;
                }
            }
            catch (Exception e) {
                data = null;
            }

            return data;
        }


        public void SendTo(Player player, byte[] data){
            player.Send(data);
        }

        public void SendToAll(byte[] data){
            foreach (var player in _allPlayers) {
                SendTo(player, data);
            }
        }

        public void SendTo(Player player, EMsgCS type, ISerializable body){
            player.Send(type, body);
        }

        public void SendToAll(EMsgCS type, ISerializable body){
            var writer = new Serializer();
            writer.Put((byte) type);
            body.Serialize(writer);
            var data = Compressor.Compress(writer);
            foreach (var player in _allPlayers) {
                SendTo(player, data);
            }
        }

        #endregion


        #region player operator

        public void OnPlayerEnter(Player player){
            if (_allPlayers.Contains(player)) {
                Debug.LogError("Player already exist" + player.PlayerId);
                return;
            }

            Debug.Log($"Player{player.PlayerId} Enter room {RoomId}");
            player.room = this;
            var localId = curPlayerLocalId++;
            player.localId = localId;
            netId2LocalId[player.netID] = localId;
            _localId2Player[localId] = player;
            _allPlayers.Add(player);

            if (CurPlayerCount == MaxPlayerCount) {
                Console.WriteLine("Room is full, starting new simulation...");
                StartSimulationOnConnectedPeers();
            }
            else {
                Console.WriteLine(netId2LocalId.Count + " / " + MaxPlayerCount + " players have connected.");
            }
        }

        public void OnPlayerReady(Player player){ }

        public void OnPlayerLeave(Player player){
            Debug.Log($"Player{player.PlayerId} Leave room {RoomId}");
            _allPlayers.Remove(player);
            _localId2Player.Remove(player.localId);
            netId2LocalId.Remove(player.netID);
            if (netId2LocalId.Count == 0) {
                Console.WriteLine("All players left, stopping current simulation...");
                IsRunning = false;
                _lobby.RemoveRoom(this);
            }
            else {
                Console.WriteLine(CurPlayerCount + " players remaining.");
            }
        }

        #endregion

        #region net status

        //net status
        public void OnReconnect(Player player){ }
        public void OnDisconnect(Player player){ }

        #endregion

        #region game status

        public bool CanStartGame(){
            return true;
        }

        public void StartGame(){ }
        public void FinishedGame(){ }

        #endregion


        #region IRecyclable

//回收时候调用
        public void OnReuse(){ }
        public void OnRecyle(){ }

        #endregion

        #region Net msg handler

        void OnNet_PlayerInput(Player player, NetMsgBase data){
            _tempInputBuffer.Add(data as PlayerInput);
        }

        void OnNet_ReqMissPack(Player player, NetMsgBase data){ }
        void OnNet_HashCode(Player player, NetMsgBase data){ }

        #endregion

        private void StartSimulationOnConnectedPeers(){
            var seed = new Random().Next(int.MinValue, int.MaxValue);
            foreach (var player in _allPlayers) {
                SendTo(player, EMsgCS.S2C_StartGame, new InitServerFrame {
                    RoomID = RoomId,
                    Seed = seed,
                    ActorID = player.localId,
                    AllActors = netId2LocalId.Values.ToArray(),
                    SimulationSpeed = Define.SimulationSpeed
                });
            }
        }
    }
}