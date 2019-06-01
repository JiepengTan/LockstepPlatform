using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LiteNetLib;
using Lockstep.Game;
using Lockstep.Serialization;
using Lockstep.Server.Common;
using NetMsg.Common;
using NetMsg.Server;
using Server.Common;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Server.Login {
    public interface IClientProxy {
        void SendMsg(byte[] data);
    }

    public class ClientProxy : IClientProxy {
        private NetPeer _peer;
        
        public ClientProxy(NetPeer peer){
            _peer = peer;
        }
        
        public void SendMsg(byte[] data){
            _peer?.Send(data, DeliveryMethod.ReliableSequenced);
        }
    }
    

    public class LoginServer : Common.Server {
        private int _reportInterval = 1000;
        private int _reportTimer = 0;
        private DaemonState _curState;
        private PerformanceCounter _cpuCounter;
        private PerformanceCounter _memCounter;

        #region Server LC
        
        //Server XS
        private List<IServerProxy> _slaveXS = new List<IServerProxy>();
        private NetServer<EMsgSC, IServerProxy> _netServerXS;

        private void InitServerXS(){
            _netServerXS = new NetServer<EMsgSC, IServerProxy>(Define.MSKey, (int) EMsgXS.EnumCount,
                OnSlaveConnectXS,
                OnSlaveDisconnectXS);
            RegisterMsgHandlerS2X();
        }

        protected virtual IServerProxy OnSlaveConnectXS(NetPeer peer){
            var server = new ServerProxy(peer);
            _slaveXS.Add(server);
            return server;
        }

        protected virtual void OnSlaveDisconnectXS(NetPeer peer, IServerProxy param){
            _slaveXS.Remove(param);
        }

        void RegisterMsgHandlerS2X(){
            ServerUtil.RegisterEvent<EMsgSC, NetServer<EMsgSC, IServerProxy>.MsgHandler>("OnMsg_X2Y", "OnMsg_".Length,
                _netServerXS.RegisterMsgHandler, this);
        }

        #endregion


        public override void DoStart(ServerConfigInfo info){
            base.DoStart(info);
            _curState = new DaemonState();
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _memCounter = new PerformanceCounter("Memory", "Available MBytes");
            InitServerXS();
        }



        public void OnMsg_S2X_RegisterServer(IServerProxy server, Deserializer reader){ }
        public void OnMsg_S2X_StartServer(IServerProxy server, Deserializer reader){ }
        public void OnMsg_S2X_ShutdownServer(IServerProxy server, Deserializer reader){ }

    }
}