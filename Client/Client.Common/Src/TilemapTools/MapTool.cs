using Lockstep.Game;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class MapTool : UnityEngine.MonoBehaviour {
    public int curLevel;

    public int maxTick = 100;


    [HideInInspector] public Launcher launcherMgr;


    public void EditorUpdate(){
        launcherMgr?.DoUpdate((int)(Time.deltaTime * 1000));
        launcherMgr?.DoFixedUpdate();
    }

    public void StopEditorSimulate(){
        //if (mainMgr != null) GameObject.DestroyImmediate(mainMgr.gameObject);
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