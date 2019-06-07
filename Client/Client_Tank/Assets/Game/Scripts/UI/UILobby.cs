using System;
using System.Collections.Generic;
using Barebones.MasterServer;
using Barebones.Utils;
using Lockstep.Core;
using NetMsg.Common;

using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Lockstep.Game.UI {
    public class GamesListUiItem : MonoBehaviour {
        public bool IsSelected;
        public void SetIsSelected(bool _ss){ }

        public void Setup(object ob){ }
    }

    public class GameInfoPacket { }

    /// <summary>
    ///     Represents a basic view for login form
    /// </summary>
    public class UILobby : UIBaseWindow {
        protected virtual void Awake(){
            BindEvent("Window/JoinRoom", OnBtn_JoinRoom);
            BindEvent("Window/CreateGame", OnBtn_CreateGame);
            BindEvent("Window/CreateLobby", OnBtn_CreateLobby);
            BindEvent("Window/Refresh", OnBtn_Refresh);
        }

        void OnBtn_JoinRoom(){ }
        void OnBtn_CreateGame(){ }
        void OnBtn_CreateLobby(){ }
        void OnBtn_Refresh(){ }

        public GameObject CreateRoomWindow;

        public Button GameJoinButton;
        public GamesListUiItem ItemPrefab;
        public LayoutGroup LayoutGroup;

        private GenericUIList<GameInfoPacket> _items;

        // Use this for initialization
        protected virtual void sAwake(){
            _items = new GenericUIList<GameInfoPacket>(ItemPrefab.gameObject, LayoutGroup);
        }

        protected virtual void HandleRoomsShowEvent(object arg1, object arg2){
            gameObject.SetActive(true);
        }

        private void OnEnable(){
            RequestRooms();
        }

        protected void OnConnectedToMaster(){
            // Get rooms, if at the time of connecting the lobby is visible
            if (gameObject.activeSelf)
                RequestRooms();
        }

        public void Setup(IEnumerable<GameInfoPacket> data){
            _items.Generate<GamesListUiItem>(data, (packet, item) => { item.Setup(packet); });
            UpdateGameJoinButton();
        }

        private void UpdateGameJoinButton(){
            GameJoinButton.interactable = GetSelectedItem() != null;
        }

        public GamesListUiItem GetSelectedItem(){
            return _items.FindObject<GamesListUiItem>(item => item.IsSelected);
        }

        public void Select(GamesListUiItem gamesListItem){
            _items.Iterate<GamesListUiItem>(
                item => { item.SetIsSelected(!item.IsSelected && (gamesListItem == item)); });
            UpdateGameJoinButton();
        }

        public void OnRefreshClick(){
            RequestRooms();
        }

        public void OnJoinGameClick(){
            //var selected = GetSelectedItem();
//
            //if (selected == null)
            //    return;
//
            //if (selected.IsLobby) {
            //    OnJoinLobbyClick(selected.RawData);
            //    return;
            //}
//
            //if (selected.IsPasswordProtected) {
            //    // If room is password protected
            //    var dialogData = DialogBoxData
            //        .CreateTextInput("Room is password protected. Please enter the password to proceed",
            //            password => { Msf.Client.Rooms.GetAccess(selected.GameId, OnPassReceived, password); });
//
            //    if (!Msf.Events.Fire(Msf.EventNames.ShowDialogBox, dialogData)) {
            //        Logs.Error("Tried to show an input to enter room password, " +
            //                   "but nothing handled the event: " + Msf.EventNames.ShowDialogBox);
            //    }
//
            //    return;
            //}
//
            //// Room does not require a password
            //Msf.Client.Rooms.GetAccess(selected.GameId, OnPassReceived);
        }

        protected virtual void OnJoinLobbyClick(GameInfoPacket packet){
            //var loadingPromise = Msf.Events.FireWithPromise(Msf.EventNames.ShowLoading, "Joining lobby");
//
            //Msf.Client.Lobbies.JoinLobby(packet.Id, (lobby, error) => {
            //    loadingPromise.Finish();
//
            //    if (lobby == null) {
            //        Msf.Events.Fire(Msf.EventNames.ShowDialogBox, DialogBoxData.CreateError(error));
            //        return;
            //    }
//
            //    // Hide this window
            //    gameObject.SetActive(false);
//
            //    var lobbyUi = FindObjectOfType<LobbyUi>();
//
            //    if (lobbyUi == null && MsfUi.Instance != null) {
            //        // Try to get it through MsfUi
            //        lobbyUi = MsfUi.Instance.LobbyUi;
            //    }
//
            //    if (lobbyUi == null) {
            //        Logs.Error("Couldn't find appropriate UI element to display lobby data in the scene. " +
            //                   "Override OnJoinLobbyClick method, if you want to handle this differently");
            //        return;
            //    }
//
            //    lobbyUi.gameObject.SetActive(true);
            //    lobby.SetListener(lobbyUi);
            //});
        }

        //   protected virtual void OnPassReceived(RoomAccessPacket packet, string errorMessage){
        //       if (packet == null) {
        //           Msf.Events.Fire(Msf.EventNames.ShowDialogBox, DialogBoxData.CreateError(errorMessage));
        //           Logs.Error(errorMessage);
        //           return;
        //       }
//
        //       // Hope something handles the event
        //   }
//
        protected virtual void RequestRooms(){
            //       SendMessage(EMsgCL.C2L_ReqRoomList,);
        }

        public void OnCreateGameClick(){
            OpenWindow(UIDefine.UICreateRoom);
        }
    }
}