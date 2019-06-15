using NetMsg.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Lockstep.Game.UI
{
    /// <summary>
    ///     Represents a single row in the games list
    /// </summary>
    public class ListItemRoom : MonoBehaviour
    {
        public RoomInfo RawData { get; protected set; }
        public Image BgImage;
        public Color DefaultBgColor;
        public UIRoomList ListView;
        public GameObject LockImage;
        public Text MapName;
        public Text RoomName;
        public Text Online;

        public Color SelectedBgColor;

        public string UnknownMapName = "Unknown";

        public int RoomId { get; private set; }
        public bool IsSelected;

        public bool IsPasswordProtected => false;
        ////RawData.IsPasswordProtected;

        // Use this for initialization
        private void Awake()
        {
            BgImage = GetComponent<Image>();
            DefaultBgColor = BgImage.color;
            GetComponent<Button>().onClick.AddListener(OnClick);
            SetIsSelected(false);
        }

        public void SetIsSelected(bool isSelected)
        {
            IsSelected = isSelected;
            BgImage.color = isSelected ? SelectedBgColor : DefaultBgColor;
        }
        
        public void Setup(RoomInfo data,bool isSelected = false)
        {
             RawData = data;
             SetIsSelected(isSelected);
             RoomName.text = data.Name;
             RoomId = data.RoomId;
             LockImage.SetActive(data.State == 1 || data.CurPlayerCount >= data.MaxPlayerCount);

             Online.text = string.Format("{0}/{1}", data.CurPlayerCount, data.MaxPlayerCount);
             MapName.text = data.MapId.ToString();
        }

        private void OnClick()
        {
            ListView.Select(this);
        }
    }
}