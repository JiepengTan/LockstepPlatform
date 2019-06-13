using Lockstep.Game;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class MapTool : UnityEngine.MonoBehaviour {
    public int curLevel;

    public int maxTick = 100;

    public int curTick {
        get { return mainMgr?.CurTick ?? 0; }
        set {
            if (mainMgr != null) {
                mainMgr.CurTick = curTick;
            }
        }
    }


    [HideInInspector] public Main mainMgr;


    public void EditorUpdate(){
        mainMgr?.Update();
        mainMgr?.FixedUpdate();
    }

    public void StopEditorSimulate(){
        if (mainMgr != null) GameObject.DestroyImmediate(mainMgr.gameObject);
    }
#if false
    [Button("LoadLevel")]
    void LoadLevel(){
        var grid = GameObject.FindObjectOfType<Grid>();
        if (grid == null)
            return;
        MapManager.LoadMap(grid, curLevel);
    }

    [Button("SaveLevel")]
    void SaveLevel(){
        var grid = GameObject.FindObjectOfType<Grid>();
        if (grid == null)
            return;
        MapManager.SaveLevel(grid, curLevel);
#if UNITY_EDITOR
        EditorUtility.DisplayDialog("提示", "Finish Save " + curLevel, "OK");
#endif
    }
#endif
}