using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using LiteNetLib;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Game {
    public class NetProxy {
        //call back
        public Action OnConnectedEvent;
        public Action OnDisconnectedEvent;
        public Action<byte[]> OnRecvData;
        public Action OnThraedUpdate;

        public bool IsConnected {
            get { return _isConn; }
        }

        private Thread _thread;
        private ThreadParam _threadParam;
        private NetManager _client;
        private EventBasedNetListener _listener;
        private NetPeer _peer;
        private DeliveryMethod _deliveryMethod;

        private string _address;
        private int _port;
        private string _key;
        private bool _isConn = false;
        public int _updateIntervalMs = 16;

        public class ThreadParam {
            public bool IsForceStop = false;
        }

        public void DoStart(string address, int port, bool isUdp, int UpdateIntervalMs, string key){
            this._updateIntervalMs = UpdateIntervalMs;
            this._address = address;
            this._port = port;
            this._key = key;

            if (_thread != null) {
                DoDestroy();
            }

            _threadParam = new ThreadParam();
            _thread = new Thread(() => { ThreadProc(_threadParam); });
            _listener = new EventBasedNetListener();
            _listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) => {
                OnRecvData?.Invoke(dataReader.GetRemainingBytes());
                dataReader.Recycle();
            };
            _listener.PeerDisconnectedEvent += (NetPeer peer, DisconnectInfo disconnectInfo) => {
                Debug.LogError("Disconnected:" + disconnectInfo.ToString());
                OnDisconnectedEvent?.Invoke();
                _isConn = false;
            };
            _listener.PeerConnectedEvent += (peer) => {
                Debug.Log("Connected!!");
                _peer = peer;
                OnConnectedEvent?.Invoke();
                _isConn = true;
            };
            _deliveryMethod = isUdp ? DeliveryMethod.Unreliable : DeliveryMethod.ReliableOrdered;
            _client = new NetManager(_listener);
            _thread.Start();
            _client.Connect(address, port, key);
        }

        public void Connect(){
            _client.Connect(_address, _port, _key);
        }

        public void DoDestroy(){
            _threadParam.IsForceStop = true;
            _thread.Interrupt();
            _thread.Join();
            _client.Stop();
            _client = null;
            _thread = null;
            _threadParam = null;
            _peer = null;
        }
        
        public void SendMsg(byte[] bytes){
            if (_isConn) {
                _peer?.Send(bytes, _deliveryMethod);
            }
        }
        
        void ThreadProc(ThreadParam param){
            var sw = new Stopwatch();
            sw.Start();
            long lastTick = 0;
            while (!param.IsForceStop) {
                try {
                    var curTick = sw.ElapsedMilliseconds;
                    var elapse = curTick - lastTick;
                    if (elapse > _updateIntervalMs) {
                        OnThraedUpdate?.Invoke();
                    }

                    EventPoll();
                }
                catch (ThreadInterruptedException abortException) {
                    Debug.Log("Normal abort");
                    return;
                }
                catch (Exception e) {
                    Debug.LogError(e.ToString());
                }

                Thread.Sleep(1);
            }
        }


        private void EventPoll(){
            _client?.PollEvents();
        }


    }
}