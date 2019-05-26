using System.IO;
using Lockstep.Core;
using Lockstep.Game;
using Lockstep.Serialization;
using NetMsg.Game;
using UnityEngine;
using UnityEditor;

namespace Editor {
    [CustomEditor(typeof(Main))]
    public class EditorMain : UnityEditor.Editor {
        private Main owner;

        public override void OnInspectorGUI(){
            base.OnInspectorGUI();
            owner = target as Main;
            ShowLoadRecord();
            ShowRecordInfo();
            ShowJumpTo();
        }

        private void ShowLoadRecord(){
            if (GUILayout.Button("LoadRecord")) {
#if UNITY_EDITOR
                var path = owner.RecordPath = EditorUtility.OpenFilePanel("SelectGameRecord",
                    Path.Combine(Application.dataPath, "../../Record"), "record");
                if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
                    owner.OpenRecordFile(path);
                }
#endif
            }
        }

        public void ShowRecordInfo(){
            //if (GUILayout.Button("StopSimulate")) { }

            if (Application.isPlaying) {
                var tick = EditorGUILayout.IntSlider("Tick ", owner.CurTick, 0, owner.MaxRunTick);
            }
        }

        private void ShowJumpTo(){
            if (GUILayout.Button("Jump")) {
                if (GameManager.Instance.IsPlaying && owner.JumpToTick > 0 && owner.JumpToTick < owner.MaxRunTick) {
                    SimulationManager.Instance.JumpTo(owner.JumpToTick);
                }
            }
        }
    }
}