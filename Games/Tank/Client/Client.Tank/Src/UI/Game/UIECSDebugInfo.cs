using Entitas;
using Lockstep.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Lockstep.Game.UI {
    public class UIECSDebugInfo : MonoBehaviour {
        public Text HashCodeText;
        public Text AgentCountText;

        public Text ConnectedText;
        public Text CurrentTickText;

        bool IsConnected => NetworkService.Instance?.IsConnected ?? false;
        int CurTick => SimulationService.Instance?.World?.Tick ?? 0;
        long HashCode => Contexts.sharedInstance.gameState.hashCodeEntity?.hashCode?.value ?? 0;
        int AgentCount => Contexts.sharedInstance.game.count;

        void Update(){
            if (!GameConstStateService.Instance.IsPlaying) return;
            ConnectedText.text = $"IsConn: {IsConnected}";
            if (IsConnected) {
                HashCodeText.text = "HashCode: " + HashCode;
                CurrentTickText.text = "CurrentTick: " + CurTick;
                AgentCountText.text = "Agents: " + AgentCount;
            }
        }
    }
}