using System;
using System.Collections.Generic;
using LiteDB;
using LiteNetLib;
using Lockstep.Networking;
using Lockstep.Serialization;
using Lockstep.Server.Common;
using Lockstep.Util;
using NetMsg.Common;
using NetMsg.Server;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Server.Database {
    public class DatabaseServer : Common.Server {
        private IAuthDatabase _authDb;

        private IProfilesDatabase _profilesDb;

        //Server DS
        private NetServer<EMsgDS, IServerProxy> _netServerDS;

        public override void DoAwake(ServerConfigInfo info){
            base.DoAwake(info);
            _authDb = new AuthDbLdb(new LiteDatabase(@"auth.db"));
            _profilesDb = new ProfilesDatabaseLdb(new LiteDatabase(@"profiles.db"));
        }

        public override void DoStart(){
            base.DoStart();
            InitServerDS();
        }

        private void InitServerDS(){
            InitNetServer(ref _netServerDS, _serverConfig.serverPort);
        }

        protected override ServerIpInfo GetSlaveServeInfo(){
            return new ServerIpInfo() {
                port = _serverConfig.serverPort,
                ip = IP,
                serverType = (byte) this.serverType,
            };
        }

        protected void S2D_ReqUserInfo(IIncommingMessage reader){
            var msg = reader.Parse<Msg_ReqAccountData>();
            _authDb.GetAccount(msg.account, (dbData) => {
                reader.Respond(EMsgDS.D2S_RepUserInfo,
                    new Msg_RepAccountData() {accountData = dbData as AccountData});
            });
        }

        protected void S2D_ReqCreateUser(IIncommingMessage reader){
            var msg = reader.Parse<Msg_CreateAccount>();
            _authDb.GetAccount(msg.account, (dbData) => {
                if (dbData == null) {
                    var newInfo = _authDb.CreateAccountObject();
                    newInfo.Username = msg.account;
                    newInfo.Password = msg.password;
                    newInfo.Email = "null" + new Random().Next();
                    newInfo.Token = "aaa";
                    newInfo.IsAdmin = true;
                    newInfo.IsGuest = false;
                    newInfo.IsEmailConfirmed = false;
                    _authDb.InsertNewAccount(newInfo
                        , () => { reader.Respond(EMsgDS.D2S_RepCreateUser, new Msg_RepCreateResult() {result = 0}); });
                }
                else {
                    reader.Respond(EMsgDS.D2S_RepCreateUser,
                        new Msg_RepCreateResult() {result = 1});
                }
            });
        }
    }
}