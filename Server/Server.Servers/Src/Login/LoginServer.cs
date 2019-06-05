using Lockstep.Networking;
using Lockstep.Serialization;
using Lockstep.Server.Common;
using NetMsg.Common;
using NetMsg.Server;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Server.Login {
    public class LoginServer : Common.Server {
        private NetServer<EMsgSC, IServerProxy> _netServerSC;
        private NetClient<EMsgDS> _netClientDS;

        public override void DoStart(){
            base.DoStart();
            InitServerSC();
        }

        private void InitServerSC(){
            InitNetServer(ref _netServerSC, _serverConfig.tcpPort);
        }

        protected override void OnMasterServerInfo(ServerIpInfo info){
            if (info.serverType == (byte) EServerType.DatabaseServer) {
                InitClientDSMaster(info);
            }
        }

        private void InitClientDSMaster(ServerIpInfo info){
            InitNetClient(ref _netClientOMS, info.ip, info.port, OnDBMasterConn);
        }

        private void OnDBMasterConn(){
            _netClientOMS.SendMessage(EMsgMS.S2M_ReqOtherServerInfo,
                new Msg_ReqOtherServerInfo() {serverType = (byte) EServerType.DatabaseServer},
                (status, reader) => {
                    var msg = reader.Parse<Msg_RepOtherServerInfo>();
                    var info = msg.serverInfo;
                    if (EServerType.DatabaseServer == (EServerType) info.serverType) {
                        InitNetClient(ref _netClientDS, info.ip, info.port, OnDBConn);
                    }
                }
            );
        }

        private void OnDBConn(){
            TestDB();
        }

        private void TestDB(){
            ReqUserInfo("LockstepPlatform", "123");
            CreateUser("LockstepPlatform", "123");
            ReqUserInfo("LockstepPlatform", "123");
            ReqUserInfo("hehehe", "123");
        }

        void ReqUserInfo(string account, string password){
            _netClientDS.SendMessage(EMsgDS.S2D_ReqUserInfo, new Msg_ReqAccountData() {
                account = account,
                password = password
            }, (status, response) => {
                var msg = response.Parse<Msg_RepAccountData>();
                Debug.Log($" S2D_ReqUserInfo account:{account} password:{password} Respond:{msg.ToString()}");
            });
        }

        void CreateUser(string account, string password){
            _netClientDS.SendMessage(EMsgDS.S2D_ReqCreateUser, new Msg_ReqAccountData() {
                account = account,
                password = password
            }, (status, response) => {
                var msg = response.Parse<Msg_RepCreateResult>();
                Debug.Log($" S2D_ReqCreateUser account:{account} password:{password} Respond:{msg.ToString()}");
            });
        }
    }
}