using Lockstep.ECS.ECDefine;

[EntityCount(4)]
public class Goblin : IEntity {
    public Transform2D Transform2D;
    public DynamicBody DynamicBody;
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

public class GoblinResources : IComponent {
    public float CooldownHeal;
    public float onHealTimer;
    public float onAttackTimer;
}

public class input : IComponent {
    public Vector2 Movement;
    public float GoblinAngle;
    public Button Fire;
    public Button Defend;
    public Button Heal;
    public Vector2 MousePosition;
}

public class GoblinAnimationData : IComponent {
    public bool Walking;
    public bool Defend;
    public bool Dead;
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

public enum DamageType {
    Magical,
    Physical
}

public class Damage : IComponent {
    public EntityRef<Goblin> Goblin;
    public DamageType Type;
    public float Value;
}

public class Score : IComponent {
    public float GolemSmall;
    public float RangedGolem;
    public float LavaGolem;
    public float Total;
}