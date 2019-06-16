using System.Collections.Generic;
using NetMsg.Common;
using UnityEngine.UI;

namespace Lockstep.Game.UI {
    /// <summary>
    ///     Represents a basic view for login form
    /// </summary>
    public class UIRoomList : UIBaseWindow {
        private Button BtnJoinRoom;
        public ListItemRoom itemRoomPrefab;
        public LayoutGroup LayoutGroup;
        private GenericUIList<RoomInfo> _items;

        protected virtual void Awake(){
            BtnJoinRoom = BindEvent("Window/JoinRoom", OnBtn_JoinRoom);
            BindEvent("Window/CreateGame", OnBtn_CreateGame);
            BindEvent("Window/CreateLobby", OnBtn_CreateLobby);
            BindEvent("Window/Refresh", OnBtn_Refresh);
            _items = new GenericUIList<RoomInfo>(itemRoomPrefab.gameObject, LayoutGroup);
            Setup(GetService<NetworkManager>().RoomInfos);
        }

        void OnBtn_JoinRoom(){
            var selected = GetSelectedItem();
            if (selected == null)
                return;
            NetworkManager.Instance.JoinRoom(selected.RoomId);
        }

        void OnBtn_CreateGame(){
            OpenWindow(UIDefine.UICreateRoom);
        }

        void OnBtn_CreateLobby(){ }

        void OnBtn_Refresh(){
            NetworkManager.Instance.ReqRoomList(0);
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
            Setup(NetworkManager.Instance.RoomInfos);
            NetworkManager.Instance.ReqRoomList(0);
        }

        public void Setup(IEnumerable<RoomInfo> data){
            var roomId = int.MaxValue;
            var select = GetSelectedItem();
            if (select != null) {
                roomId = select.RoomId;
            }

            _items.Generate<ListItemRoom>(data, (packet, item) => { item.Setup(packet, packet.RoomId == roomId); });
            UpdateGameJoinButton();
        }

        private void UpdateGameJoinButton(){
            if (BtnJoinRoom != null) {
                BtnJoinRoom.interactable = GetSelectedItem() != null;
            }
        }

        public ListItemRoom GetSelectedItem(){
            return _items.FindObject<ListItemRoom>(item => item.IsSelected);
        }

        public void Select(ListItemRoom listItemRoom){
            _items.Iterate<ListItemRoom>(
                item => { item.SetIsSelected(!item.IsSelected && (listItemRoom == item)); });
            UpdateGameJoinButton();
        }
    }
}