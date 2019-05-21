using Lockstep.Math;
using Entitas;
using Lockstep.Game;
using UnityEngine;

namespace Lockstep.ECS.Game
{
    [Game]
    [System.Serializable]
    public class BornPointComponent : IComponent {
        public LVector2 coord;
    }
}