using System;
using NetMsg.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Lockstep.Game.UI {
    /// <summary>
    ///     Represents a single row in the games list
    /// </summary>
    public class ListItemRoom : UIBaseItem {
        private GameObject LockImage => GetRef<GameObject>("LockImage");
        private Text TextMapName => GetRef<Text>("TextMapName");
        private Text TextRoomName => GetRef<Text>("TextRoomName");
        private Text TextPlayerCount => GetRef<Text>("TextPlayerCount");
        public RoomInfo RawData { get; protected set; }
        public int RoomId { get; private set; }

        public Image ImgBg;
        public Color DefaultBgColor;
        public Color SelectedBgColor;
        public string UnknownMapName = "Unknown";
        public Action<ListItemRoom> OnSelectCallback;

        public bool IsSelected;

        public bool IsPasswordProtected => false;
        ////RawData.IsPasswordProtected;

        // Use this for initialization
        protected override void DoAwake(){
            ImgBg = GetComponent<Image>(); 
            SelectedBgColor = BmHelper.SelectColor;
            DefaultBgColor = ImgBg.color;
            GetComponent<Button>().onClick.AddListener(OnClick);
            SetIsSelected(false);
        }

        public void SetIsSelected(bool isSelected){
            IsSelected = isSelected;
            ImgBg.color = isSelected ? SelectedBgColor : DefaultBgColor;
        }

        public void Setup(RoomInfo data, bool isSelected = false){
            RawData = data;
            SetIsSelected(isSelected);
            TextRoomName.text = data.Name;
            RoomId = data.RoomId;
            LockImage.SetActive(data.State == 1 || data.CurPlayerCount >= data.MaxPlayerCount);

            TextPlayerCount.text = string.Format("{0}/{1}", data.CurPlayerCount, data.MaxPlayerCount);
            TextMapName.text = data.MapId.ToString();
        }

        private void OnClick(){
            OnSelectCallback?.Invoke(this);
        }
    }
}