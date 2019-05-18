using Lockstep.Math;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using Lockstep.Game;

namespace Lockstep.ECS.Game
{
    [Game]
    public partial class UnitComponent : IComponent {
        public int camp;
        public int detailType;
        public int health;
    }
}