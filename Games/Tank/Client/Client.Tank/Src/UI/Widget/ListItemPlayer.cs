using Lockstep.Game.UI;
using NetMsg.Common;
using UnityEngine;
using UnityEngine.UI;


namespace Lockstep.Game.UI {
    public class ListItemPlayer : UIBaseItem {
        private Text TextName=> GetRef<Text>("TextName");
        private Image ImgReady=> GetRef<Image>("ImgReady");
        public RoomPlayerInfo RawData { get; protected set; }
        private Color DefaultBgColor;
        public Color SelectedBgColor;
        public int GameId { get; private set; }
        public bool IsSelected { get; private set; }
        public bool IsReady { get; set; }

        protected override void DoAwake(){
            DefaultBgColor = ImgReady.color;
            SelectedBgColor = BmHelper.SelectColor;
            SetIsSelected(false);
        }

        public void SetIsSelected(bool isSelected){
            IsSelected = isSelected;
        }

        public void Setup(RoomPlayerInfo data){
            RawData = data;
            SetIsSelected(false);
            TextName.text = data.Name;
            SetReady(RawData.Status == 1);
        }

        public void SetReady(bool isReady){
            ImgReady.color = isReady ? SelectedBgColor : DefaultBgColor;
        }

    }
}