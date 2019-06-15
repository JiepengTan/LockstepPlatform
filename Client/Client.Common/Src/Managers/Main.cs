using System;
using System.IO;
using Lockstep.Core;
using Lockstep.Math;
using Lockstep.Serialization;
using NetMsg.Common;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif
namespace Lockstep.Game {
    public partial class MainManager : ManagerReferenceHolder {
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
        public Camera gameCamera;
        public Vector2Int renderTextureSize;
        [HideInInspector] public RenderTexture rt;
        public int CurTick;
        

        public void OpenRecordFile(string path){
            var bytes = File.ReadAllBytes(path);
            var reader = new Deserializer(Compressor.Decompress(bytes));
            GameStartInfo = reader.Parse<Msg_G2C_GameStartInfo>();
            FramesInfo = reader.Parse<Msg_RepMissFrame>();
            MaxRunTick = FramesInfo.frames.Length + 1;
            IsVideoMode = true;
        }
        
        public void DoAwake(){
            rt = new RenderTexture(renderTextureSize.x, renderTextureSize.y, 1, RenderTextureFormat.ARGB32);
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

        public void DoStart(){ }

        public  void AfterStart(){
            if (IsVideoMode) {
                EventHelper.Trigger(EEvent.BorderVideoFrame, FramesInfo);
                EventHelper.Trigger(EEvent.OnGameCreate, GameStartInfo);
            }
        }

        public void DoUpdate(int deltaTimeMs){
            realtimeSinceStartup = Time.realtimeSinceStartup;
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

    }
}