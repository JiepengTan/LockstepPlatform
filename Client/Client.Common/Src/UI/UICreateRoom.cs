using System.Collections;
using System.Collections.Generic;
using NetMsg.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Lockstep.Game.UI {
    public class UICreateRoom : UIBaseWindow {
        public Dropdown MapSelect;
        public Dropdown MaxCountSelect;
        public Button btnCreate;
        public Text txtRoomName;

        protected virtual void Awake(){
            BindEvent("BtnCreate", OnBtn_Create);
            MapSelect = transform.Find("MapSelect").GetComponent<Dropdown>();
            MaxCountSelect = transform.Find("MaxCountSelect").GetComponent<Dropdown>();
            MapSelect.ClearOptions();
            MapSelect.AddOptions(MapNames);
            MaxCountSelect.ClearOptions();
            MaxCountSelect.AddOptions(MaxCounts);
            MapSelect.onValueChanged.AddListener(OnChangedMapId);
            MaxCountSelect.onValueChanged.AddListener(OnChangedMaxCount);
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

        void OnBtn_Create(){
            Debug.Log("hhe OnBtn_Create");
            NetworkManager.Instance.CreateRoom(_curMapIdx,txtRoomName.text,_curMaxCount);
        }
        void OnEvent_OnCreateRoom(object param){
            var info = param as RoomInfo;
            if (info != null) {
                OpenWindow(UIDefine.UILobby);
                Close();
            }
        }
        private string _curMapName = "";
        private int _curMaxCount = 1;
        private int _curMapIdx = 0;

        void OnChangedMapId(int idx){
            _curMapIdx = idx;
        }

        void OnChangedMaxCount(int idx){
            _curMaxCount = int.Parse(MaxCounts[idx]);
        }
    }
}