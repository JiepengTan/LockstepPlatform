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
        private NetServer<EMsgDS> _netServerDS;

        public override void DoAwake(ServerConfigInfo info){
            base.DoAwake(info);
            _authDb = new AuthDbLdb(new LiteDatabase(@"auth.db"));
            _profilesDb = new ProfilesDatabaseLdb(new LiteDatabase(@"profiles.db"));
        }

        public override void DoStart(){
            base.DoStart();
            LRandom.SetSeed((uint) DateTime.Now.Millisecond);
            InitServerDS();
        }

        private void InitServerDS(){
            InitNetServer(ref _netServerDS, _serverConfig.serverPort);
        }

        //TODO CacheInfo
        protected void S2D_ReqGameData(IIncommingMessage reader){
            var msg = reader.Parse<Msg_S2D_ReqGameData>();
            _authDb.GetGameData(msg.account, (dbData) => {
                reader.Respond(EMsgDS.D2S_RepUserInfo,
                    new Msg_D2S_RepGameData() {data = dbData as GameData});
            });
        }

        protected void S2D_SaveGameData(IIncommingMessage reader){
            var msg = reader.Parse<Msg_S2D_SaveGameData>();
            _authDb.UpdateGameData(msg.data, () => {
                reader.Respond(1,EResponseStatus.Success);
            });
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
                    newInfo.Email = "null" + LRandom.Next();
                    newInfo.Token = "aaa";
                    newInfo.IsAdmin = true;
                    newInfo.IsGuest = false;
                    newInfo.IsEmailConfirmed = false;
                    _authDb.InsertNewAccount(newInfo
                        , (userId) => { reader.Respond(EMsgDS.D2S_RepCreateUser, new Msg_RepCreateResult() {result = 0, userId = userId}); });
                }
                else {
                    reader.Respond(EMsgDS.D2S_RepCreateUser,
                        new Msg_RepCreateResult() {result = 1});
                }
            });
        }
    }
}