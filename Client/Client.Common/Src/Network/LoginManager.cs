using System;
using System.Collections.Generic;
using System.Linq;
using Lockstep.Logging;
using Lockstep.Networking;
using NetMsg.Common;

namespace Lockstep.Client {
    public class LoginManager : NetworkProxy {
        private string _name = "";
        private string _account = "";
        private string _password = "";
        private string _encryptHash;
        private string _loginHash;
        private long _userId;
        private int _gameType = 1;

        //match infos
        protected IPEndInfo _gameUdpEnd;
        protected IPEndInfo _gameTcpEnd;
        protected string _gameHash;
        protected int _curMapId;
        protected byte _localId;
        protected int _roomId;
        protected int _gameId;
        public bool IsReconnect;


        private NetClient<EMsgSC> _netClientIC;
        private NetClient<EMsgSC> _netClientLC;

        private RoomInfo[] _roomInfos;

        public RoomInfo[] RoomInfos {
            get { return _roomInfos; }
        }

        private ILoginHandler _loginHandler;

        private string _serverIp;
        private ushort _serverPort;

        private List<RoomPlayerInfo> _playerInfos;
        public List<RoomPlayerInfo> PlayerInfos => _playerInfos;
        private RoomInfo _curRoomInfo;
        public RoomInfo CurRoomInfo => _curRoomInfo;

        private List<RoomChatInfo> _chatInfos = new List<RoomChatInfo>();
        public List<RoomChatInfo> ChatInfos => _chatInfos;

        private IRoomMsgManager _roomMsgMgr;

        public int GameType {
            get => _gameType;
            set => _gameType = value;
        }

        public void Init(IRoomMsgManager roomMsgMgr, ILoginHandler loginHandler, string serverIp, ushort serverPort){
            _roomMsgMgr = roomMsgMgr;
            _loginHandler = loginHandler;
            _loginHandler.SetLogger(this.Debug);
            _serverIp = serverIp;
            _serverPort = serverPort;
        }


        public override void DoAwake(){
            Debug = new DebugInstance(_account + ": ");
        }

        public override void DoStart(){
            InitNetClient(ref _netClientIC, _serverIp, _serverPort, OnConnLogin);
        }


        void OnConnLogin(){
            _loginHandler.OnConnectedLoginServer();
        }

        public bool isLogining = false;

        public void Login(string account, string password){
            _account = account;
            _password = password;
            Debug.SetPrefix(_account + ": ");

            if (_netClientIC != null && !_netClientIC.IsConnected) {
                Debug.Log("Has not connect to LoginServer");
                return;
            }

            if (isLogining) return;
            isLogining = true;
            _netClientIC.SendMessage(EMsgSC.C2I_UserLogin, new Msg_C2I_UserLogin() {
                    Account = _account,
                    EncryptHash = _encryptHash,
                    GameType = 1,
                    Password = _password
                }, (status, response) => {
                    isLogining = false;
                    var rMsg = response.Parse<Msg_I2C_LoginResult>();
                    if (rMsg.LoginResult != 0) {
                        OnLoginFailed(ELoginResult.PasswordMissMatch);
                        return;
                    }
                    else {
                        var lobbyEnd = rMsg.LobbyEnd;
                        _loginHash = rMsg.LoginHash;
                        _userId = rMsg.UserId;
                        InitNetClient(ref _netClientLC, lobbyEnd.Ip, lobbyEnd.Port, OnConnLobby);
                    }
                }
            );
        }

        private void OnConnLobby(){
            //_netClientIC.DoDestroy();
            Debug.Log("OnConnLobby ");
            _netClientLC.SendMessage(EMsgSC.C2L_UserLogin, new Msg_C2L_UserLogin() {
                    UserId = _userId,
                    LoginHash = _loginHash,
                }, (status, response) => {
                    if (status == EResponseStatus.Failed) {
                        OnLoginFailed((ELoginResult) response.AsInt());
                        return;
                    }

                    if (response.OpCode == (short) EMsgSC.L2C_StartGame) {
                        var rMsg = response.Parse<Msg_L2C_StartGame>();
                        OnMsgStartGame(rMsg);
                    }
                    else {
                        var rMsg = response.Parse<Msg_L2C_RoomList>();
                        _roomInfos = rMsg.Rooms;
                        _loginHandler.OnConnLobby(_roomInfos);
                    }
                }
            );
        }

