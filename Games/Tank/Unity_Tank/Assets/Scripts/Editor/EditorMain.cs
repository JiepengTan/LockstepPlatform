using System.IO;
using Lockstep.Game;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MainScript))]
public class EditorMain : UnityEditor.Editor {
    private MainManager owner;
    private Launcher _Launcher;


    public override void OnInspectorGUI(){
        base.OnInspectorGUI();
        _Launcher = (target as MainScript)?._launcher;
        owner = _Launcher?.MainManager;
        ShowLoadRecord();
        ShowRecordInfo();
        ShowJumpTo();
    }

    public T GetService<T>() where T : IService{
        return _Launcher.GetService<T>();
    }

    private void ShowLoadRecord(){
        if (GUILayout.Button("LoadRecord")) {
            var path = owner.RecordPath = EditorUtility.OpenFilePanel("SelectGameRecord",
                Path.Combine(Application.dataPath, "../../../../../Record"), "record");
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
                GetService<ISimulation>().JumpTo(tick);
            }
        }
    }

    private void ShowJumpTo(){
        if (Application.isPlaying) {
            if (GUILayout.Button("Jump")) {
                if (GetService<IGameConstStateService>().IsPlaying && owner.JumpToTick > 0 &&
                    owner.JumpToTick < owner.MaxRunTick) {
                    GetService<ISimulation>().JumpTo(owner.JumpToTick);
                }
            }
        }
    }
}