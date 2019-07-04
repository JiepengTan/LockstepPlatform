using System;
using System.Collections.Generic;
using Lockstep.Logging;
using Lockstep.Networking;
using Lockstep.Serialization;
using Lockstep.Util;
using NetMsg.Common;

namespace Lockstep.Client {
    public interface IRoomMsgManager {
        void Init(IRoomMsgHandler msgHandler);
        void SendInput(Msg_PlayerInput msg);
        void SendMissFrameReq(int missFrameTick);
        void SendMissFrameRepAck(int missFrameTick);
        void SendHashCodes(int firstHashTick, List<long> allHashCodes, int startIdx, int count);

        void SendGameEvent(byte[] data);
        void SendLoadingProgress(byte progress);


        void ConnectToGameServer(Msg_C2G_Hello helloBody, IPEndInfo _gameTcpEnd, bool isReconnect);
        void OnLevelLoadProgress(float progress);
    }

    public class RoomMsgManager : NetworkProxy, IRoomMsgManager {
        private delegate void DealNetMsg(BaseMsg data);

        private delegate BaseMsg ParseNetMsg(Deserializer reader);

        public EGameState CurGameState = EGameState.Idle;

        private NetClient<EMsgSC> _netUdp;
        private NetClient<EMsgSC> _netTcp;

        private float _curLoadProgress;
        private float _framePursueRate;

        public float FramePursueRate {
            get { return _framePursueRate; }
            set {
                _framePursueRate =System. Math.Max(System.Math.Min(1f, value), 0f);
            }
        }

        private float _nextSendLoadProgressTimer;
        private IRoomMsgHandler _handler;


        protected string _gameHash;
        protected int _curMapId;
        protected byte _localId;
        protected int _roomId;

        protected IPEndInfo _gameUdpEnd;
        protected IPEndInfo _gameTcpEnd;
        protected MessageHello helloBody;

        protected bool HasConnGameTcp;
        protected bool HasConnGameUdp;
        protected bool HasRecvGameDta;
        protected bool HasFinishedLoadLevel;

        public void Init(IRoomMsgHandler msgHandler){
            _maxMsgId = (byte) System.Math.Min((int) EMsgSC.EnumCount, (int) byte.MaxValue);
            _allMsgDealFuncs = new DealNetMsg[_maxMsgId];
            _allMsgParsers = new ParseNetMsg[_maxMsgId];
            Debug = new DebugInstance("Client " + ": ");
            RegisterMsgHandlers();
            _handler = msgHandler;
        }

        public override void DoUpdate(int deltaTimeMs){
            base.DoUpdate(deltaTimeMs);
            if (CurGameState == EGameState.Loading) {
                if (_nextSendLoadProgressTimer < LTime.timeSinceLevelLoad) {
                    SendLoadingProgress(CurProgress);
                }
            }
        }

        void ResetStatus(){
            HasConnGameTcp = false;
            HasConnGameUdp = false;
            HasRecvGameDta = false;
            HasFinishedLoadLevel = false;
        }

        public byte CurProgress {
            get {
                if (_curLoadProgress > 1) _curLoadProgress = 1;
                if (_curLoadProgress < 0) _curLoadProgress = 0;
                if (IsReconnecting) {
                    var val = (HasRecvGameDta ? 10 : 0) +
                              (HasConnGameUdp ? 10 : 0) +
                              (HasConnGameTcp ? 10 : 0) +
                              _curLoadProgress * 20 +
                              FramePursueRate * 50
                        ;
                    return (byte) val;
                }
                else {
                    var val = _curLoadProgress * 70 +
                              (HasRecvGameDta ? 10 : 0) +
                              (HasConnGameUdp ? 10 : 0) +
                              (HasConnGameTcp ? 10 : 0);

                    return (byte) val;
                }
            }
        }


        public void OnLevelLoadProgress(float progress){
            _curLoadProgress = progress;
            if (CurProgress >= 100) {
                CurGameState = EGameState.PartLoaded;
                _nextSendLoadProgressTimer += LTime.timeSinceLevelLoad + 0.5f;
                SendLoadingProgress(CurProgress);
            }
        }

        public bool IsReconnecting { get; set; }

        public void ConnectToGameServer(Msg_C2G_Hello helloBody, IPEndInfo _gameTcpEnd, bool isReconnect){
            Log("ConnectToGameServer  " + helloBody + " isReconnect=" + isReconnect);
            IsReconnecting = isReconnect;
            ResetStatus();
            this.helloBody = helloBody.Hello;
            InitNetClient(ref _netTcp, _gameTcpEnd.Ip, _gameTcpEnd.Port, () => {
                HasConnGameTcp = true;
                CurGameState = EGameState.Loading;
                _netTcp.SendMessage(EMsgSC.C2G_Hello, helloBody, (status, respond) => {
                        if (status != EResponseStatus.Failed) {
                            var rMsg = respond.Parse<Msg_G2C_Hello>();
                            _curMapId = rMsg.MapId;
                            _localId = rMsg.LocalId;
                            _gameUdpEnd = rMsg.UdpEnd;
                            _handler.OnTcpHello(rMsg);
                            ConnectUdp();
                        }
                        else {
                            _handler.OnGameStartFailed();
                        }
                    }
                );
            });
        }

        void ConnectUdp(){
            InitNetClient(ref _netUdp, _gameUdpEnd.Ip, _gameUdpEnd.Port, () => {
                HasConnGameUdp = true;
                Log("ConnectUdp succ");
                _netUdp.SendMessage(EMsgSC.C2G_UdpHello,
                    new Msg_C2G_UdpHello() {
                        Hello = helloBody
                    }
                );
                _handler.OnUdpHello(_curMapId, _localId);
            });
        }


