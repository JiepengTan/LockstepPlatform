using Lockstep.Math;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using Lockstep.Game;


namespace Lockstep.ECS.Game
{
    [Game, Event(EventTarget.Self)]
    public partial class DirComponent : IComponent
    {
        public EDir value;
    }
}