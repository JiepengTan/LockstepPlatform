using System.IO;
using Lockstep.Game;
using NetMsg.Common;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapTool))]
public class EditorMapTool : UnityEditor.Editor {
    private MapTool owner;

    public override void OnInspectorGUI(){
        base.OnInspectorGUI();
        owner = target as MapTool;
        ShowLoadLevel();
        ShowSaveLevel();
        //ShowLoadRecord();
        ShowEnum();
    }

    public int EnumIdx;

    public void ShowEnum(){
        EnumIdx = EditorGUILayout.IntField("EnumIdx ", EnumIdx);
        EditorGUILayout.LabelField("enum: " + ((EMsgSC) EnumIdx).ToString());
    }


    private void ShowSaveLevel(){
        if (GUILayout.Button("SaveLevel")) {
            var grid = GameObject.FindObjectOfType<Grid>();
            if (grid == null)
                return;
            //TODO 修正编译错误
            //Map2DService.SaveLevel(grid, owner.curLevel);
            EditorUtility.DisplayDialog("提示", "Finish Save", "OK");
        }

        return;
    }

    private void ShowLoadLevel(){
        if (GUILayout.Button(" LoadLevel")) {
            var grid = GameObject.FindObjectOfType<Grid>();
            if (grid == null)
                return;
            //TODO
            //TODO 修正编译错误
            //Map2DService.LoadMap(grid, owner.curLevel);
        }
    }
}