        #region tcp

        public Msg_G2C_GameStartInfo GameStartInfo { get; private set; }


        protected void C2G_GameEvent(IIncommingMessage reader){
            var msg = reader.Parse<Msg_G2C_GameEvent>();
            _handler.OnGameEvent(msg.Data);
        }

        protected void G2C_GameStartInfo(IIncommingMessage reader){
            var msg = reader.Parse<Msg_G2C_GameStartInfo>();
            Log("G2C_GameStartInfo " + msg);
            HasRecvGameDta = true;
            GameStartInfo = msg;
            _handler.OnGameStartInfo(msg);
        }

        private short curLevel;

        protected void G2C_LoadingProgress(IIncommingMessage reader){
            var msg = reader.Parse<Msg_G2C_LoadingProgress>();
            _handler.OnLoadingProgress(msg.Progress);
        }

        protected void G2C_AllFinishedLoaded(IIncommingMessage reader){
            var msg = reader.Parse<Msg_G2C_AllFinishedLoaded>();
            curLevel = msg.Level;
            _handler.OnAllFinishedLoaded(msg.Level);
        }

        public void SendGameEvent(byte[] msg){
            SendTcp(EMsgSC.C2G_GameEvent, new Msg_C2G_GameEvent() {Data = msg});
        }

        public void SendLoadingProgress(byte progress){
            if (!IsReconnecting) {
                SendTcp(EMsgSC.C2G_LoadingProgress, new Msg_C2G_LoadingProgress() {
                    Progress = progress
                });
            }
        }

        #endregion

        #region udp

        private byte _maxMsgId = byte.MaxValue;
        private DealNetMsg[] _allMsgDealFuncs;
        private ParseNetMsg[] _allMsgParsers;


        private void RegisterMsgHandlers(){
            RegisterNetMsgHandler(EMsgSC.G2C_RepMissFrame, OnMsg_G2C_RepMissFrame, ParseData<Msg_RepMissFrame>);
            RegisterNetMsgHandler(EMsgSC.G2C_FrameData, OnMsg_G2C_FrameData, ParseData<Msg_ServerFrames>);
        }

        private void RegisterNetMsgHandler(EMsgSC type, DealNetMsg func, ParseNetMsg parseFunc){
            _allMsgDealFuncs[(int) type] = func;
            _allMsgParsers[(int) type] = parseFunc;
        }

        private T ParseData<T>(Deserializer reader) where T : BaseMsg, new(){
            return reader.Parse<T>();
        }

        public void SendInput(Msg_PlayerInput msg){
            SendUdp(EMsgSC.C2G_PlayerInput, msg);
        }

        public void SendMissFrameReq(int missFrameTick){
            SendUdp(EMsgSC.C2G_ReqMissFrame, new Msg_ReqMissFrame() {StartTick = missFrameTick});
        }

        public void SendMissFrameRepAck(int missFrameTick){
            SendUdp(EMsgSC.C2G_RepMissFrameAck, new Msg_RepMissFrameAck() {MissFrameTick = missFrameTick});
        }

        public void SendHashCodes(int firstHashTick, List<long> allHashCodes, int startIdx, int count){
            Msg_HashCode msg = new Msg_HashCode();
            msg.StartTick = firstHashTick;
            msg.HashCodes = new long[count];
            for (int i = startIdx; i < count; i++) {
                msg.HashCodes[i] = allHashCodes[i];
            }

            SendUdp(EMsgSC.C2G_HashCode, msg);
        }


        public void SendUdp(EMsgSC msgId, ISerializable body){
            var writer = new Serializer();
            writer.Write((short) msgId);
            body?.Serialize(writer);
            _netUdp?.SendMessage(EMsgSC.C2G_UdpMessage, writer.CopyData(), EDeliveryMethod.Unreliable);
        }

        public void SendTcp(EMsgSC msgId, BaseMsg body){
            var writer = new Serializer();
            writer.Write((short) msgId);
            body.Serialize(writer);
            _netTcp?.SendMessage(EMsgSC.C2G_TcpMessage, writer.CopyData());
        }

        protected void G2C_UdpMessage(IIncommingMessage reader){
            var bytes = reader.GetRawBytes();
            var data = new Deserializer(Compressor.Decompress(bytes));
            OnRecvMsg(data);
        }

        protected void OnRecvMsg(Deserializer reader){
            var msgType = reader.ReadInt16();
            if (msgType >= _maxMsgId) {
                Debug.LogError($" send a Error msgType out of range {msgType}");
                return;
            }

            try {
                var _func = _allMsgDealFuncs[msgType];
                var _parser = _allMsgParsers[msgType];
                if (_func != null && _parser != null) {
                    _func(_parser(reader));
                }
                else {
                    Debug.LogError($" ErrorMsg type :no msg handler or parser {msgType}");
                }
            }
            catch (Exception e) {
                Debug.LogError($" Deal Msg Error :{(EMsgSC) (msgType)}  " + e);
            }
        }

        protected void OnMsg_G2C_FrameData(BaseMsg reader){
            var msg = reader as Msg_ServerFrames;
            _handler.OnServerFrames(msg);
        }

        protected void OnMsg_G2C_RepMissFrame(BaseMsg reader){
            var msg = reader as Msg_RepMissFrame;
            _handler.OnMissFrames(msg);
        }

        #endregion
    }
}