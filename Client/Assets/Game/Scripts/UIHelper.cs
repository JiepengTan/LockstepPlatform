using Entitas;
using Lockstep.Game;
using UnityEngine;
using UnityEngine.UI;

public class UIHelper : MonoBehaviour {
    public Text HashCodeText;
    public Text AgentCountText;

    public Text ConnectedText;
    public Text CurrentTickText;

    bool IsConnected => Main.Instance.networkMgr?.IsConnected ?? false;
    uint CurTick => Main.Instance.simulationMgr?.World?.Tick ?? 0;
    long HashCode => Main.Instance.contexts.gameState.hashCodeEntity?.hashCode?.value ?? 0;
    int AgentCount => Main.Instance.contexts.game.count;

    void Update(){
        var instance = Main.Instance;
        ConnectedText.text = $"IsConn: {IsConnected}";
        if (instance != null && IsConnected) {
            HashCodeText.text = "HashCode: " + HashCode;
            CurrentTickText.text = "CurrentTick: " + CurTick;
            AgentCountText.text = "Agents: " + AgentCount;
        }
    }
}