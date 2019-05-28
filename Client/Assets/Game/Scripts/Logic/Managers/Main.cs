using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Lockstep.Core;
using Lockstep.Logging;
using Lockstep.Math;
using Lockstep.Serialization;
using NetMsg.Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
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
        public Msg_StartGame gameInfo;
        public Msg_RepMissFrame framesInfo;

        public float realtimeSinceStartup;
        
        
        public bool isRunVideo;
        public int JumpToTick = 10;
        public void OpenRecordFile(string path){
            var bytes = File.ReadAllBytes(path);
            var reader = new Deserializer(Compressor.Decompress(bytes));
            var TypeId = reader.GetInt();
            var RoomId = reader.GetInt();
            var Seed = reader.GetInt();
            var AllActors = reader.GetBytes_255();
            var msg = new Msg_RepMissFrame();
            msg.startTick = 0;
            msg.Deserialize(reader);
            var msgStartGame = new Msg_StartGame();
            msgStartGame.RoomID = RoomId;
            msgStartGame.Seed = Seed;
            msgStartGame.AllActors = AllActors;
            msgStartGame.SimulationSpeed = 60;
            MaxRunTick = msg.frames.Length + 1;
            IsVideoMode = true;
            framesInfo = msg;
            gameInfo = msgStartGame;
        }
    }

    public partial class Main {
        private void DoAwake(){
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
                EventHelper.Trigger(EEvent.OnRoomGameStart, gameInfo);
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