        public void CreateRoom(int mapId, string name, int size){
            _netClientLC.SendMessage(EMsgSC.C2L_CreateRoom, new Msg_C2L_CreateRoom() {
                GameType = _gameType,
                MapId = mapId,
                Name = name,
                MaxPlayerCount = (byte) size
            }, (status, respond) => {
                if (status == EResponseStatus.Failed) {
                    _loginHandler.OnCreateRoom(null, null);
                }
                else {
                    var roomInfo = respond.Parse<Msg_L2C_CreateRoom>();
                    _curRoomInfo = roomInfo.Info;
                    _playerInfos = new List<RoomPlayerInfo>(roomInfo.PlayerInfos);
                    _loginHandler.OnCreateRoom(_curRoomInfo, roomInfo.PlayerInfos);
                }
            });
        }

        public void JoinRoom(int roomId, Action<RoomPlayerInfo[]> resultCallBack){
            _netClientLC.SendMessage(EMsgSC.C2L_JoinRoom, new Msg_C2L_JoinRoom() {
                    RoomId = roomId
                }
                , (status, respond) => {
                    if (status == EResponseStatus.Failed) {
                        resultCallBack(null);
                    }
                    else {
                        var msg = respond.Parse<Msg_L2C_JoinRoomResult>();
                        _playerInfos = new List<RoomPlayerInfo>(msg.PlayerInfos);
                        resultCallBack(msg.PlayerInfos);
                    }
                });
        }

        public void ReqRoomList(int startIdx){
            _netClientLC.SendMessage(EMsgSC.C2L_ReqRoomList, new Msg_C2L_ReqRoomList() {
                    StartIdx = (short) startIdx
                }
            );
        }

        public void LeaveRoom(){
            _netClientLC.SendMessage(EMsgSC.C2L_LeaveRoom, new Msg_C2L_LeaveRoom() { },
                (status, respond) => {
                    if (status != EResponseStatus.Failed) {
                        _loginHandler.OnLeaveRoom();
                        ClearRoomInfo();
                    }
                });
        }

        private void ClearRoomInfo(){
            _chatInfos.Clear();
            _playerInfos.Clear();
            _curRoomInfo = null;
        }

        public void StartGame(){
            _netClientLC.SendMessage(EMsgSC.C2L_StartGame, new Msg_C2L_StartGame() { },
                (status, respond) => {
                    _loginHandler.OnStartRoomResult(status != EResponseStatus.Failed ? 0 : respond.AsInt());
                }
            );
        }

        public void ReadyInRoom(bool isReady){
            _netClientLC.SendMessage(EMsgSC.C2L_ReadyInRoom, new Msg_C2L_ReadyInRoom() {
                State = (byte) (isReady ? 1 : 0)
            });
        }

        public void SendChatInfo(RoomChatInfo chatInfo){
            _netClientLC.SendMessage(EMsgSC.C2L_RoomChatInfo, new Msg_C2L_RoomChatInfo() {ChatInfo = chatInfo});
        }

        private void OnLoginFailed(ELoginResult result){
            _loginHandler.OnLoginFailed(result);
        }

        protected void L2C_RoomList(IIncommingMessage reader){
            var msg = reader.Parse<Msg_L2C_RoomList>();
            _roomInfos = msg.Rooms;
            _loginHandler.OnRoomInfo(_roomInfos);
        }

        protected void S2C_TickPlayer(IIncommingMessage reader){
            var msg = reader.Parse<Msg_S2C_TickPlayer>();
            _loginHandler.OnTickPlayer(msg.Reason);
        }


