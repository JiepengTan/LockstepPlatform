using Lockstep.ECS.ECDefine;

public enum EnemyType {
    None,
    GolemSmall,
    GolemBig,
    LavaGolem,
    RangedGolem
}

public enum AttackType {
    Normal,
    Stomp
}

[EntityCount(30)]
public class Enemy : IEntity {
    public CharacterResources CharacterResources;
    public Transform2D Transform2D;
    public DynamicBody DynamicBody;
    public Prefab Prefab;
    public FSM FSM;
    public CharacterAnimationData CharacterAnimationData;
    public AttackTimingInformation AttackTimingInformation;
    public NavMeshAgent NavMeshAgent;

    public AttackType AttackType;
    public EnemyType EnemyType;
    public AssetRef<CharacterSpec> CharacterSpec;

    public float NavMeshUpdateRatio;
    public float NavMeshTimeCounter;

    public float SlowDebuffTimer;
    public bool isHit;
    public bool isDead;
    public float onDeathTimer;
    public float attackTypeDecision;
    float speedRandomizer;
}

//TO REFACTOR
public class AttackTimingInformation : IComponent {
    public bool isAttacking;
    public float onAttackTimer;
    public bool attackAlreadyDamaged;
}