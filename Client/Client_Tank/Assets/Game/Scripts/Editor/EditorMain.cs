using System.IO;
using Lockstep.Game;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Main))]
public class EditorMain : UnityEditor.Editor {
    private Main owner;

    public override void OnInspectorGUI(){
        base.OnInspectorGUI();
        owner = (target as Main);
        ShowLoadRecord();
        ShowRecordInfo();
        ShowJumpTo();
    }

    private void ShowLoadRecord(){
        if (GUILayout.Button("LoadRecord")) {
            var path = owner.RecordPath = EditorUtility.OpenFilePanel("SelectGameRecord",
                Path.Combine(Application.dataPath, "../../../Record"), "record");
            if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
                owner.OpenRecordFile(path);
            }
        }

        if (GUILayout.Button("CleanRecord")) {
            owner.GameStartInfo = null;
            owner.FramesInfo = null;
        }
    }

    public void ShowRecordInfo(){
        //if (GUILayout.Button("StopSimulate")) { }

        if (Application.isPlaying) {
            var tick = EditorGUILayout.IntSlider("Tick ", owner.CurTick, 0, owner.MaxRunTick);
            if (tick != owner.CurTick) {
                SimulationManager.Instance.JumpTo(tick);
            }
        }
    }

    private void ShowJumpTo(){
        if (Application.isPlaying) {
            if (GUILayout.Button("Jump")) {
                if (GameManager.Instance.IsPlaying && owner.JumpToTick > 0 && owner.JumpToTick < owner.MaxRunTick) {
                    SimulationManager.Instance.JumpTo(owner.JumpToTick);
                }
            }
        }
    }
}