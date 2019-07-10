using Lockstep.Math;

namespace Lockstep.Game  {
    public interface IComponent { }

    public interface IEntity { }

    public interface IAsset { }
    
    public class AssetRef<T> where T:IAsset{ }

    public class EntityRef<T> where T:IEntity { }

    public class EntityRef { }
    public class PlayerRef { }

    public class Prefab { }
    
    public class Transform2D {
        public LVector2 pos;
        public LFloat deg;
    }
    
    public class CollisionAgent { }
    public class NavMeshAgent { }
    public class Button { }
}