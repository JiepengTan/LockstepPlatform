using System.Collections.Generic;
using NetMsg.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Lockstep.Game.UI {
    public class UILobby : UIBaseWindow {
        private Button BtnLeave=> GetRef<Button>("BtnLeave");
        private Button BtnStartGame=> GetRef<Button>("BtnStartGame");
        private Button BtnReady=> GetRef<Button>("BtnReady");
        private LayoutGroup LayoutGroup => GetRef<LayoutGroup>("LayoutGroup");
        private GameObject ListItemPlayer => GetRef<GameObject>("ListItemPlayer");
        private GameObject ReadyTick => GetRef<GameObject>("ReadyTick");
        
        private GenericUIList<RoomPlayerInfo> _items;
        private bool _isReady = false;

        public override void DoAwake(){
            _items = new GenericUIList<RoomPlayerInfo>(ListItemPlayer, LayoutGroup);
            Setup(NetworkService.Instance.PlayerInfos);
        }
        private void OnEnable(){
            Setup(NetworkService.Instance.PlayerInfos);
        }


        void OnClick_BtnReady(){
            Debug.Log("OnClick_BtnReady");
            _isReady = !_isReady;
            ReadyTick.SetActive(_isReady);
            NetworkService.Instance.ReadyInRoom(_isReady);
        }

        void OnClick_BtnStartGame(){
            Debug.Log("OnClick_BtnStartGame");
            NetworkService.Instance.StartGame();
        }

        void OnClick_BtnLeave(){
            Debug.Log("OnClick_BtnLeave");
            NetworkService.Instance.LeaveRoom();
        }

        void OnEvent_OnPlayerJoinRoom(object param){
            Debug.Log("OnEvent_OnPlayerJoinRoom");
            Setup(NetworkService.Instance.PlayerInfos);
        }

        void OnEvent_OnPlayerLeaveRoom(object param){
            Debug.Log("OnEvent_OnPlayerLeaveRoom");
            Setup(NetworkService.Instance.PlayerInfos);
        }

        void OnEvent_OnPlayerReadyInRoom(object param){
            Debug.Log("OnEvent_OnPlayerReadyInRoom");
            Setup(NetworkService.Instance.PlayerInfos);
        }

        void OnEvent_OnLeaveRoom(object param){
            OpenWindow(UIDefine.UIRoomList);
            Close();
        }

        void OnEvent_OnConnectToGameServer(object param){
            OpenWindow(UIDefine.UILoading);
            Close();
        }

        public void Setup(IEnumerable<RoomPlayerInfo> data){
            _items?.Generate<ListItemPlayer>(data, (packet, item) => { item.Setup(packet); });
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