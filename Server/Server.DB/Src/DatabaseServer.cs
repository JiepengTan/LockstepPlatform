using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LiteDB;
using LiteNetLib;
using Lockstep.Game;
using Lockstep.Serialization;
using Lockstep.Server.Common;
using NetMsg.Server;
using Server.Common;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Server.Database {
    public interface IDaemonProxy {
        DaemonState state { get; set; }
        void SendMsg(byte[] data);
    }

    public class DaemonProxy : IDaemonProxy {
        private NetPeer _peer;

        public DaemonProxy(NetPeer peer){
            _peer = peer;
        }

        public DaemonState state { get; set; }

        public void SendMsg(byte[] data){
            _peer?.Send(data, DeliveryMethod.ReliableSequenced);
        }
    }
    public class DatabaseServer : Common.Server {
        private int _reportInterval = 1000;
        private int _reportTimer = 0;
        private DaemonState _curState;
        private PerformanceCounter _cpuCounter;
        private PerformanceCounter _memCounter;

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