using System.IO;
using Lockstep.Core;
using Lockstep.Game;
using Lockstep.Serialization;
using NetMsg.Common;
using UnityEngine;
using UnityEditor;

namespace Editor {
    [CustomEditor(typeof(MapTool))]
    public class EditorMapTool : UnityEditor.Editor {
        private MapTool owner;

        public override void OnInspectorGUI(){
            base.OnInspectorGUI();
            owner = target as MapTool;
            ShowLoadLevel();
            ShowSaveLevel();
            //ShowLoadRecord();
            ShowRecordInfo();
        }

        public void ShowRecordInfo(){
            if (GUILayout.Button("StopSimulate")) {
                EditorApplication.update -= owner.EditorUpdate;
                owner.StopEditorSimulate();
            }
            var tick = EditorGUILayout.IntSlider("Tick ", owner.curTick, 0, owner.maxTick);
            owner.curTick = tick;
        }

        private void ShowSaveLevel(){
            if (GUILayout.Button("SaveLevel")) {
                var grid = GameObject.FindObjectOfType<Grid>();
                if (grid == null)
                    return;
                MapManager.SaveLevel(grid, owner.curLevel);
                EditorUtility.DisplayDialog("提示", "Finish Save", "OK");
            }

            return;
        }

        private void ShowLoadLevel(){
            if (GUILayout.Button(" LoadLevel")) {
                var grid = GameObject.FindObjectOfType<Grid>();
                if (grid == null)
                    return;
                MapManager.LoadMap(grid, owner.curLevel);
            }
        }

      
    }
}