using System.IO;
using System.Security.Policy;
using Lockstep.Game;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Editor {
    [CustomEditor(typeof(MapTool))]
    public class EditorMapTool : UnityEditor.Editor {
        private MapTool owner;

        public override void OnInspectorGUI(){
            base.OnInspectorGUI();
            owner = target as MapTool;
            if (GUILayout.Button(" LoadLevel")) {
                var grid = GameObject.FindObjectOfType<Grid>();
                if (grid == null)
                    return;
                MapManager.LoadMap(grid,owner.curLevel);
            }

            if (GUILayout.Button("SaveLevel")) {
                var grid = GameObject.FindObjectOfType<Grid>();
                if (grid == null)
                    return;
                MapManager.SaveLevel(grid,owner.curLevel);
                EditorUtility.DisplayDialog("提示", "Finish Save","OK");
            }
        }
    }
}