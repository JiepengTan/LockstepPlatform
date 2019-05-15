using System;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib;
using Lockstep.Serialization;
using NetMsg.Game.Tank;
using NetMsg.Lobby;
using Server.Common;
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

        private ILobby _lobby;
        private byte curPlayerLocalId = 0;

        private DealNetMsg[] allMsgDealFuncs = new DealNetMsg[(int) EMsgCS.EnumCount];
        private ParseNetMsg[] allMsgParsers = new ParseNetMsg[(int) EMsgCS.EnumCount];

        private bool haveStart = false; //是否已经开始游戏

        private delegate void DealNetMsg(Player player, BaseFormater data);

        private delegate BaseFormater ParseNetMsg(Deserializer reader);

        #region  life cycle

        public void DoStart(int type, int roomId, ILobby server, int size, string name){
            this.RoomId = roomId;
            this.MaxPlayerCount = size;
            this.name = name;
            _lobby = server;
            Tick = 0;
            TypeId = type;
            timeSinceLoaded = 0;
            firstFrameTimeStamp = 0;
            allHistoryFrames.Clear();
            RegisterMsgHandlers();
        }

        public void DoUpdate(int deltaTime){
            timeSinceLoaded += deltaTime;
            BorderServerFrame(deltaTime);
        }

        public long timeSinceLoaded;
        private long firstFrameTimeStamp = 0;
        private int MaxWaitTime = 300;//最多等待事时间
        private int waitTimer = 0;
        
        //所有需要等待输入到来的Ids
        public List<byte> allNeedWaitInputPlayerIds;
        List<ServerFrame> allHistoryFrames = new List<ServerFrame>();//所有的历史帧
        private void BorderServerFrame(int deltaTime){
            waitTimer += deltaTime;
            if (!haveStart) return;
            while (true) {// 如果落后太多 就一直追帧
                var iTick = (int) Tick;
                if (allHistoryFrames.Count <= iTick) {
                    return;
                }
                var frame = allHistoryFrames[iTick];
                if (frame == null) {
                    return;
                }
                
                var inputs = frame.inputs;
                //超时等待 移除超时玩家
                if (waitTimer > MaxWaitTime) {
                    waitTimer = 0;
                    //移除还没有到来的帧的Player
                    for (int i = 0; i < inputs.Length; i++) {
                        if (inputs[i] == null) {
                            Debug.Log($"Overtime wait remove localId = {i}" );
                            allNeedWaitInputPlayerIds.Remove((byte)i);
                        }
                    }
                }
                
                //是否所有的输入  都已经等到
                foreach (var id in allNeedWaitInputPlayerIds) {
                    if (inputs[id] == null) {
                        return;
                    }
                }
                Debug.Log("Border input " + Tick);
                var allFrames = new ServerFrames();
                int count = Tick < 2 ? iTick + 1 : 3;
                var frames = new ServerFrame[count];
                for (int i = 0; i <count; i++) {
                    frames[count - i - 1] = allHistoryFrames[iTick - i];
                }
                allFrames.frames = frames;
                SendToAll(EMsgCS.S2C_FrameData, allFrames);
                if (firstFrameTimeStamp <= 0) {
                    firstFrameTimeStamp = timeSinceLoaded;
                }
                Tick++;
            }
        }


        public void DoDestroy(){
            Debug.Log($"Room {RoomId} Destroy");
        }

        #endregion


        #region net msg

        public void OnRecvMsg(Player player, Deserializer reader){
            if (reader.IsEnd) {
                DealMsgHandlerError(player, $"{player.PlayerId} send a Error:Net Msg");
                return;
            }

            var msgType = reader.GetByte();
            if (msgType >= (byte) EMsgCS.EnumCount) {
                DealMsgHandlerError(player, $"{player.PlayerId} send a Error msgType out of range {msgType}");
                return;
            }

            //Debug.Log($"OnDataReceived netID = {player.localId}  type:{(EMsgCS) msgType}");
            {
                var _func = allMsgDealFuncs[msgType];
                var _parser = allMsgParsers[msgType];
                if (_func != null && _parser != null) {
                    var data = _parser(reader);
                    if (data == null) {
                        DealMsgHandlerError(player,
                            $"ErrorMsg type :parser data error playerID = {player.PlayerId} msgType:{msgType}");
                        return;
                    }

                    _func(player, data);
                }
                else {
                    DealMsgHandlerError(player,
                        $" {player.PlayerId} ErrorMsg type :no msg handler or parser {msgType}");
                }
            }
        }

        void DealMsgHandlerError(Player player, string msg){
            Debug.LogError(msg);
            TickOut(player, 0);
        }

        public void TickOut(Player player, int reason){
            _lobby.TickOut(player, reason);
        }

        private void RegisterMsgHandlers(){
            RegisterNetMsgHandler(EMsgCS.C2S_PlayerInput, OnNet_PlayerInput,
                (reader) => { return ParseData<PlayerInput>(reader); });
            RegisterNetMsgHandler(EMsgCS.C2S_ReqMissPack, OnNet_ReqMissPack, null);
            RegisterNetMsgHandler(EMsgCS.C2S_HashCode, OnNet_HashCode, null);
            RegisterNetMsgHandler(EMsgCS.C2S_PlayerReady, OnNet_PlayerReady,
                (reader) => { return ParseData<Msg_PlayerReady>(reader); });
        }

        private void RegisterNetMsgHandler(EMsgCS type, DealNetMsg func, ParseNetMsg parseFunc){
            allMsgDealFuncs[(int) type] = func;
            allMsgParsers[(int) type] = parseFunc;
        }

        T ParseData<T>(Deserializer reader) where T : BaseFormater, new(){
            T data = null;
            try {
                data = reader.Parse<T>();
                if (!reader.IsEnd) {
                    data = null;
                }
            }
            catch (Exception e) {
                Debug.LogError("Parse Msg Error:" + e);
                data = null;
            }

            return data;
        }


        public void SendTo(Player player, byte[] data){
            player.SendRoom(data);
        }

        public void SendToAll(byte[] data){
            foreach (var player in _allPlayers) {
                SendTo(player, data);
            }
        }

        public void SendTo(Player player, EMsgCS type, ISerializable body){
            var writer = new Serializer();
            writer.Put((byte) type);
            body.Serialize(writer);
            var bytes = Compressor.Compress(writer);
            player.SendRoom(bytes);
        }

        public void SendToAll(EMsgCS type, ISerializable body){
            var writer = new Serializer();
            writer.Put((byte) type);
            body.Serialize(writer);
            var bytes = Compressor.Compress(writer);
            foreach (var player in _allPlayers) {
                player.SendRoom(bytes);
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
        }

        public void OnPlayerReady(Player tempplayer){
            Debug.Log("OnPlayerReady " + tempplayer.PlayerId);
            tempplayer.status = EPlayerStatus.ReadyToPlay;
            int readyCount = 0;
            foreach (var player in _allPlayers) {
                if (player.status == EPlayerStatus.ReadyToPlay) {
                    readyCount++;
                }
            }

            if (readyCount == MaxPlayerCount) {
                Console.WriteLine("Room is full, starting new simulation...");
                StartGame();
            }
            else {
                Console.WriteLine(readyCount + " / " + MaxPlayerCount + " players have ready.");
            }
        }

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

        public void StartGame(){
            allNeedWaitInputPlayerIds = new List<byte>();
            foreach (var val in netId2LocalId.Values) {
                allNeedWaitInputPlayerIds.Add(val);
            }

            StartSimulationOnConnectedPeers();
        }

        public void FinishedGame(){ }

        #endregion


        #region IRecyclable

        //回收时候调用
        public void OnReuse(){ }
        public void OnRecyle(){ }

        #endregion

        #region Net msg handler



        void OnNet_PlayerInput(Player player, BaseFormater data){
            haveStart = true;
            var input = data as PlayerInput;
            Debug.Log($"RecvInput actorID:{input.ActorId} inputTick:{input.Tick} Tick{Tick}");
            if (input.Tick < Tick) {
                return;
            }

            var tick = input.Tick;
            var iTick = (int) tick;
            //扩充帧队列
            var frameCount = allHistoryFrames.Count;
            if (frameCount <= iTick) {
                var count = iTick - allHistoryFrames.Count + 1;
                for (int i = 0; i < count; i++) {
                    allHistoryFrames.Add(null);
                }
            }

            if (allHistoryFrames[iTick] == null) {
                allHistoryFrames[iTick] = new ServerFrame() {tick = tick};
            }

            var frame = allHistoryFrames[iTick];
            if (frame.inputs == null || frame.inputs.Length != MaxPlayerCount) {
                frame.inputs = new PlayerInput[MaxPlayerCount];
            }

            var id = input.ActorId;
            if (!allNeedWaitInputPlayerIds.Contains(id)) {
                allNeedWaitInputPlayerIds.Add(id);
            }

            frame.inputs[id] = input;
        }

        void OnNet_ReqMissPack(Player player, BaseFormater data){ }
        void OnNet_HashCode(Player player, BaseFormater data){ }

        void OnNet_PlayerReady(Player player, BaseFormater data){
            OnPlayerReady(player);
        }

        #endregion

        private void StartSimulationOnConnectedPeers(){
            var seed = new Random().Next(int.MinValue, int.MaxValue);
            var ids = netId2LocalId.Values.ToArray();
            foreach (var player in _allPlayers) {
                SendTo(player, EMsgCS.S2C_StartGame, new InitServerFrame {
                    RoomID = RoomId,
                    Seed = seed,
                    ActorID = player.localId,
                    AllActors = ids,
                    SimulationSpeed = Define.SimulationSpeed
                });
            }
        }
    }
}