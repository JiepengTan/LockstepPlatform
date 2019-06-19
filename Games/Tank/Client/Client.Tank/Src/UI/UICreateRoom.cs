using System.Collections;
using System.Collections.Generic;
using NetMsg.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Lockstep.Game.UI {
    public class UICreateRoom : UIBaseWindow {
        private Dropdown DropMapId=> GetRef<Dropdown>("DropMapId");
        private Dropdown DropMaxCount=> GetRef<Dropdown>("DropMaxCount");
        private InputField InputRoomName=> GetRef<InputField>("InputRoomName");
        private Button BtnCreate=> GetRef<Button>("BtnCreate");

        private int _curMaxCount = 1;
        private int _curMapIdx = 0;

       
        public override void DoAwake(){
            DropMapId.ClearOptions();
            DropMapId.AddOptions(MapNames);
            DropMaxCount.ClearOptions();
            DropMaxCount.AddOptions(MaxCounts);
        }
        private List<string> MaxCounts {
            get {
                return new List<string>() {
                    "1",
                    "2",
                    "3",
                    "4",
                };
            }
        }

        private List<string> MapNames {
            get {
                return new List<string>() {
                    "Map1",
                    "Map2",
                    "Map3",
                    "Map4",
                };
            }
        }

        void OnClick_BtnCreate(){
            Debug.Log("hhe OnClick_BtnCreate");
            NetworkService.Instance.CreateRoom(_curMapIdx,InputRoomName.text,_curMaxCount);
        }
        void OnEvent_OnCreateRoom(object param){
            var info = param as RoomInfo;
            if (info != null) {
                OpenWindow(UIDefine.UILobby);
                Close();
            }
        }

        void OnSelect_DropMapId(int idx){
            _curMapIdx = idx;
        }

        void OnSelect_DropMaxCount(int idx){
            _curMaxCount = int.Parse(MaxCounts[idx]);
        }
    }
}