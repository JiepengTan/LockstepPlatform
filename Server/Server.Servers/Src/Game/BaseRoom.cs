using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteNetLib;
using Lockstep.Logging;
using Lockstep.Networking;
using Lockstep.Serialization;
using NetMsg.Common;
using Lockstep.Server.Common;
using Lockstep.Util;
using Random = System.Random;

namespace Lockstep.Server.Game {
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


    public interface IRoom : IRecyclable {
        Msg_G2C_GameStartInfo GameStartInfo { get; set; }
        int GameType { get; }
        int RoomId { get; }
        int CurPlayerCount { get; }

        int MaxPlayerCount { get; }

        //room life cycle
        void DoStart(IGameServer server, int roomId, int gameType, int mapId, GamePlayerInfo[] playerInfos,
            string gameHash);

        void DoUpdate(int deltaTime);
        void DoDestroy();

        //net status
        void OnPlayerConnect(Player player);
        void OnPlayerReconnect(Player player);
        void OnPlayerDisconnect(Player player);
        void OnPlayerLeave(Player player);

        //game status
        void StartGame();

        void FinishedGame();

        //net msg
        void SendUdp(Player player, byte[] data);
        void BorderUdp(byte[] data);
        void BorderTcp(EMsgSC type, BaseFormater info);
        void SetStartInfo(Msg_G2C_GameStartInfo info);
        void OnRecvMsg(Player player, Deserializer reader);
    }

    public class BaseRoom : BaseLogger, IRoom, IRecyclable {
        public int MapId { get; set; }
        public string GameHash { get; set; }

        public IPEndInfo TcpEnd { get; set; }
        public IPEndInfo UdpEnd { get; set; }

        public EGameState State = EGameState.Idle;
        public int GameType { get; set; }
        public int RoomId { get; set; }

        public int CurPlayerCount {
            get {
                int count = 0;
                foreach (var player in Players) {
                    if (player != null) {
                        count++;
                    }
                }

                return count;
            }
        }

        public int MaxPlayerCount { get; set; }

        public bool IsRunning { get; private set; }
        public string Name;
        public float TimeSinceCreate;
        public bool IsFinished = false;

        public Msg_G2C_GameStartInfo GameStartInfo { get; set; }


        private Dictionary<long, byte> _userId2LocalId = new Dictionary<long, byte>();
        public Player[] Players { get; private set; }


        //hashcode 
        public int Tick = 0;
        private Dictionary<int, HashCodeMatcher> _hashCodes = new Dictionary<int, HashCodeMatcher>();


        private IGameServer _gameServer;

        private const int MaxMsgIdx = (short) EMsgSC.EnumCount;
        private DealNetMsg[] allMsgDealFuncs = new DealNetMsg[MaxMsgIdx];
        private ParseNetMsg[] allMsgParsers = new ParseNetMsg[MaxMsgIdx];

        private delegate void DealNetMsg(Player player, BaseFormater data);

        private delegate BaseFormater ParseNetMsg(Deserializer reader);

        public long timeSinceLoaded;
        private long firstFrameTimeStamp = 0;
        private int waitTimer = 0;

        //所有需要等待输入到来的Ids
        public List<byte> allNeedWaitInputPlayerIds;
        private List<ServerFrame> allHistoryFrames = new List<ServerFrame>(); //所有的历史帧

        public int Seed { get; set; }

        public void OnRecvPlayerGameData(Player player){
            if (player == null || MaxPlayerCount <= player.LocalId || Players[player.LocalId] != player) {
                return;
            }

            bool hasRecvAll = true;
            foreach (var user in Players) {
                if (user != null && user.GameData == null) {
                    hasRecvAll = false;
                    break;
                }
            }

            var playerCount = MaxPlayerCount;
            if (hasRecvAll) {
                var userInfos = new GameData[playerCount];
                for (int i = 0; i < playerCount; i++) {
                    userInfos[i] = Players[i]?.GameData;
                }

                //all user data ready notify game start
                SetStartInfo(new Msg_G2C_GameStartInfo() {
                    MapId = MapId,
                    RoomId = RoomId,
                    Seed = Seed,
                    UserCount = MaxPlayerCount,
                    TcpEnd = TcpEnd,
                    UdpEnd = UdpEnd,
                    SimulationSpeed = 60,
                    UserInfos = userInfos
                });
            }
        }

   
        public int GetUserLocalId(long userId){
            if (_userId2LocalId.TryGetValue(userId, out var id)) {
                return id;
            }

            return -1;
        }


