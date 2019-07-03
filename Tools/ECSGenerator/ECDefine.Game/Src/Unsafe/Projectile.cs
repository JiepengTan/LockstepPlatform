using Lockstep.ECS.ECDefine;

public class ProjectileSpec : IAsset { }

public enum ProjectileType {
    Simple,
    Special
}

[EntityCount(64)]
public class Projectile : IEntity {
    public Prefab Prefab;
    public Transform2D Transform2D;
    public DynamicBody DynamicBody;
    public float Time;
    public float AuxiliarTimer;
    public float ProjectileAngle;
    public EntityRef ProjectileSource;
    public AssetRef<ProjectileSpec> ProjectileSpec;
}

//Where the collision occurred so the VFX/SFX can be instantiated
//The ProjectileSpec is needed because it points to the VFX prefab that is instantiated
public class OnProjectileCollision : IEvent {
    public Vector2 CollisionPosition;
    public float Rotation;
    public AssetRef<ProjectileSpec> ProjectileSpec;
}

public class OnProjectileCollisionDynamic : IEvent {
    public Vector2 CollisionPosition;
    public float Rotation;
    public AssetRef<ProjectileSpec> ProjectileSpec;
    public EntityRef TargeteRef;
    public EntityRef SourceRef;
}

//who has cast the projectile (so it doesn't deal damage to itself), forward direction,
//angle of the projectile and the SpellType, so the correct projectile can be created (the simple one, or the special one)
public partial class SignalDefine {
    [Signal]
    void OnCastProjectile(EntityRef projectileSource, Vector2 forward, Vector2 right, float
        projectileAngle, ProjectileType projectileType, ProjectileSpec projectileSpec){ }
}

//NetCoding type-safe collision
public partial class SignalDefine {
    [Signal]
    void collision(ref Projectile projectile, ref Goblin goblin){ }
}

public partial class SignalDefine {
    [Signal]
    void collision(ref Projectile projectile, ref Enemy enemy){ }
}

public partial class SignalDefine {
    [Signal]
    void collision(ref Projectile projectile){ }
}