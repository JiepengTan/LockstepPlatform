using System;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib;
using Lockstep.Serialization;
using NetMsg.Game;
using NetMsg.Lobby;
using Server.Common;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Logic.Server {
    public class HashCodeMatcher {
        public long hashCode;
        public bool[] sendResult;
        public int count;

        public HashCodeMatcher(int num){
            hashCode = 0;
            sendResult = new bool[num];
            count = 0;
        }

        public bool IsMatchered => count == sendResult.Length;
    }

    public enum ERoomState {
        Idle,
        WaitingToPlay,
        PartLoading,
        PartLoaded,
        Playing,
        PartFinished,
        FinishAll,
    }


    public class Room : IRoom {
        public ERoomState State = ERoomState.Idle;
        public int TypeId { get; protected set; }
        public int RoomId { get; protected set; }

        public int CurPlayerCount {
            get { return _allPlayers.Count; }
        }

        public int MaxPlayerCount { get; private set; }

        public bool IsRunning { get; private set; }
        public string name;
        public int Tick = 0;

        private readonly Dictionary<int, byte> netId2LocalId = new Dictionary<int, byte>();
        private readonly Dictionary<byte, Player> _localId2Player = new Dictionary<byte, Player>();
        private readonly List<Player> _allPlayers = new List<Player>();

        public List<ServerFrame> _historyFrames = new List<ServerFrame>();
        private Dictionary<ulong, long> _hashCodes = new Dictionary<ulong, long>();

        private ILobby _lobby;
        private byte curPlayerLocalId = 0;

        private DealNetMsg[] allMsgDealFuncs = new DealNetMsg[(int) EMsgCS.EnumCount];
        private ParseNetMsg[] allMsgParsers = new ParseNetMsg[(int) EMsgCS.EnumCount];

        //hashcode 
        private Dictionary<int, HashCodeMatcher> allHashCodes = new Dictionary<int, HashCodeMatcher>();

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
        private int MaxWaitTime = 300; //最多等待事时间
        private int waitTimer = 0;

        //所有需要等待输入到来的Ids
        public List<byte> allNeedWaitInputPlayerIds;
        List<ServerFrame> allHistoryFrames = new List<ServerFrame>(); //所有的历史帧

        private void BorderServerFrame(int deltaTime){
            waitTimer += deltaTime;
            if (State != ERoomState.Playing) return;
            while (true) { // 如果落后太多 就一直追帧
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
                            Debug.Log($"Overtime wait remove localId = {i}");
                            allNeedWaitInputPlayerIds.Remove((byte) i);
                        }
                    }
                }

                //是否所有的输入  都已经等到
                foreach (var id in allNeedWaitInputPlayerIds) {
                    if (inputs[id] == null) {
                        return;
                    }
                }

                //将所有未到的包 给予默认的输入
                for (int i = 0; i < inputs.Length; i++) {
                    if (inputs[i] == null) {
                        inputs[i] = new Msg_PlayerInput(Tick, (byte) i, null);
                    }
                }

                //Debug.Log("Border input " + Tick);
                var msg = new Msg_ServerFrames();
                int count = Tick < 2 ? iTick + 1 : 3;
                var frames = new ServerFrame[count];
                for (int i = 0; i < count; i++) {
                    frames[count - i - 1] = allHistoryFrames[iTick - i];
                }

                msg.frames = frames;
                SendToAll(EMsgCS.S2C_FrameData, msg);
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
                (reader) => { return ParseData<Msg_PlayerInput>(reader); });
            RegisterNetMsgHandler(EMsgCS.C2S_ReqMissFrame, OnNet_ReqMissPack, null);
            RegisterNetMsgHandler(EMsgCS.C2S_HashCode, OnNet_HashCode,
                (reader) => { return ParseData<Msg_HashCode>(reader); });
            RegisterNetMsgHandler(EMsgCS.C2S_PlayerReady, OnNet_PlayerReady,
                (reader) => { return ParseData<Msg_PlayerReady>(reader); });
            RegisterNetMsgHandler(EMsgCS.C2S_LoadingProgress, OnNet_LoadingProgress,
                (reader) => { return ParseData<Msg_LoadingProgress>(reader); });
            RegisterNetMsgHandler(EMsgCS.C2S_ReqMissFrame, OnNet_ReqMissFrame,
                (reader) => { return ParseData<Msg_ReqMissFrame>(reader); });
            RegisterNetMsgHandler(EMsgCS.C2S_RepMissFrameAck, OnNet_RepMissFrameAck,
                (reader) => { return ParseData<Msg_RepMissFrameAck>(reader); });
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
            if (State == ERoomState.Idle) {
                State = ERoomState.WaitingToPlay;
            }

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
                State = ERoomState.Idle;
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

            State = ERoomState.PartLoading;
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
            if (State != ERoomState.PartLoaded && State != ERoomState.Playing) return;
            if (State == ERoomState.PartLoaded) {
                State = ERoomState.Playing;
            }

            var input = data as Msg_PlayerInput;
            //Debug.Log($"RecvInput actorID:{input.ActorId} inputTick:{input.Tick} Tick{Tick} count = {input.Commands.Count}");
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
                frame.inputs = new Msg_PlayerInput[MaxPlayerCount];
            }

            var id = input.ActorId;
            if (!allNeedWaitInputPlayerIds.Contains(id)) {
                allNeedWaitInputPlayerIds.Add(id);
            }

            frame.inputs[id] = input;
            //if (input.Commands.Count > 0) {
            //    Debug.Log($"RecvInput actorID:{input.ActorId}  inputTick:{input.Tick}  cmd:{(ECmdType)(input.Commands[0].type)}");
            //}
        }

        void OnNet_ReqMissPack(Player player, BaseFormater data){ }


        void OnNet_HashCode(Player player, BaseFormater data){
            var hashInfo = data as Msg_HashCode;
            var id = player.localId;
            for (int i = 0; i < hashInfo.hashCodes.Length; i++) {
                var code = hashInfo.hashCodes[i];
                var tick = hashInfo.startTick + i;
                if (allHashCodes.TryGetValue(tick, out HashCodeMatcher matcher1)) {
                    if (matcher1 == null || matcher1.sendResult[id]) {
                        continue;
                    }

                    if (matcher1.hashCode != code) {
                        OnHashMatchResult(tick, code, false);
                    }

                    matcher1.count = matcher1.count + 1;
                    matcher1.sendResult[id] = true;
                    if (matcher1.IsMatchered) {
                        OnHashMatchResult(tick, code, true);
                    }
                }
                else {
                    var matcher2 = new HashCodeMatcher(MaxPlayerCount);
                    matcher2.count = 1;
                    matcher2.hashCode = code;
                    matcher2.sendResult[id] = true;
                    allHashCodes.Add(tick, matcher2);
                    if (matcher2.IsMatchered) {
                        OnHashMatchResult(tick, code, true);
                    }
                }
            }
        }

        void OnHashMatchResult(int tick, long hash, bool isMatched){
            if (isMatched) {
                allHashCodes[tick] = null;
            }

            if (!isMatched) {
                Debug.Log($"!!!!!!!!!!!! Hash not match tick{tick} hash{hash} ");
            }
        }

        void OnNet_PlayerReady(Player player, BaseFormater data){
            OnPlayerReady(player);
        }

        private byte[] playerLoadingProgress;

        public const int MaxRepMissFrameCountPerPack = 200;
        void OnNet_ReqMissFrame(Player player, BaseFormater data){
            var reqMsg = data as Msg_ReqMissFrame;
            var nextCheckTick = (int) reqMsg.missFrames[0];
            Debug.Log($"OnNet_ReqMissFrame nextCheckTick :{nextCheckTick}");
            var msg = new Msg_RepMissFrame();;
            int count = Math.Min(allHistoryFrames.Count - nextCheckTick, MaxRepMissFrameCountPerPack);
            var frames = new ServerFrame[count];
            for (int i = nextCheckTick; i < count; i++) {
                frames[i] = allHistoryFrames[nextCheckTick + i];
            }
            msg.frames = frames;
            SendTo(player,EMsgCS.S2C_RepMissFrame, msg);
        }

        void OnNet_RepMissFrameAck(Player player, BaseFormater data){
            var msg = data as Msg_RepMissFrameAck;
            Debug.Log($"OnNet_RepMissFrameAck missFrameTick:{msg.missFrameTick}");
        }


        void OnNet_LoadingProgress(Player player, BaseFormater data){
            if (State != ERoomState.PartLoading) return;
            var msg = data as Msg_LoadingProgress;
            if (playerLoadingProgress == null) {
                playerLoadingProgress = new byte[MaxPlayerCount];
            }

            playerLoadingProgress[player.localId] = msg.progress;

            Debug.Log($"palyer{player.localId} Load {msg.progress}");
            var isDone = true;
            foreach (var progress in playerLoadingProgress) {
                if (progress < 100) {
                    isDone = false;
                    break;
                }
            }

            var retmsg = new Msg_AllLoadingProgress();
            retmsg.isAllDone = isDone;
            retmsg.progress = playerLoadingProgress;
            SendToAll(EMsgCS.S2C_LoadingProgress, retmsg);
            if (isDone) {
                for (int i = 0; i < playerLoadingProgress.Length; i++) {
                    playerLoadingProgress[i] = 0;
                }

                State = ERoomState.PartLoaded;
                Debug.Log("All Load done");
            }
        }

        #endregion

        private void StartSimulationOnConnectedPeers(){
            var seed = new Random().Next(int.MinValue, int.MaxValue);
            var ids = netId2LocalId.Values.ToArray();
            foreach (var player in _allPlayers) {
                SendTo(player, EMsgCS.S2C_StartGame, new Msg_StartGame {
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