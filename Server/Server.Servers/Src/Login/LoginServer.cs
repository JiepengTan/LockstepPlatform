using Lockstep.Networking;
using Lockstep.Serialization;
using Lockstep.Server.Common;
using Lockstep.Util;
using NetMsg.Common;
using NetMsg.Server;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Server.Login {
    public class LoginServer : Common.Server {
        private NetServer<EMsgSC> _netServerSC;
        private NetClient<EMsgDS> _netClientDS;
        protected NetClient<EMsgLS> _netClientLI; //其他类型的Master 用于提供服务

        private ServerIpInfo lobbyInfo;

        public override void DoStart(){
            base.DoStart();
            InitServerSC();
        }

        private void InitServerSC(){
            InitNetServer(ref _netServerSC, _serverConfig.tcpPort);
        }

        protected override void OnMasterServerInfo(ServerIpInfo info){
            if (info.ServerType == (byte) EServerType.DatabaseServer) {
                if (_netClientDS != null) return;
                Debug.Log("OnMasterServerInfo" + info);
                ReqOtherServerInfo(EServerType.DatabaseServer, (status, respond) => {
                    if (status != EResponseStatus.Failed) {
                        InitClientDS(respond.Parse<Msg_RepOtherServerInfo>().ServerInfo);
                    }
                });
            }

            if (info.ServerType == (byte) EServerType.LobbyServer) {
                if (_netClientLI != null) return;
                Debug.Log("OnMasterServerInfo" + info);
                ReqOtherServerInfo(EServerType.LobbyServer, (status, respond) => {
                    if (status != EResponseStatus.Failed) {
                        InitClientLI(respond.Parse<Msg_RepOtherServerInfo>().ServerInfo);
                    }
                });
                ReqOtherServerInfo(EServerType.LobbyServer, (status, respond) => {
                    if (status != EResponseStatus.Failed) {
                        lobbyInfo = respond.Parse<Msg_RepOtherServerInfo>().ServerInfo;
                    }
                }, EServerDetailPortType.TcpPort);
            }
        }

        private void InitClientLI(ServerIpInfo info){
            InitNetClient(ref _netClientLI, info.Ip, info.Port, OnLobbyConn);
        }

        private void InitClientDS(ServerIpInfo info){
            InitNetClient(ref _netClientDS, info.Ip, info.Port, OnDBConn);
        }


        private void OnLobbyConn(){
            //TestDB();
            Debug.Log("OnLobbyConn");
        }

        private void OnDBConn(){
            //TestDB();
            Debug.Log("OnDBConn");
        }

        protected void C2I_UserLogin(IIncommingMessage reader){
            var msg = reader.Parse<Msg_C2I_UserLogin>();
            Debug.Log("C2I_UserLogin" + msg);
            ReqUserInfo(msg, reader);
        }

        private void TestDB(){
            //ReqUserInfo("LockstepPlatform", "123");
            //CreateUser("LockstepPlatform", "123");
            //ReqUserInfo("LockstepPlatform", "123");
            //ReqUserInfo("hehehe", "123");
        }

        void ReqUserInfo(Msg_C2I_UserLogin cInfo, IIncommingMessage reader){
            var account = cInfo.Account;
            var password = cInfo.Password;
            _netClientDS?.SendMessage(EMsgDS.S2D_ReqUserInfo, new Msg_ReqAccountData() {
                account = account,
                password = password
            }, (status, response) => {
                var msg = response.Parse<Msg_RepAccountData>();
                if (msg.accountData == null) {
                    CreateUser(cInfo, reader);
                }
                else {
                    //密码不正确
                    NotifyLobbyUserLoginResult(msg.accountData.Password, msg.accountData.UserId, cInfo, reader);
                }
            });
        }

        void CreateUser(Msg_C2I_UserLogin cInfo, IIncommingMessage reader){
            _netClientDS.SendMessage(EMsgDS.S2D_ReqCreateUser, new Msg_ReqAccountData() {
                account = cInfo.Account,
                password = cInfo.Password
            }, (status, response) => {
                var msg = response.Parse<Msg_RepCreateResult>();
                NotifyLobbyUserLoginResult(cInfo.Password, msg.userId, cInfo, reader);
            });
        }

        void NotifyLobbyUserLoginResult(string password, long userId, Msg_C2I_UserLogin cInfo,
            IIncommingMessage reader){
            if (cInfo.Password != password) {
                reader.Respond(EMsgSC.I2C_LoginResult,
                    new Msg_I2C_LoginResult() {LoginResult = (byte) ELoginResult.PasswordMissMatch});
            }
            else {
                var loginHash = "LSHash" + Time.timeSinceLevelLoad;
                _netClientLI.SendMessage(EMsgLS.I2L_UserLogin, new Msg_I2L_UserLogin() {
                        Account = cInfo.Account,
                        GameType = cInfo.GameType,
                        UserId = userId,
                        LoginHash = loginHash
                    },
                    (sta, res) => {
                        reader.Respond(EMsgSC.I2C_LoginResult, new Msg_I2C_LoginResult() {
                            LoginResult = (byte) ELoginResult.Success,
                            UserId = userId,
                            LoginHash = loginHash,
                            LobbyEnd = new IPEndInfo() {Ip = lobbyInfo.Ip, Port = lobbyInfo.Port}
                        });
                    });
            }
        }
    }
}