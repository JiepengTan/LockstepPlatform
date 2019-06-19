using System.Collections.Generic;
using NetMsg.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Lockstep.Game.UI {
    /// <summary>
    ///     Represents a basic view for login form
    /// </summary>
    public class UIRoomList : UIBaseWindow {
        private Button BtnJoinRoom => GetRef<Button>("BtnJoinRoom");
        private Button BtnCreateGame => GetRef<Button>("BtnCreateGame");
        private Button BtnCreateLobby => GetRef<Button>("BtnCreateLobby");
        private Button BtnRefresh => GetRef<Button>("BtnRefresh");
        private LayoutGroup LayoutGroup => GetRef<LayoutGroup>("LayoutGroup");
        private GameObject ListItemRoom => GetRef<GameObject>("ListItemRoom");

        private GenericUIList<RoomInfo> _items;


        public override void DoAwake(){
            _items = new GenericUIList<RoomInfo>(ListItemRoom, LayoutGroup);
            var service = GetService<INetworkService>();
            var infos = (service as NetworkService)?.RoomInfos;
            Setup(infos);
#if UNITY_EDITOR
            if (_uiService.IsDebugMode &&  infos!= null ) {
                _items.GetObjectAt(0).GetComponent<ListItemRoom>().SetIsSelected(true);
                OnClick_BtnJoinRoom();
            }
#endif
        }
    
        void OnClick_BtnJoinRoom(){
            var selected = GetSelectedItem();
            if (selected == null)
                return;
            NetworkService.Instance.JoinRoom(selected.RoomId);
        }

        void OnClick_BtnCreateGame(){
            OpenWindow(UIDefine.UICreateRoom);
        }

        void OnClick_BtnCreateLobby(){ }

        void OnClick_BtnRefresh(){
            NetworkService.Instance.ReqRoomList(0);
        }

        protected void OnEvent_OnRoomInfoUpdate(object param){
            var info = param as RoomInfo[];
            Setup(info ?? new RoomInfo[0]);
        }

        protected void OnEvent_OnCreateRoom(object param){
            var info = param as RoomInfo;
            if (info != null) {
                Close();
            }
        }

        protected void OnEvent_OnJoinRoomResult(object param){
            if (param is RoomPlayerInfo[] playerInfos) {
                OpenWindow(UIDefine.UILobby);
                Close();
            }
        }

        private void OnEnable(){
            if (_items == null) return;
            Setup(NetworkService.Instance.RoomInfos);
            NetworkService.Instance.ReqRoomList(0);
        }

        public void Setup(IEnumerable<RoomInfo> data){
            var roomId = int.MaxValue;
            var select = GetSelectedItem();
            if (select != null) {
                roomId = select.RoomId;
            }

            if (data != null) {
                _items.Generate<ListItemRoom>(data, (packet, item) => {
                    item.OnSelectCallback = Select;
                    item.Setup(packet, packet.RoomId == roomId);
                });
            }
            UpdateGameJoinButton();
        }

        private void UpdateGameJoinButton(){
            if (BtnJoinRoom != null) {
                BtnJoinRoom.interactable = GetSelectedItem() != null;
            }
        }

        public ListItemRoom GetSelectedItem(){
            return _items.FindObject<ListItemRoom>(item => item != null && item.IsSelected);
        }

        public void Select(ListItemRoom listItemRoom){
            _items.Iterate<ListItemRoom>(
                item => { item.SetIsSelected(!item.IsSelected && (listItemRoom == item)); });
            UpdateGameJoinButton();
        }
    }
}