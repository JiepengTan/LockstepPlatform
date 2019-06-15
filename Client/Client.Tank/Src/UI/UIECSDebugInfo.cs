﻿using Entitas;
using Lockstep.Game;
using UnityEngine;
using UnityEngine.UI;

public class UIECSDebugInfo : MonoBehaviour {
    public Text HashCodeText;
    public Text AgentCountText;

    public Text ConnectedText;
    public Text CurrentTickText;

    bool IsConnected => GameMsgManager.Instance?.IsConnected ?? false;
    int CurTick => SimulationManager.Instance?.World?.Tick ?? 0;
    long HashCode => Contexts.sharedInstance.gameState.hashCodeEntity?.hashCode?.value ?? 0;
    int AgentCount => Contexts.sharedInstance.game.count;

    void Update(){
        if (!GameConstStateManager.Instance.IsPlaying) return;
        ConnectedText.text = $"IsConn: {IsConnected}";
        if ( IsConnected) {
            HashCodeText.text = "HashCode: " + HashCode;
            CurrentTickText.text = "CurrentTick: " + CurTick;
            AgentCountText.text = "Agents: " + AgentCount;
        }
    }
}