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
                LevelManager.LoadLevel(owner.curLevel);
            }

            if (GUILayout.Button("SaveLevel")) {
                LevelManager.SaveLevel(owner.curLevel);
                EditorUtility.DisplayDialog("提示", "Finish Save","OK");
            }
        }
    }
}