using System;
using System.IO;
using Lockstep.Serialization;
using Lockstep.Util;
using NetMsg.Common;

namespace Lockstep.Game {
    [Serializable]
    public class MainManager : ILifeCycle {
        /// <summary>
        /// 回放模式
        /// </summary>
        public bool IsVideoMode;

        public string RecordPath;
        public int MaxRunTick = int.MaxValue;
        public Msg_G2C_GameStartInfo GameStartInfo;
        public Msg_RepMissFrame FramesInfo;
        public float realtimeSinceStartup;

        public bool isRunVideo;
        public int JumpToTick = 10;
        public int CurTick;

        private ISimulation _simulationService;
        private IConstStateService _constStateService;

        public void OpenRecordFile(string path){
            var bytes = File.ReadAllBytes(path);
            var reader = new Deserializer(Compressor.Decompress(bytes));
            GameStartInfo = reader.Parse<Msg_G2C_GameStartInfo>();
            FramesInfo = reader.Parse<Msg_RepMissFrame>();
            MaxRunTick = FramesInfo.frames.Length + 1;
            IsVideoMode = true;
        }

        public void DoAwake(IServiceContainer serviceContainer){
            _simulationService = serviceContainer.GetService<ISimulation>();
            _constStateService = serviceContainer.GetService<IConstStateService>();

#if !UNITY_EDITOR
            IsVideoMode = false;
#endif
            if (IsVideoMode) {
                _constStateService.SnapshotFrameInterval = 20;
                OpenRecordFile(RecordPath);
                _constStateService.IsVideoMode = true;
            }

        }

        public void DoStart(){
            if (IsVideoMode) {
                EventHelper.Trigger(EEvent.BorderVideoFrame, FramesInfo);
                EventHelper.Trigger(EEvent.OnGameCreate, GameStartInfo);
            }
        }

        public void DoUpdate(int deltaTimeMs){
            realtimeSinceStartup = LTime.realtimeSinceStartup;
            _constStateService.IsRunVideo = isRunVideo;
            if (IsVideoMode && isRunVideo && CurTick < MaxRunTick) {
                _simulationService.RunVideo();
                return;
            }

            if (IsVideoMode && !isRunVideo) {
                _simulationService.JumpTo(JumpToTick);
            }
        }

        public void DoFixedUpdate(){ }
        public void DoDestroy(){ }
        public void OnApplicationQuit(){ }
    }
}