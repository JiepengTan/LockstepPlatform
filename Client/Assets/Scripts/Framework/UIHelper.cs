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
        ConnectedText.text = "Connected: " + instance.IsConnected;
        if (instance != null && instance.IsConnected) {
            HashCodeText.text = "HashCode: " + instance.HashCode;
            CurrentTickText.text = "CurrentTick: " + instance.CurTick;
            AgentCountText.text = "Agents: " + instance.AgentCount;
        }
    }
}