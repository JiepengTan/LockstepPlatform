using System;
using System.Collections.Generic;

namespace Lockstep.Networking {

    public class NetworkProxy {
        protected List<IPollEvents> _allServerNet = new List<IPollEvents>();
        private IPollEvents[] _cachedAllServerNet;
        protected List<IUpdate> _allClientNet = new List<IUpdate>();
        private IUpdate[] _cachedAllClientNet;
        
        
        public virtual void DoAwake(){ }
        public virtual void DoStart(){ }
        public virtual void DoDestroy(){ }
        
        public virtual void DoUpdate(int deltaTime){
            if (_cachedAllClientNet == null) {
                _cachedAllClientNet = _allClientNet.ToArray();
            }

            foreach (var net in _cachedAllClientNet) {
                net.DoUpdate();
            }
        }
        public virtual void PollEvents(){
            if (_cachedAllServerNet == null) {
                _cachedAllServerNet = _allServerNet.ToArray();
            }

            foreach (var net in _cachedAllServerNet) {
                net.PollEvents();
            }
        }

        protected void InitNetServer<TMsgType>(ref NetServer<TMsgType> refServer, int port,
            PeerActionHandler disconnectedHandler = null
        ) where TMsgType : struct{
            if (NetworkUtil.InitNetServer(ref refServer, port, this)) return;
            if (disconnectedHandler != null) refServer.OnDisconnected += disconnectedHandler;
            _allServerNet.Add(refServer);
            _cachedAllServerNet = null;
        }

        protected void InitNetClient<TMsgType>(ref NetClient<TMsgType> refClient, string ip, int port,
            Action onConnCallback = null) where TMsgType : struct{
            if (NetworkUtil.InitNetClient(ref refClient, ip, port, onConnCallback, this)) return;
            _allClientNet.Add(refClient);
            _cachedAllClientNet = null;
        }
        
    }
}