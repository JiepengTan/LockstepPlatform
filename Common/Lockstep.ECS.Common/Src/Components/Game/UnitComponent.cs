using Lockstep.Math;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using Lockstep.Game;


namespace Lockstep.ECS.Game
{
    [Game]
    public partial class UnitComponent : IComponent {
        public string name;
        public ECampType camp;
        public int detailType;
        public int health;
        public int damage;
        public uint killerLocalId;
    }
}