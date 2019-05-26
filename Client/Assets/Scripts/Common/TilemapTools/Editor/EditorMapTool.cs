using System.IO;
using Lockstep.Game;
using Lockstep.Serialization;
using NetMsg.Game;
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
            ShowLoadRecord();
            ShowRecordInfo();
        }

        public void ShowRecordInfo(){
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

        private void ShowLoadRecord(){
            if (GUILayout.Button("LoadGameRecord")) {
                var path = EditorUtility.OpenFilePanelWithFilters("SelectGameRecord",
                    Path.Combine(Application.dataPath, "../../Record"), new[] {"*.record"});
                if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
                    OpenRecordFile(path);
                }
            }
        }

        void OpenRecordFile(string path){
            var bytes = File.ReadAllBytes(path);
            var reader = new Deserializer(bytes);
          var TypeId =   reader.GetInt();
          var RoomId =  reader.GetInt();
          var seed =   reader.GetInt();
          var allLocalId =  reader.GetBytes_255();
          
          var msg = new Msg_RepMissFrame();
          msg.startTick = 0;
          msg.Deserialize(reader);
          
        }

        void StartGame(Msg_RepMissFrame allFrames,int typeid,int roomid,int seed,byte[] allActorIds){
            
            
        }
    }
}