        protected void L2C_RoomInfoUpdate(IIncommingMessage reader){
            var msg = reader.Parse<Msg_L2C_RoomInfoUpdate>();
            var roomInfos = RoomInfos;
            var roomDict = new Dictionary<int, RoomInfo>();
            if (roomInfos != null) {
                foreach (var rawInfo in roomInfos) {
                    roomDict.Add(rawInfo.RoomId, rawInfo);
                }
            }

            if (msg.ChangedInfo != null) {
                foreach (var info in msg.ChangedInfo) {
                    if (roomDict.TryGetValue(info.RoomId, out var rawInfo)) {
                        rawInfo.CurPlayerCount = info.CurPlayerCount;
                    }
                }
            }

            if (msg.AddInfo != null) {
                foreach (var info in msg.AddInfo) {
                    roomDict[info.RoomId] = info;
                }
            }

            if (msg.DeleteInfo != null) {
                foreach (var delId in msg.DeleteInfo) {
                    roomDict.Remove(delId);
                }
            }

            _roomInfos = roomDict.Values.ToArray();
            _loginHandler.OnRoomInfo(_roomInfos);
        }

        protected void L2C_RoomChatInfo(IIncommingMessage reader){
            var msg = reader.Parse<Msg_L2C_RoomChatInfo>();
            _chatInfos.Add(msg.ChatInfo);
            _loginHandler.OnRoomChatInfo(msg.ChatInfo);
        }

        protected void L2C_ReadyInRoom(IIncommingMessage reader){
            var msg = reader.Parse<Msg_L2C_ReadyInRoom>();
            var info = _playerInfos.Find((a) => a.UserId == msg.UserId);
            if (info != null) {
                info.Status = msg.State;
                _loginHandler.OnPlayerReadyInRoom(msg.UserId, msg.State);
            }
        }

        protected void L2C_LeaveRoom(IIncommingMessage reader){
            var msg = reader.Parse<Msg_L2C_LeaveRoom>();
            var idx = _playerInfos.FindIndex((item) => { return item.UserId == msg.UserId; });
            if (idx != -1) {
                _playerInfos.RemoveAt(idx);
            }

            _loginHandler.OnPlayerLeaveRoom(msg.UserId);
        }

        protected void L2C_JoinRoom(IIncommingMessage reader){
            var msg = reader.Parse<Msg_L2C_JoinRoom>();
            var userId = msg.PlayerInfo.UserId;
            if (_playerInfos == null) {
                _playerInfos = new List<RoomPlayerInfo>();
            }

            var idx = _playerInfos.FindIndex((item) => { return item.UserId == userId; });
            if (idx == -1) {
                _playerInfos.Add(msg.PlayerInfo);
            }
            else {
                _playerInfos[idx] = msg.PlayerInfo;
            }

            _loginHandler.OnPlayerJoinRoom(msg.PlayerInfo);
        }


        protected void L2C_StartGame(IIncommingMessage reader){
            var msg = reader.Parse<Msg_L2C_StartGame>();
            OnMsgStartGame(msg);
        }

        void OnMsgStartGame(Msg_L2C_StartGame msg){
            Debug.Log("L2C_StartGame" + msg);
            _gameTcpEnd = msg.GameServerEnd;
            _gameHash = msg.GameHash;
            _roomId = msg.RoomId;
            _gameId = msg.GameId;
            IsReconnect = msg.IsReconnect;

            var rmsg = new Msg_C2G_Hello() {
                Hello = new MessageHello() {
                    GameHash = _gameHash,
                    GameId = _gameId,
                    GameType = _gameType,
                    IsReconnect = IsReconnect,
                    UserInfo = new GamePlayerInfo() {
                        UserId = _userId,
                        Account = _account,
                        LoginHash = _loginHash,
                    }
                }
            };
            _loginHandler.OnGameStart(rmsg, _gameTcpEnd, IsReconnect);
            _roomMsgMgr.ConnectToGameServer(rmsg, _gameTcpEnd, IsReconnect);
        }
    }
}