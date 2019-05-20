using Entitas;
using Entitas.CodeGeneration.Attributes;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.ECS.GameState
{        
    [GameState]
    [Unique]
    public class WorldBoundComponent : IComponent {
        public LVector2 min;
        public LVector2 max;
    }
}