        #region  life cycle

        public void DoStart(IGameServer server, int roomId, int gameType, int mapId, GamePlayerInfo[] playerInfos,
            string gameHash){
            State = EGameState.Loading;
            _gameServer = server;
            Seed = LRandom.Range(1, 100000);
            Tick = 0;
            timeSinceLoaded = 0;
            firstFrameTimeStamp = 0;
            RegisterMsgHandlers();
            Debug = new DebugInstance("Room" + RoomId + ": ");
            var count = playerInfos.Length;
            GameType = gameType;
            GameHash = gameHash;
            RoomId = roomId;
            MaxPlayerCount = count;
            Name = roomId.ToString();
            MapId = mapId;
            Players = new Player[count];
            for (int i = 0; i < count; i++) {
                var user = playerInfos[i];
                var player = Pool.Get<Player>();
                player.UserId = user.UserId;
                player.Account = user.Account;
                player.LoginHash = user.LoginHash;
                player.LocalId = (byte) i;
                player.Room = this;
                Players[i] = player;
            }

            _userId2LocalId.Clear();
            TimeSinceCreate = Time.timeSinceLevelLoad;
            for (byte i = 0; i < count; i++) {
                var player = Players[i];
                _userId2LocalId.Add(player.UserId, player.LocalId);
            }
        }

        public void DoUpdate(int deltaTime){
            timeSinceLoaded += deltaTime;
            BorderServerFrame(deltaTime);
        }

        public void DoDestroy(){
            Log($"Room {RoomId} Destroy");
            DumpGameFrames();
        }

        private void BorderServerFrame(int deltaTime){
            waitTimer += deltaTime;
            if (State != EGameState.Playing) return;
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
                        if (inputs[i] == null ) {
                            if (Players[i] != null) { 
                                Log($"Overtime wait remove localId = {i}");
                            }
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
                BorderUdp(EMsgSC.G2C_FrameData, msg);
                if (firstFrameTimeStamp <= 0) {
                    firstFrameTimeStamp = timeSinceLoaded;
                }

                Tick++;
            }
        }

        private void DumpGameFrames(){
            var msg = new Msg_RepMissFrame();
            int count = Math.Min((Tick - 1), allHistoryFrames.Count);
            if (count <= 0) return;
            var writer = new Serializer();
            GameStartInfo.Serialize(writer);
            var frames = new ServerFrame[count];
            for (int i = 0; i < count; i++) {
                frames[i] = allHistoryFrames[i];
                Logging.Debug.Assert(frames[i] != null, "!!!!!!!!!!!!!!!!!");
            }

            msg.startTick = frames[0].tick;
            msg.frames = frames;
            msg.Serialize(writer);
            var bytes = Compressor.Compress(writer);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "../Record/" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + GameType + "_" + RoomId +
                ".record");
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }

