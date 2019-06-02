using System;
using System.Collections.Generic;
using System.Diagnostics;
using LiteNetLib;
using Lockstep.Serialization;
using Lockstep.Server.Common;
using NetMsg.Common;
using NetMsg.Server;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Server.Login {


    

    public class LoginServer : Common.Server {
        #region Server LC
        //Server SC
        private NetServer<EMsgSC, INetProxy> _netServerSC;
        private void InitServerSC(){
            _netServerSC = new NetServer<EMsgSC, INetProxy>(Define.MSKey, (int) EMsgSC.EnumCount,"C2S",this,
                (peer) => new ServerProxy(peer));
            _netServerSC.Run(_serverConfig.tcpPort);
        }
        #endregion


        public override void DoStart( ){
            base.DoStart();
            InitServerSC();
        }
        public override void PollEvents(){
            base.PollEvents();
            _netServerSC?.PollEvents();
        }
    }
}