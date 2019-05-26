using System;
using System.Collections.Generic;
using System.IO;
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

        private byte _curLocalId = 0;
        private Dictionary<long, byte> _playerId2LocalId = new Dictionary<long, byte>();
        private Dictionary<byte, Player> _localId2Player = new Dictionary<byte, Player>();
        private List<Player> _allPlayers = new List<Player>();

        //hashcode 
        public int Tick = 0;
        private Dictionary<int, HashCodeMatcher> _hashCodes = new Dictionary<int, HashCodeMatcher>();


        private ILobby _lobby;

        private DealNetMsg[] allMsgDealFuncs = new DealNetMsg[(int) EMsgCS.EnumCount];
        private ParseNetMsg[] allMsgParsers = new ParseNetMsg[(int) EMsgCS.EnumCount];

        private delegate void DealNetMsg(Player player, BaseFormater data);

        private delegate BaseFormater ParseNetMsg(Deserializer reader);

        public long timeSinceLoaded;
        private long firstFrameTimeStamp = 0;
        private int waitTimer = 0;
        private int seed;

        //所有需要等待输入到来的Ids
        public List<byte> allNeedWaitInputPlayerIds;
        List<ServerFrame> allHistoryFrames = new List<ServerFrame>(); //所有的历史帧


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
            RegisterMsgHandlers();
        }

        public void DoUpdate(int deltaTime){
            timeSinceLoaded += deltaTime;
            BorderServerFrame(deltaTime);
        }


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

                var inputs = frame.Inputs;
                //超时等待 移除超时玩家
                if (waitTimer > NetworkDefine.MAX_DELAY_TIME_MS) {
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

                msg.startTick = frames[0].tick;
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
            DumpGameFrames();
        }

        private void DumpGameFrames(){
            var msg = new Msg_RepMissFrame();
            int count = Math.Min((Tick - 1), allHistoryFrames.Count);
            if (count <= 0) return;
            var frames = new ServerFrame[count];
            for (int i = 0; i < count; i++) {
                frames[i] = allHistoryFrames[i];
                Debug.Assert(frames[i] != null, "!!!!!!!!!!!!!!!!!");
            }

            msg.startTick = frames[0].tick;
            msg.frames = frames;
            var writer = new Serializer();
            writer.Put(TypeId);
            writer.Put(RoomId);
            writer.Put(seed);
            writer.PutBytes_255(_playerId2LocalId.Values.ToArray());

            msg.Serialize(writer);
            var bytes = Compressor.Compress(writer);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "../../Record/" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "Type:" + TypeId + "Room:" + RoomId +
                ".bytes");
            Debug.Log("Create Record " + path);
            File.WriteAllBytes(path, bytes);
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

        public void SendTo(Player player, EMsgCS type, ISerializable body, bool isNeedDebugSize = false){
            var writer = new Serializer();
            writer.Put((byte) type);
            body.Serialize(writer);
            var bytes = Compressor.Compress(writer);
            if (isNeedDebugSize) {
                Debug.Log($"msg :type {type} size{bytes.Length}");
            }

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

        public long[] GetAllPlayerIDs(){
            return _playerId2LocalId.Keys.ToArray();
        }

        public byte[] GetReconnectMsg(Player player){
            var ids = _playerId2LocalId.Values.ToArray();
            var body = CreateStartGame(player.localId, ids);
            var writer = new Serializer();
            body.Serialize(writer);
            return writer.CopyData();
        }

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
            var localId = _curLocalId++;
            player.localId = localId;
            _playerId2LocalId[player.PlayerId] = localId;
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
                Debug.Log("Room is full, starting new simulation...");
                StartGame();
            }
            else {
                Debug.Log(readyCount + " / " + MaxPlayerCount + " players have ready.");
            }
        }

        public void OnPlayerLeave(Player player){
            RemovePlayer(player);
            _playerId2LocalId.Remove(player.PlayerId); //同时还需要彻底的移除记录 避免玩家重连
            Debug.Log($"Player{player.PlayerId} OnPlayerLeave room {RoomId}");
        }

        #endregion

        #region net status

        //net status
        public void OnReconnect(Player player){
            player.localId = _playerId2LocalId[player.PlayerId];
            _localId2Player[player.localId] = player;
            if (_allPlayers.Contains(player)) {
                Debug.LogError("Duplicate add players" + player.PlayerId);
                return;
            }

            _allPlayers.Add(player);
        }

        public void OnDisconnect(Player player){
            Debug.Log($"Player{player.PlayerId} OnDisconnect room {RoomId}");
            RemovePlayer(player);
        }

        void RemovePlayer(Player player){
            _allPlayers.Remove(player);
            _localId2Player.Remove(player.localId);
            if (_localId2Player.Count == 0) {
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

        #region game status

        public bool CanStartGame(){
            return true;
        }

        public void StartGame(){
            allNeedWaitInputPlayerIds = new List<byte>();
            foreach (var val in _playerId2LocalId.Values) {
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

        public void OnRecycle(){
            _playerId2LocalId.Clear();
            _localId2Player.Clear();
            _allPlayers.Clear();
            _hashCodes.Clear();
            _curLocalId = 0;
            Tick = 0;
        }

        #endregion

        #region Net msg handler

        void OnNet_PlayerInput(Player player, BaseFormater data){
            if (State != ERoomState.PartLoaded && State != ERoomState.Playing) return;
            if (State == ERoomState.PartLoaded) {
                State = ERoomState.Playing;
            }

            var input = data as Msg_PlayerInput;
            //Debug.Log($"RecvInput actorID:{input.ActorId} inputTick:{input.Tick} Tick{Tick} count = {input.Commands.Length}");
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
            if (frame.Inputs == null || frame.Inputs.Length != MaxPlayerCount) {
                frame.Inputs = new Msg_PlayerInput[MaxPlayerCount];
            }

            var id = input.ActorId;
            if (!allNeedWaitInputPlayerIds.Contains(id)) {
                allNeedWaitInputPlayerIds.Add(id);
            }

            frame.Inputs[id] = input;
            //if (input.Commands.Count > 0) {
            //    Debug.Log($"RecvInput actorID:{input.ActorId}  inputTick:{input.Tick}  cmd:{(ECmdType)(input.Commands[0].type)}");
            //}
        }


        void OnNet_HashCode(Player player, BaseFormater data){
            var hashInfo = data as Msg_HashCode;
            var id = player.localId;
            for (int i = 0; i < hashInfo.hashCodes.Length; i++) {
                var code = hashInfo.hashCodes[i];
                var tick = hashInfo.startTick + i;
                if (_hashCodes.TryGetValue(tick, out HashCodeMatcher matcher1)) {
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
                    _hashCodes.Add(tick, matcher2);
                    if (matcher2.IsMatchered) {
                        OnHashMatchResult(tick, code, true);
                    }
                }
            }
        }

        void OnHashMatchResult(int tick, long hash, bool isMatched){
            if (isMatched) {
                _hashCodes[tick] = null;
            }

            if (!isMatched) {
                Debug.Log($"!!!!!!!!!!!! Hash not match tick{tick} hash{hash} ");
            }
        }

        void OnNet_PlayerReady(Player player, BaseFormater data){
            OnPlayerReady(player);
        }

        private byte[] playerLoadingProgress;

        public const int MaxRepMissFrameCountPerPack = 600;

        void OnNet_ReqMissFrame(Player player, BaseFormater data){
            var reqMsg = data as Msg_ReqMissFrame;
            var nextCheckTick = reqMsg.startTick;
            Debug.Log($"OnNet_ReqMissFrame nextCheckTick id:{player.localId}:{nextCheckTick}");
            var msg = new Msg_RepMissFrame();
            int count = Math.Min(Math.Min((Tick - 1), allHistoryFrames.Count) - nextCheckTick,
                MaxRepMissFrameCountPerPack);
            if (count <= 0) return;
            var frames = new ServerFrame[count];
            for (int i = 0; i < count; i++) {
                frames[i] = allHistoryFrames[nextCheckTick + i];
                Debug.Assert(frames[i] != null);
            }

            msg.startTick = frames[0].tick;
            msg.frames = frames;
            SendTo(player, EMsgCS.S2C_RepMissFrame, msg, true);
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
            seed = new Random().Next(int.MinValue, int.MaxValue);
            var ids = _playerId2LocalId.Values.ToArray();
            foreach (var player in _allPlayers) {
                SendTo(player, EMsgCS.S2C_StartGame, CreateStartGame(player.localId, ids));
            }
        }

        Msg_StartGame CreateStartGame(byte playerId, byte[] ids){
            return new Msg_StartGame {
                RoomID = RoomId,
                Seed = seed,
                ActorID = playerId,
                AllActors = ids,
                SimulationSpeed = Define.SimulationSpeed
            };
        }
    }
}