            Log("Create Record " + path);
            File.WriteAllBytes(path, bytes);
        }

        #endregion

        #region net msg

        public void SetStartInfo(Msg_G2C_GameStartInfo info){
            GameStartInfo = info;
            BorderTcp(EMsgSC.G2C_GameStartInfo, GameStartInfo);
        }

        public void OnRecvMsg(Player player, Deserializer reader){
            if (reader.IsEnd) {
                DealMsgHandlerError(player, $"{player.UserId} send a Error:Net Msg");
                return;
            }

            var msgType = reader.GetInt16();
            if (msgType >= MaxMsgIdx) {
                DealMsgHandlerError(player, $"{player.UserId} send a Error msgType out of range {msgType}");
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
                            $"ErrorMsg type :parser data error playerID = {player.UserId} msgType:{msgType}");
                        return;
                    }

                    _func(player, data);
                }
                else {
                    DealMsgHandlerError(player,
                        $" {player.UserId} ErrorMsg type :no msg handler or parser {msgType}");
                }
            }
        }

        void DealMsgHandlerError(Player player, string msg){
            LogError(msg);
            TickOut(player, 0);
        }

        public void TickOut(Player player, int reason){
            _gameServer.TickOut(player, reason);
        }

        private void RegisterMsgHandlers(){
            RegisterNetMsgHandler(EMsgSC.C2G_PlayerInput, OnNet_PlayerInput,
                (reader) => { return ParseData<Msg_PlayerInput>(reader); });
            RegisterNetMsgHandler(EMsgSC.C2G_HashCode, OnNet_HashCode,
                (reader) => { return ParseData<Msg_HashCode>(reader); });
            RegisterNetMsgHandler(EMsgSC.C2G_LoadingProgress, OnNet_LoadingProgress,
                (reader) => { return ParseData<Msg_C2G_LoadingProgress>(reader); });
            RegisterNetMsgHandler(EMsgSC.C2G_ReqMissFrame, OnNet_ReqMissFrame,
                (reader) => { return ParseData<Msg_ReqMissFrame>(reader); });
            RegisterNetMsgHandler(EMsgSC.C2G_RepMissFrameAck, OnNet_RepMissFrameAck,
                (reader) => { return ParseData<Msg_RepMissFrameAck>(reader); });
        }

        private void RegisterNetMsgHandler(EMsgSC type, DealNetMsg func, ParseNetMsg parseFunc){
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
                LogError("Parse Msg Error:" + e);
                data = null;
            }

            return data;
        }

        public void BorderTcp(EMsgSC type, BaseFormater data){
            var msg = MessageHelper.Create((short) type, data);
            foreach (var player in Players) {
                player?.SendTcp(msg);
            }
        }

        public void BorderUdp(byte[] data){
            foreach (var player in Players) {
                SendUdp(player, data);
            }
        }

        public void BorderUdp(EMsgSC type, ISerializable body){
            var writer = new Serializer();
            writer.PutInt16((short) type);
            body.Serialize(writer);
            var bytes = Compressor.Compress(writer);
            foreach (var player in Players) {
                player?.SendUdp(bytes);
            }
        }


        public void SendUdp(Player player, byte[] data){
            player?.SendUdp(data);
        }

        public void SendUdp(Player player, EMsgSC type, ISerializable body, bool isNeedDebugSize = false){
            var writer = new Serializer();
            writer.PutInt16((short) type);
            body.Serialize(writer);
            var bytes = Compressor.Compress(writer);
            if (isNeedDebugSize) {
                Log($"msg :type {type} size{bytes.Length}");
            }

            player?.SendUdp(bytes);
        }

        #endregion

        #region net status

        public void OnPlayerConnect(Player player){
            if (GameStartInfo != null) {
                player.SendTcp(EMsgSC.G2C_GameStartInfo, GameStartInfo);
            }
        }

        //net status
        public void OnPlayerReconnect(Player player){
            player.LocalId = _userId2LocalId[player.UserId];
            Players[player.LocalId] = player;
        }

        public void OnPlayerDisconnect(Player player){
            Log($"Player{player.UserId} OnDisconnect room {RoomId}");
            RemovePlayer(player);
        }
        public void OnPlayerLeave(long userId){
            if (_userId2LocalId.TryGetValue(userId, out var localId)) {
                var player = Players[localId];
                if (player != null) {
                    OnPlayerLeave(player);
                }
            }
        }
        public void OnPlayerLeave(Player player){
            RemovePlayer(player);
            _userId2LocalId.Remove(player.UserId); //同时还需要彻底的移除记录 避免玩家重连
            Log($"Player{player.UserId} OnPlayerLeave room {RoomId}");
        }

        void RemovePlayer(Player player){
            if (Players[player.LocalId] == null) return;
            Players[player.LocalId] = null;
            player.PeerTcp.CleanExtension();
            player.PeerTcp.Disconnect("");
            player.PeerTcp = null;
            player.PeerUdp.CleanExtension();
            player.PeerUdp.Disconnect("");
            player.PeerUdp = null;
            
            var curCount = CurPlayerCount;
            if (curCount == 0) {
                Log("All players left, stopping current simulation...");
                IsRunning = false;
                State = EGameState.Idle;
                _gameServer.OnGameEmpty(this);
            }
            else {
                Log(curCount + " players remaining.");
            }
        }

        #endregion

        #region game status

        public void StartGame(){ }

        public void FinishedGame(){ }

        #endregion

        #region IRecyclable

        //回收时候调用
        public void OnReuse(){ }

        public void OnRecycle(){
            _userId2LocalId.Clear();
            _hashCodes.Clear();
            Tick = 0;
            RoomId = -1;
            if (Players == null) return;
            foreach (var player in Players) {
                Pool.Return(player);
            }

            Players = null;
        }

        #endregion

        #region Net msg handler

        void OnNet_PlayerInput(Player player, BaseFormater data){
            if (State != EGameState.PartLoaded && State != EGameState.Playing) return;
            if (State == EGameState.PartLoaded) {
                Log("First input: game start playing");
                State = EGameState.Playing;
            }

            var input = data as Msg_PlayerInput;
            //Debug.Log($"RecvInput actorID:{input.ActorId} inputTick:{input.Tick} Tick{Tick}");
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
            var id = player.LocalId;
            for (int i = 0; i < hashInfo.HashCodes.Length; i++) {
                var code = hashInfo.HashCodes[i];
                var tick = hashInfo.StartTick + i;
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
                Log($"!!!!!!!!!!!! Hash not match tick{tick} hash{hash} ");
            }
        }

        private byte[] playerLoadingProgress;

        public const int MaxRepMissFrameCountPerPack = 600;

        void OnNet_ReqMissFrame(Player player, BaseFormater data){
            var reqMsg = data as Msg_ReqMissFrame;
            var nextCheckTick = reqMsg.StartTick;
            Log($"OnNet_ReqMissFrame nextCheckTick id:{player.LocalId}:{nextCheckTick}");
            var msg = new Msg_RepMissFrame();
            int count = Math.Min(Math.Min((Tick - 1), allHistoryFrames.Count) - nextCheckTick,
                MaxRepMissFrameCountPerPack);
            if (count <= 0) return;
            var frames = new ServerFrame[count];
            for (int i = 0; i < count; i++) {
                frames[i] = allHistoryFrames[nextCheckTick + i];
                Logging.Debug.Assert(frames[i] != null);
            }

            msg.startTick = frames[0].tick;
            msg.frames = frames;
            SendUdp(player, EMsgSC.G2C_RepMissFrame, msg, true);
        }

        void OnNet_RepMissFrameAck(Player player, BaseFormater data){
            var msg = data as Msg_RepMissFrameAck;
            Log($"OnNet_RepMissFrameAck missFrameTick:{msg.MissFrameTick}");
        }


        void OnNet_LoadingProgress(Player player, BaseFormater data){
            if (State != EGameState.Loading) return;
            var msg = data as Msg_C2G_LoadingProgress;
            if (playerLoadingProgress == null) {
                playerLoadingProgress = new byte[MaxPlayerCount];
            }

            playerLoadingProgress[player.LocalId] = msg.Progress;

            //Log($"palyer{player.LocalId} Load {msg.Progress}");

            BorderTcp(EMsgSC.G2C_LoadingProgress, new Msg_G2C_LoadingProgress() {
                Progress = playerLoadingProgress
            });
            
            if (msg.Progress < 100) return;
            var isDone = true;
            foreach (var progress in playerLoadingProgress) {
                if (progress < 100) {
                    isDone = false;
                    break;
                }
            }

            if (isDone) {
                OnFinishedLoaded();
            }
        }


        void OnFinishedLoaded(){
            Log("All Load done");
            State = EGameState.PartLoaded;
            for (int i = 0; i < playerLoadingProgress.Length; i++) {
                playerLoadingProgress[i] = 0;
            }

            allNeedWaitInputPlayerIds = new List<byte>();
            foreach (var val in _userId2LocalId.Values) {
                allNeedWaitInputPlayerIds.Add(val);
            }

            //BorderTcp(EMsgSC.G2C_GameStartInfo, GameStartInfo);
            BorderTcp(EMsgSC.G2C_AllFinishedLoaded, new Msg_G2C_AllFinishedLoaded() { });
        }

        #endregion
    }
}