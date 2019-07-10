using Lockstep.ECS.ECDefine;

namespace  Lockstep.ECS.ECDefine.UnsafeECS {


    //Used for both the Goblin or the Enemy to perform an attack so the Unity-side may know of that moment and activate animations, SFX, etc
    public class CharacterAttack : IEvent {
        public EntityRef Character;
        public EProjectileType ProjectileType;
    }

    //activate damage VFX/SFX on the position of the Character that has been damaged
    public class CharacterDamage : IEvent {
        public EntityRef Character;
    }

    //activate death VFX/SFX on the position of the Character that has died
    public class CharacterDead : IEvent {
        public EntityRef Character;
    }

    [Abstract]
    public class GoblinEvent : IEvent {
        public EntityRef<Goblin> Goblin;
    }

    public class GoblinDamage : GoblinEvent {
        public float Damage;
        //nothashed Vector2 Position;
    }

    public class GoblinAttack : GoblinEvent { }

    public class GoblinHeal : GoblinEvent { }

    public class HitEnv : IEvent {
        public Vector2 Position;
        public float Rotation;
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


    public class CharacterSpawn : IEvent {
        public EntityRef SpawnedEntity;
        public Vector2 Position;
        public bool IsPlayer;
    }
}