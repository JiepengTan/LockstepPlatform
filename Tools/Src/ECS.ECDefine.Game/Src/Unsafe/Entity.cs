using Lockstep.ECS.ECDefine.UnsafeECS;

namespace  Lockstep.ECS.ECDefine.UnsafeECS {
    [EntityCount(4)]
    public class Goblin : IEntity {
        public Transform2D Transform2D;
        public CollisionAgent CollisionAgent;
        public CharacterResources CharacterResources;
        public GoblinResources GoblinResources;
        public Prefab Prefab;
        public Score Score;
        public GoblinAnimationData GoblinAnimationData;


        public PlayerRef Player;
        public AssetRef<CharacterSpec> CharacterSpec;

        public bool IsDead;
        public float RespawnTimer;
        public float DeathTimer;
    }


    [EntityCount(30)]
    public class Enemy : IEntity {
        public CharacterResources CharacterResources;
        public Transform2D Transform2D;
        public CollisionAgent CollisionAgent;
        public Prefab Prefab;
        public FSM FSM;
        public CharacterAnimationData CharacterAnimationData;
        public AttackTimingInformation AttackTimingInformation;
        public NavMeshAgent NavMeshAgent;

        public EAttackType AttackType;
        public EEnemyType EnemyType;
        public AssetRef<CharacterSpec> CharacterSpec;

        public float NavMeshUpdateRatio;
        public float NavMeshTimeCounter;

        public float SlowDebuffTimer;
        public bool isHit;
        public bool isDead;
        public float onDeathTimer;
        public float attackTypeDecision;
        public float speedRandomizer;
    }


    [EntityCount(64)]
    public class Projectile : IEntity {
        public Prefab Prefab;
        public Transform2D Transform2D;
        public CollisionAgent CollisionAgent;
        public float Time;
        public float AuxiliarTimer;
        public float ProjectileAngle;
        public EntityRef ProjectileSource;
        public AssetRef<ProjectileSpec> ProjectileSpec;
    }
}