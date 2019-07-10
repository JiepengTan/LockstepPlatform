using Lockstep.ECS.ECDefine;

namespace  Lockstep.ECS.ECDefine.UnsafeECS {
    //TO REFACTOR
    public class AttackTimingInformation : IComponent {
        public bool isAttacking;
        public float onAttackTimer;
        public bool attackAlreadyDamaged;
    }

    //Each single enemy will have its own FSM component
    //This way, every enemy can have its own InitialState informing "how this enemy begins to act"
    //and can have its CurrentState informing "how is this enemy acting right now"
    public class FSM : IComponent {
        AssetRef<State> InitialState;
        AssetRef<State> CurrentState;
    }

    public class GoblinResources : IComponent {
        public float CooldownHeal;
        public float onHealTimer;
        public float onAttackTimer;
    }

    public class Input : IComponent {
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


    //Every character has at least HP
    public class CharacterResources : IComponent {
        public float Health;
        public float CoolDownAttack;
    }

    //Used at the Unity-side to perform animations
    public class CharacterAnimationData : IComponent {
        public float Speed;
        public float AnimationSpeed;
        public bool Walking;
    }


    public class DamageStructure : IComponent {
        public EntityRef Target;
        public EntityRef CharacterAttacker;
        public float Value;
    }

    public class Damage : IComponent {
        public EntityRef<Goblin> Goblin;
        public EDamageType Type;
        public float Value;
    }

    public class Score : IComponent {
        public float GolemSmall;
        public float RangedGolem;
        public float LavaGolem;
        public float Total;
    }


    public class Spawner : IComponent {
        public bool IsActive;

        public AssetRef<CharacterSpec> EnemySpec;
        public AssetRef<State> InitialEnemyState;

        public float CurrentSpawnNumber;

        public float SpawnTimer;

        //The next spawn will only happen when this entity_ref is gone
        public EntityRef SpawnedEntity;
    }
}