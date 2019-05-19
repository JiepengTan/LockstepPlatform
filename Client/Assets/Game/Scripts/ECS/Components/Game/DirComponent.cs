using Lockstep.Math;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using Lockstep.Game;

namespace Lockstep.ECS.Game
{
    [Game, Event(true)]
    [System.Serializable]
    public class DirComponent : IComponent
    {
        public EDir value;
    }
}