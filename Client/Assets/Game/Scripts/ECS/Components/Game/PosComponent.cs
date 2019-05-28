using Lockstep.Math;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Lockstep.ECS.Game
{
    [Game, Event(EventTarget.Self)]
    [System.Serializable]
    public class PosComponent : IComponent
    {
        public LVector2 value;
    }
}