using Entitas;
using Lockstep.Game;
using UnityEngine;
using UnityEngine.UI;

public class UIECSDebugInfo : MonoBehaviour {
    public Text HashCodeText;
    public Text AgentCountText;

    public Text ConnectedText;
    public Text CurrentTickText;

    bool IsConnected => NetworkManager.Instance?.IsConnected ?? false;
    int CurTick => SimulationManager.Instance?.World?.Tick ?? 0;
    long HashCode => Main.Instance.contexts.gameState.hashCodeEntity?.hashCode?.value ?? 0;
    int AgentCount => Main.Instance.contexts.game.count;

    void Update(){
        if (!GameManager.Instance.IsPlaying) return;
        ConnectedText.text = $"IsConn: {IsConnected}";
        if ( IsConnected) {
            HashCodeText.text = "HashCode: " + HashCode;
            CurrentTickText.text = "CurrentTick: " + CurTick;
            AgentCountText.text = "Agents: " + AgentCount;
        }
    }
}