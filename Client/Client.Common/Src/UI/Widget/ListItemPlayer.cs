using Lockstep.Game.UI;
using NetMsg.Common;
using UnityEngine;
using UnityEngine.UI;


namespace Lockstep.Game.UI {
    public class ListItemPlayer : UIBaseItem {
        private Text TextName=> GetRef<Text>("TextName");
        public RoomPlayerInfo RawData { get; protected set; }
        public Image BgImage;
        private Color DefaultBgColor;
        public Color SelectedBgColor;
        public int GameId { get; private set; }
        public bool IsSelected { get; private set; }
        public bool IsReady { get; set; }

        protected override void Awake(){
            BgImage = GetComponent<Image>();
            DefaultBgColor = BgImage.color;
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
            BgImage.color = isReady ? SelectedBgColor : DefaultBgColor;
        }

    }
}