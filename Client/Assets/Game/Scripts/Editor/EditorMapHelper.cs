using System.IO;
using System.Security.Policy;
using Lockstep.Game;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Editor {
    [CustomEditor(typeof(MapHelper))]
    public class EditorMapHelper : UnityEditor.Editor {
        private MapHelper owner;

        public override void OnInspectorGUI(){
            base.OnInspectorGUI();
            owner = target as MapHelper;
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