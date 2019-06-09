using System;
using System.Collections.Generic;
using Lockstep.Logging;
using Lockstep.Networking;
using Lockstep.Serialization;
using Lockstep.Util;
using NetMsg.Common;

namespace Lockstep.Client {
    public interface IRoomMsgManager {
        void Init(BaseRoomMsgHandler msgHandler);

        void SendInput(Msg_PlayerInput msg);
        void SendMissFrameReq(int missFrameTick);
        void SendMissFrameRepAck(int missFrameTick);
        void SendHashCodes(int firstHashTick, List<long> allHashCodes, int startIdx, int count);

        void SendGameEvent(byte[] data);
        void SendLoadingProgress(byte progress);
        
        
        void ConnectToGameServer(IPEndInfo _gameTcpEnd, Msg_C2G_Hello helloBody);
        void OnLoadLevelProgress(float progress);
    }

    public class RoomMsgManager : NetworkProxy, IRoomMsgManager {
        private delegate void DealNetMsg(BaseFormater data);

        private delegate BaseFormater ParseNetMsg(Deserializer reader);

        public EGameState CurGameState = EGameState.Idle;

        private NetClient<EMsgSC> _netUdp;
        private NetClient<EMsgSC> _netTcp;

        private float _curLoadProgress;
        private float _nextSendLoadProgressTimer;
        private BaseRoomMsgHandler _handler;


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

        public void Init(BaseRoomMsgHandler msgHandler){
            _maxMsgId = (byte) System.Math.Min((int) EMsgSC.EnumCount, (int) byte.MaxValue);
            _allMsgDealFuncs = new DealNetMsg[_maxMsgId];
            _allMsgParsers = new ParseNetMsg[_maxMsgId];
            RegisterMsgHandlers();
            _handler = msgHandler;
        }

        public override void DoUpdate(int deltaTime){
            base.DoUpdate(deltaTime);
            if (CurGameState == EGameState.PartLoading) {
                if (_nextSendLoadProgressTimer < Time.timeSinceLevelLoad) {
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
                if (_curLoadProgress > 0) _curLoadProgress = 1;
                if (_curLoadProgress < 0) _curLoadProgress = 0;
                var val = _curLoadProgress * 70 +
                          (HasRecvGameDta ? 10 : 0) +
                          (HasConnGameUdp ? 10 : 0) +
                          (HasConnGameTcp ? 10 : 0);

                return (byte) val;
            }
        }


        public void OnLoadLevelProgress(float progress){
            _curLoadProgress = (byte) progress;
            if (CurProgress >= 100) {
                CurGameState = EGameState.PartLoaded;
                _nextSendLoadProgressTimer += Time.timeSinceLevelLoad + 0.5f;
                SendLoadingProgress(CurProgress);
            }
        }

        public void ConnectToGameServer(IPEndInfo _gameTcpEnd, Msg_C2G_Hello helloBody){
            ResetStatus();
            CurGameState = EGameState.PartLoading;
            this.helloBody = helloBody.Hello;
            InitNetClient(ref _netTcp, _gameTcpEnd.Ip, _gameTcpEnd.Port, () => {
                HasConnGameTcp = true;
                _netTcp.SendMessage(EMsgSC.C2G_Hello, helloBody, (status, respond) => {
                        if (status != EResponseStatus.Failed) {
                            var rMsg = respond.Parse<Msg_G2C_Hello>();
                            _curMapId = rMsg.MapId;
                            _localId = rMsg.LocalId;
                            _gameUdpEnd = rMsg.UdpEnd;
                            _handler.OnTcpHello(_curMapId, _localId);
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
                _netUdp.SendMessage(EMsgSC.C2G_UdpHello,
                    new Msg_C2G_UdpHello() {
                        Hello = helloBody
                    }
                );
                _handler.OnTcpHello(_curMapId, _localId);
            });
        }


        #region tcp

        public Msg_G2C_GameStartInfo GameStartInfo { get; private set; }

        public void SendTcp(EMsgSC msgId, BaseFormater body){
            var writer = new Serializer();
            writer.PutByte((byte) msgId);
            body.Serialize(writer);
            _netTcp?.SendMessage(EMsgSC.C2G_TcpMessage, Compressor.Compress(writer));
        }

        protected void C2G_GameEvent(IIncommingMessage reader){
            var msg = reader.Parse<Msg_G2C_GameEvent>();
            _handler.OnGameEvent(msg.Data);
        }

        protected void G2C_GameStartInfo(IIncommingMessage reader){
            var msg = reader.Parse<Msg_G2C_GameStartInfo>();
            _handler.OnGameStartInfo(msg);
        }

        public void SendGameEvent(byte[] msg){
            SendTcp(EMsgSC.C2G_GameEvent, new Msg_C2G_GameEvent() {Data = msg});
        }

        public void SendLoadingProgress(byte progress){
            SendTcp(EMsgSC.C2G_LoadingProgress, new Msg_C2G_LoadingProgress() {
                Progress = progress
            });
        }

        #endregion

        #region udp

        private byte _maxMsgId = byte.MaxValue;
        private DealNetMsg[] _allMsgDealFuncs;
        private ParseNetMsg[] _allMsgParsers;


        private void RegisterMsgHandlers(){
            RegisterNetMsgHandler(EMsgSC.G2C_RepMissFrame, G2C_RepMissFrame, ParseData<Msg_ServerFrames>);
            RegisterNetMsgHandler(EMsgSC.G2C_FrameData, G2C_FrameData, ParseData<Msg_ServerFrames>);
        }

        private void RegisterNetMsgHandler(EMsgSC type, DealNetMsg func, ParseNetMsg parseFunc){
            _allMsgDealFuncs[(int) type] = func;
            _allMsgParsers[(int) type] = parseFunc;
        }

        private T ParseData<T>(Deserializer reader) where T : BaseFormater, new(){
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
            writer.PutByte((byte) msgId);
            body.Serialize(writer);
            _netUdp.SendMessage(EMsgSC.C2G_UdpMessage, Compressor.Compress(writer), EDeliveryMethod.Unreliable);
        }


        protected void G2C_UdpMessage(IIncommingMessage reader){
            var data = reader.GetData();
            OnRecvMsg(data);
        }

        protected void OnRecvMsg(Deserializer reader){
            var msgType = reader.GetByte();
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
                Debug.LogError($" Deal Msg Error :{msgType}  " + e);
            }
        }

        protected void G2C_FrameData(BaseFormater reader){
            var msg = reader as Msg_ServerFrames;
            _handler.OnServerFrames(msg);
        }

        protected void G2C_RepMissFrame(BaseFormater reader){
            var msg = reader as Msg_ServerFrames;
            _handler.OnMissFrames(msg);
        }

        #endregion
    }
}