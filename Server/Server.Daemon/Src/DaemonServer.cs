using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Lockstep.Serialization;
using Lockstep.Server.Common;
using Server.Common;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Server.Daemon {
    public enum EDaemonMsg {
        ReportState,
    }

    public class DaemonServer : BaseServer, IDaemonServer {
        private Dictionary<int, EServerType> servers = new Dictionary<int, EServerType>();
        private int _reportInterval = 1000;
        private int _reportTimer = 0;
        private PerformanceCounter _cpuCounter;
        private PerformanceCounter _memCounter;
        private DaemonState _curState;
        private ConfigInfo _config;

        public void RegisterServer(IGameServer server){ }

        public void ReqStartServer(EServerType type){
            //TODO 根据个Daemon 的cpu 占用决定究竟是哪一个改启动新的服务
            StartServer(type);
        }

        public void StartServer(EServerType type){
            var serverConfig = _config.GetServerConfig(type);
            if (serverConfig != null) {
                LunchProgram(serverConfig);
            }
        }

        public void ReportState(DaemonState state){
            var writer = new Serializer();
            writer.PutByte((byte) EDaemonMsg.ReportState);
            state.Serialize(writer);
            var bytes = writer.CopyData();
            masterServer?.SendMsg(bytes);
        }

        public void ReportState(){
            _curState.cpu = _cpuCounter.NextValue();
            _curState.memory = _memCounter.NextValue();
            var serves = servers.Values;
            _curState.localServers = new byte[serves.Count];
            int i = 0;
            foreach (var type in serves) {
                _curState.localServers[i++] = (byte) type;
            }

            ReportState(_curState);
        }

        public override void DoStart(ServerConfigInfo info){
            _curState = new DaemonState();
            _config = ServerUtil.LoadConfig();
            if (_config.IsMaster()) {
                foreach (var serverConfig in _config.servers) {
                    LunchProgram(serverConfig);
                }
            }
            
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _memCounter = new PerformanceCounter("Memory", "Available MBytes");
        }

        public override void DoUpdate(int deltaTime){
            base.DoUpdate(deltaTime);
            _reportTimer -= deltaTime;
            if (_reportTimer <= 0) {
                _reportTimer = _reportInterval;
                ReportState();
            }
        }

        void LunchProgram(ServerConfigInfo configInfo){
            Process proc = Process.Start(configInfo.path);
            if (proc != null) {
                servers.Add(proc.Id, (EServerType) configInfo.type);
                proc.EnableRaisingEvents = true;
                proc.Exited += new EventHandler(OnProcExited);
            }
        }
        void OnProcExited(object sender, EventArgs e){
            var proc = (Process) sender;
            var type = servers[proc.Id];
            servers.Remove(proc.Id);
            Debug.Log($"type {type}  name {proc.ProcessName} Exit！");
        }
    }
}