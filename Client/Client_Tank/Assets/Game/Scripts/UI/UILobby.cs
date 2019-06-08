using System.Collections.Generic;
using Barebones.Utils;
using NetMsg.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Lockstep.Game.UI {

    public class UILobby : UIBaseWindow {
        private Button BtnStartGame;
        private Button BtnReady;
        public ListItemPlayer itemRoomPrefab;
        public LayoutGroup LayoutGroup;
        private GenericUIList<RoomPlayerInfo> _items;
        private InputField chatFiled;

        protected virtual void Awake(){
            BtnStartGame = BindEvent("Inner/BottomRight/BtnStartGame", OnBtn_StartGame);
            BtnReady = BindEvent("Inner/BottomRight/BtnReady", OnBtn_Ready);
            BindEvent("Inner/BtnLeave", OnBtn_Leave);
            //chatFiled = transform.Find("Inner/Left/Chat/Bottom/InputFiled").GetComponent<InputField>();
            _items = new GenericUIList<RoomPlayerInfo>(itemRoomPrefab.gameObject, LayoutGroup);
            Setup(NetworkManager.Instance.PlayerInfos);
        }        
        
        private void OnEnable(){
            Setup(NetworkManager.Instance.PlayerInfos);
        }


        private bool isReady = false;

        void OnBtn_Ready(){
            Debug.Log("OnBtn_Ready");
            isReady = !isReady;
            BtnReady.transform.Find("Tick").gameObject.SetActive(isReady);
            NetworkManager.Instance.ReadyInRoom(isReady);
        }

        void OnBtn_StartGame(){
            Debug.Log("OnBtn_StartGame");
            NetworkManager.Instance.StartGame();
        }

        void OnBtn_Leave(){
            Debug.Log("OnBtn_Leave");
            NetworkManager.Instance.LeaveRoom();
        }

        void OnEvent_OnPlayerReadyInRoom(object param){
            Debug.Log("OnEvent_OnPlayerReadyInRoom");
            Setup(NetworkManager.Instance.PlayerInfos);
        }       
        void OnEvent_OnLeaveRoom(object param){
            OpenWindow(UIDefine.UIRoomList);
            Close();
        }
        void OnEvent_OnRoomGameBegin(object param){
            Close();
        }
        
        public void Setup(IEnumerable<RoomPlayerInfo> data){
            _items.Generate<ListItemPlayer>(data, (packet, item) => { item.Setup(packet); });
        }

        public ListItemPlayer GetSelectedItem(){
            return _items.FindObject<ListItemPlayer>(item => item.IsSelected);
        }

        public void Select(ListItemPlayer listItemRoom){
            _items.Iterate<ListItemPlayer>(
                item => { item.SetIsSelected(!item.IsSelected && (listItemRoom == item)); });
        }
    }
}