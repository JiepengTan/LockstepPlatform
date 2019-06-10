using System.IO;
using Lockstep.Core;
using Lockstep.Serialization;
using NetMsg.Common;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif
namespace Lockstep.Game {
    public partial class Main {
        /// <summary>
        /// 回放模式
        /// </summary>
        public bool IsVideoMode;

        public string RecordPath;
        public int MaxRunTick = int.MaxValue;
        public Msg_L2C_StartGame gameInfo;
        public Msg_RepMissFrame framesInfo;

        public float realtimeSinceStartup;
        
        
        public bool isRunVideo;
        public int JumpToTick = 10;
        public void OpenRecordFile(string path){
            var bytes = File.ReadAllBytes(path);
            var reader = new Deserializer(Compressor.Decompress(bytes));
            var TypeId = reader.GetInt32();
            var RoomId = reader.GetInt32();
            var Seed = reader.GetInt32();
            var AllActors = reader.GetBytes_255();
            var msg = new Msg_RepMissFrame();
            msg.startTick = 0;
            msg.Deserialize(reader);
            var msgStartGame = new Msg_L2C_StartGame();
            //msgStartGame.RoomID = RoomId;
            //msgStartGame.Seed = Seed;
            //msgStartGame.AllActors = AllActors;
            //msgStartGame.SimulationSpeed = 60;
            //MaxRunTick = msg.frames.Length + 1;
            IsVideoMode = true;
            framesInfo = msg;
            gameInfo = msgStartGame;
        }
    }

    public partial class Main {
        public Camera gameCamera;
        public Vector2Int renderTextureSize;
        public Camera mainCamera;
        public RenderTexture rt;
        private void DoAwake(){
            rt  = new RenderTexture(renderTextureSize.x,renderTextureSize.y,1,RenderTextureFormat.ARGB32);
            gameCamera.targetTexture = rt;
#if !UNITY_EDITOR
            IsVideoMode = false;
#endif
            if (IsVideoMode) {
                FrameBuffer.SnapshotFrameInterval = 20;
                OpenRecordFile(RecordPath);
                _constStateService.IsVideoMode = true;
            }

            //set resolution for debug
            Screen.SetResolution(1024, 768, false);
        }

        private void DoStart(){ }

        private void AfterStart(){
            if (IsVideoMode) {
                EventHelper.Trigger(EEvent.BorderVideoFrame, framesInfo);
                EventHelper.Trigger(EEvent.OnGameCreate, gameInfo);
            }
        }

        private void DoUpdate(float deltaTime){
            realtimeSinceStartup = Time.realtimeSinceStartup;
            _constStateService.IsRunVideo = isRunVideo;
            if (IsVideoMode && isRunVideo && CurTick < MaxRunTick) {
                _simulationService.RunVideo();
            }

            if (IsVideoMode && !isRunVideo) {
                _simulationService.JumpTo(JumpToTick);
            }
        }
        private void DoFixedUpdate(){ }
        private void DoDestroy(){ }

        public bool IsGameOver(){
            return false;
        }
    }
}