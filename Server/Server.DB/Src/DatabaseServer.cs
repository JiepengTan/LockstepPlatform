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

        #region Server DS

        //Server DS
        private List<IServerProxy> _slaveDS = new List<IServerProxy>();
        private NetServer<EMsgDS, IServerProxy> _netServerDS;

        private void InitServerDS(){
            _netServerDS = new NetServer<EMsgDS, IServerProxy>(Define.MSKey, (int) EMsgDS.EnumCount,
                OnSlaveConnectDS,
                OnSlaveDisconnectDS);
            RegisterMsgHandlerS2D();
        }

        protected virtual IServerProxy OnSlaveConnectDS(NetPeer peer){
            var server = new ServerProxy(peer);
            _slaveDS.Add(server);
            return server;
        }

        protected virtual void OnSlaveDisconnectDS(NetPeer peer, IServerProxy param){
            _slaveDS.Remove(param);
        }

        void RegisterMsgHandlerS2D(){
            ServerUtil.RegisterEvent<EMsgDS, NetServer<EMsgDS, IServerProxy>.MsgHandler>("OnMsg_S2D", "OnMsg_".Length,
                _netServerDS.RegisterMsgHandler, this);
        }

        #endregion

        public override void DoAwake(ServerConfigInfo info){
            base.DoAwake(info);
            _authDb = new AuthDbLdb(new LiteDatabase(@"auth.db"));
            _profilesDb = new ProfilesDatabaseLdb(new LiteDatabase(@"profiles.db"));
        }

        public override void DoStart(ServerConfigInfo info){
            base.DoStart(info);
            InitServerDS();
        }

        private void OnNet_S2D_ReqUserInfo(IServerProxy proxy,Deserializer reader){}
    }
}