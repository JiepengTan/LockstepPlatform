using UnityEngine;
using Lockstep.Logging;
using Debug = UnityEngine.Debug;

namespace Lockstep.Game {
    public enum EDir {
        Up,
        Right,
        Down,
        Left,
    }

    public class Main : MonoBehaviour {
        public const string ClientKey = "SomeConnectionKey";
        public static Main Instance;
        public string ServerIp = "127.0.0.1";
        public int ServerPort = 9050;
        private Simulation _simulation;
        
        public bool IsConnected { get; }
        public int CurTick { get; }
        public int HashCode { get; }
        public int AgentCount { get; }

        public NetMgr netMgr;
        private void Awake(){
            Instance = this;
            Log.OnMessage += OnLog;
            netMgr = new NetMgr();
            _simulation = new Simulation(new Contexts(), netMgr);
            netMgr.Init(_simulation,ServerIp, ServerPort,ClientKey);
        }


        private void Start(){
            netMgr.StartLobby();;
        }

        private void OnDestroy(){
            netMgr.DoDestroy();
            _simulation.DoDestroy();
        }

        void Update(){
            var deltaTimeMs = Time.deltaTime * 1000;
            netMgr.DoUpdate(deltaTimeMs);
            _simulation.DoUpdate(deltaTimeMs);
        }


        void OnLog(object sender, LogEventArgs args){
            switch (args.LogSeverity) {
                case LogSeverity.Info:
                    UnityEngine.Debug.Log(args.Message);
                    break;
                case LogSeverity.Warn:
                    UnityEngine.Debug.LogWarning(args.Message);
                    break;
                case LogSeverity.Error:
                    UnityEngine.Debug.LogError(args.Message);
                    break;
                case LogSeverity.Exception:
                    UnityEngine.Debug.LogError(args.Message);
                    break;
            }
        }
    }
}