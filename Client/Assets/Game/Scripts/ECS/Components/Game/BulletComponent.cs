using Lockstep.Math;
using Entitas;
using Lockstep.Game;

namespace Lockstep.ECS.Game
{

    [Game]
    [System.Serializable]
    public class BulletComponent : IComponent {
        
        public bool canDestoryIron = false;
        public bool canDestoryGrass = false;
        public uint ownerLocalId;
    }
}