using Lockstep.Math;
using Entitas;


namespace Lockstep.ECS.Game
{

    [Game]
    public partial class BulletComponent : IComponent {
        
        public bool canDestoryIron = false;
        public bool canDestoryGrass = false;
        public uint ownerLocalId;
    }
}