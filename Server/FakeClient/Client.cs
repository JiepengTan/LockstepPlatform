using System;
using System.Collections.Generic;
using LitJson;
using Lockstep.Networking;
using Lockstep.Serialization;
using Lockstep.Server.Common;
using NetMsg.Common;

namespace Lockstep.FakeClient {
    public class Client {
        public class Debug {
            public static string prefix = "";

            public static void Log(string ss){
                Console.WriteLine(prefix + ss);
            }
        }

        public bool HasInit;
        private string _account;
        private string _name;
        private string _password;
        private string _encryptHash;
        private string _loginHash;
        private long _userId;
        private int _gameType;


        //match infos
        protected IPEndInfo _gameServerInfo;
        protected string _gameHash;
        protected int _curMapId;
        protected byte _localId;
        protected int _roomId;


        private NetClient<EMsgSC> _netClientIC;
        private NetClient<EMsgSC> _netClientLC;
        private NetClient<EMsgSC> _netClientGC;

        public List<IUpdate> _allClientNet = new List<IUpdate>();
        private IUpdate[] _cachedAllClientNet;
        private RoomInfo[] _roomInfos;

        public static Random rand = new Random(1152);

        public Client(){
            _name = _account = "FakeClient " + rand.Next();
            _password = "1234";
            _gameType = 1;
            Debug.prefix = _name + ": ";
        }

        public virtual void DoAwake(){ }

        public virtual void DoStart(){
            InitNetClient(ref _netClientIC, "127.0.0.1", 7150, OnConnLogin);
        }

        public virtual void DoUpdate(int deltaTime){
            if (_cachedAllClientNet == null) {
                _cachedAllClientNet = _allClientNet.ToArray();
            }

            foreach (var net in _cachedAllClientNet) {
                net.DoUpdate();
            }
        }

        public virtual void DoDestroy(){ }
        public virtual void PollEvents(){ }

        protected void InitNetClient<TMsgType>(ref NetClient<TMsgType> refClient, string ip, int port,
            Action onConnCallback = null) where TMsgType : struct{
            if (NetworkUtil.InitNetClient(ref refClient, ip, port, onConnCallback,this)) return;
            _allClientNet.Add(refClient);
            _cachedAllClientNet = null;
        }

        void OnConnLogin(){
            Debug.Log("OnConnLogin ");
            _netClientIC.SendMessage(EMsgSC.C2I_UserLogin, new Msg_C2I_UserLogin() {
                    Account = _account,
                    EncryptHash = _encryptHash,
                    GameType = 1,
                    Password = _password
                }, (status, response) => {
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
            Debug.Log("OnConnLobby ");
            _netClientLC.SendMessage(EMsgSC.C2L_UserLogin, new Msg_C2L_UserLogin() {
                    userId = _userId,
                    LoginHash = _loginHash,
                }, (status, response) => {
                    if (status == EResponseStatus.Failed) {
                        OnLoginFailed((ELoginResult) response.AsInt());
                        return;
                    }

                    var rMsg = response.Parse<Msg_L2C_RoomList>();
                    _roomInfos = rMsg.Rooms;
                    UpdateRoomsState();
                }
            );
        }

        void UpdateRoomsState(){
            if (_roomInfos == null) {
                CreateRoom(3, "TestRoom", 1);
                return;
            }

            Debug.Log("UpdateRoomsState " + JsonMapper.ToJson(_roomInfos));
        }

        public void CreateRoom(int mapId, string name, int size){
            _netClientLC.SendMessage(EMsgSC.C2L_CreateRoom, new Msg_C2L_CreateRoom() {
                GameType = _gameType,
                MapId = mapId,
                Name = name,
                MaxPlayerCount = (byte) size
            }, (status, respond) => {
                if (status == EResponseStatus.Failed) {
                    Debug.Log("CreateRoom failed reason ");
                }
                else {
                    var roomInfo = respond.Parse<Msg_L2C_CreateRoom>();
                    Debug.Log(roomInfo.ToString());
                    StartGame();
                }
            });
        }

        public void StartGame(){
            _netClientLC.SendMessage(EMsgSC.C2L_StartGame, new Msg_C2L_StartGame() { },
                (status, respond) => {
                    if (status == EResponseStatus.Failed) {
                        Debug.Log("StartGame failed reason ");
                    }
                }
            );
        }

        private void OnLoginFailed(ELoginResult result){
            Debug.Log("Login failed reason " + result);
        }

        public void SendMessage(object type, BaseFormater msg, ResponseCallback callback = null){ }

        protected void L2C_RoomList(IIncommingMessage reader){
            var msg = reader.Parse<Msg_L2C_RoomList>();
            _roomInfos = msg.Rooms;
            UpdateRoomsState();
        }

        protected void L2C_RoomChangedInfo(IIncommingMessage reader){
            var msg = reader.Parse<Msg_C2L_UserLogin>();
            Debug.Log("C2L_UserLogin" + msg);
        }

        protected void L2C_StartGame(IIncommingMessage reader){
            var msg = reader.Parse<Msg_L2C_StartGame>();
            Debug.Log("L2C_StartGame" + msg);
            _gameServerInfo = msg.GameServerEnd;
            _gameHash = msg.GameHash;
            _roomId = msg.RoomId;
            InitNetClient(ref _netClientGC, _gameServerInfo.Ip, _gameServerInfo.Port, () => {
                _netClientGC.SendMessage(EMsgSC.C2G_Hello,
                    new Msg_C2G_Hello() {
                        GameHash = _gameHash,
                        RoomId = _roomId,
                        GameType = _gameType,
                        UserInfo = new GamePlayerInfo() {
                            UserId = _userId,
                            Account = _account,
                            LoginHash = _loginHash,
                        }
                    }, (status, respond) => {
                        if (status != EResponseStatus.Failed) {
                            var rMsg = respond.Parse<Msg_G2C_Hello>();
                            _curMapId = rMsg.MapId;
                            _localId = rMsg.LocalId;
                            LoadMap(_curMapId);
                        }
                    }
                );
            });
        }

        void LoadMap(int mapId){
            Debug.Log("mapId" + mapId);
        }
    }
}