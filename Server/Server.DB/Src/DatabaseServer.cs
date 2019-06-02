using System;
using System.Collections.Generic;
using LiteDB;
using LiteNetLib;
using Lockstep.Serialization;
using Lockstep.Server.Common;
using NetMsg.Server;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Server.Database {
    public class DatabaseServer : Common.Server {
        private IAuthDatabase _authDb;
        private IProfilesDatabase _profilesDb;
        //Server DS
        private NetServer<EMsgDS, INetProxy> _netServerDS;

        public override void DoAwake(ServerConfigInfo info){
            base.DoAwake(info);
            _authDb = new AuthDbLdb(new LiteDatabase(@"auth.db"));
            _profilesDb = new ProfilesDatabaseLdb(new LiteDatabase(@"profiles.db"));
        }
        public override void DoStart(){
            base.DoStart();
            _netServerDS = new NetServer<EMsgDS, INetProxy>(Define.MSKey, (int) EMsgDS.EnumCount,"S2D", this,
                (peer) => new ServerProxy(peer));
        }

        private void OnNet_S2D_ReqUserInfo(INetProxy proxy, Deserializer reader){ }
    }
}