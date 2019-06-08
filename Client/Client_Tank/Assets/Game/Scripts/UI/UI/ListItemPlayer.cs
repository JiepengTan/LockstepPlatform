using Lockstep.Game.UI;
using NetMsg.Common;
using UnityEngine;
using UnityEngine.UI;


public class ListItemPlayer : MonoBehaviour {
    public RoomPlayerInfo RawData { get; protected set; }
    public Image BgImage;
    public UILobby ListView;
    public Text TextName;
    private Color DefaultBgColor;
    public Color SelectedBgColor;
    public int GameId { get; private set; }
    public bool IsSelected { get; private set; }
    public bool IsReady { get; set; }

    private void Awake(){
        BgImage = GetComponent<Image>();
        DefaultBgColor = BgImage.color;
        //GetComponent<Button>().onClick.AddListener(OnClick);
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

    private void OnClick(){
        ListView.Select(this);
    }
}
