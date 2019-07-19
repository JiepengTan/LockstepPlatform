namespace Lockstep.ECS.ECDefine {
    [Abstract]
    public class AssetRef<T> : IRef where T : IAsset { }

    [Abstract]
    public class EntityRef<T> : IRef where T : IEntity { }

    public class EntityRef : IRef { }

    public class PlayerRef : IRef { }

    public class Vector2 { }

    public class Vector3 { }

    public class Quaternion { }

    public class Prefab { }

    public class Transform2D { }

    public class CollisionAgent { }

    public class NavMeshAgent { }

    public class Button { }
}