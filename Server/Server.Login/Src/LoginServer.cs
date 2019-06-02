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
            InitNetServer(ref _netServerSC, _serverConfig.tcpPort, (peer) => new ServerProxy(peer));
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
            Debug.Log(" OnDBMasterConn");
            _netClientOMS.Send(EMsgMS.S2M_ReqOtherServerInfo, new Msg_ReqOtherServerInfo() {serverType = (byte) EServerType.DatabaseServer});
        }

        protected void OnMsg_M2S_RepOtherServerInfo(Deserializer reader){
            var msg = reader.Parse<Msg_RepOtherServerInfo>();
            var info = msg.serverInfo;
            if (EServerType.DatabaseServer == (EServerType) info.serverType) {
                Debug.Log("OnMsg_M2S_RepOtherServerInfo " + msg.ToString());
                InitNetClient(ref _netClientDS, info.ip, info.port, OnDBConn);
            }
        }
        private void OnDBConn(){
            Debug.Log(" OnDBConn");
            _netClientDS.Send(EMsgDS.S2D_ReqUserInfo, new Msg_ReqAccountData() {
                account = "LockstepPlatform",
                password = "123"
            });
            _netClientDS.Send(EMsgDS.S2D_ReqCreateUser, new Msg_ReqAccountData() {
                account = "LockstepPlatform",
                password = "123"
            });
            _netClientDS.Send(EMsgDS.S2D_ReqCreateUser, new Msg_ReqAccountData() {
                account = "jiepengtan",
                password = "123"
            });
            _netClientDS.Send(EMsgDS.S2D_ReqUserInfo, new Msg_ReqAccountData() {
                account = "jiepengtan",
                password = "123"
            });
            _netClientDS.Send(EMsgDS.S2D_ReqUserInfo, new Msg_ReqAccountData() {
                account = "LockstepPlatform",
                password = "123"
            });
            _netClientDS.Send(EMsgDS.S2D_ReqUserInfo, new Msg_ReqAccountData() {
                account = "hehehe",
                password = "123"
            });
        }

        protected void OnMsg_D2S_RepUserInfo(Deserializer reader){
            var msg = reader.Parse<Msg_RepAccountData>();
            Debug.Log("OnMsg_D2S_RepUserInfo " + msg.ToString());
        }
        protected void OnMsg_D2S_RepCreateUser(Deserializer reader){
            var msg = reader.Parse<Msg_RepCreateResult>();
            Debug.Log("OnMsg_D2S_RepCreateUser " + msg.ToString());
        }
        
    }
}