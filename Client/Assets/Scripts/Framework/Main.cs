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
        private readonly NetHelper _netMgr = new NetHelper();


        public bool IsConnected { get; }
        public int CurTick { get; }
        public int HashCode { get; }
        public int AgentCount { get; }
        

        private void Awake(){
            Instance = this;
            Log.OnMessage += OnLog;
            //_simulation = new Simulation(_netMgr);
            _netMgr.Init(ServerIp, ServerPort, _simulation, ClientKey);
        }


        private void Start(){
            _netMgr.DoStart();
        }

        private void OnDestroy(){
            _netMgr.DoDestroy();
            _simulation.DoDestroy();
        }
        
        void Update(){
            SendInput();
            _netMgr.DoUpdate();
            _simulation.Update(Time.deltaTime * 1000);
        }

        void SendInput(){
            var vert = UnityEngine.Input.GetAxis("Vertical");
            var horz = UnityEngine.Input.GetAxis("Horizontal");
            var isFire = Input.GetKey(KeyCode.Space);
            var absv = Mathf.Abs(vert);
            var absh = Mathf.Abs(horz);
            var dir = (absv > absh ? (vert > 0 ? EDir.Up : EDir.Down) : (horz > 0 ? EDir.Right : EDir.Left));
            const float MIN_INPUT_VAL = 0.01f;
            if (absv < MIN_INPUT_VAL && absh < MIN_INPUT_VAL && !isFire) {
                //无输入
                return;
            }
            _netMgr.SendInput(dir, isFire);
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