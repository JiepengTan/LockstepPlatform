using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    public class PlayerView : MonoBehaviour {
        [SerializeField] private BasePlayer player;

        public void Init(BasePlayer player){
            this.player = player;
        }

        public void Update(){
            var _lastFrameTime = Simulation.lastFrameExcuteTimeStamp;
            var curTime = Time.timeSinceLevelLoad;
            var rate = LMath.Clamp01((curTime - _lastFrameTime).ToLFloat() / Simulation.FrameIntervalMs);
            var pos = LVector2.Lerp(player.LastFramePosition, player.Position, rate);
            var deg = player.Rotation;
            transform.position = new Vector3(pos.x.ToFloat(), pos.y.ToFloat(), 0);
            transform.localRotation = Quaternion.Euler(0, 0, deg);
        }
    }
}