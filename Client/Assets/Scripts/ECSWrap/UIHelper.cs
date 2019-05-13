using Entitas;
using Lockstep.Game;
using UnityEngine;
using UnityEngine.UI;

public class UIHelper : MonoBehaviour {
    public Text HashCodeText;
    public Text AgentCountText;

    public Text ConnectedText;
    public Text CurrentTickText;

    void Update(){
        var instance = Main.Instance;
        if (instance != null) {
            HashCodeText.text = "HashCode: " + instance.HashCode;
            CurrentTickText.text = "CurrentTick: " + instance.CurTick;
            AgentCountText.text = "Agents: " + instance.AgentCount;
            ConnectedText.text = "Connected: " + instance.IsConnected;
        }
    }
}