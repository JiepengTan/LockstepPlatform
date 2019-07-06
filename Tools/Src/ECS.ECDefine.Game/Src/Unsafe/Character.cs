using Lockstep.ECS.ECDefine;

public class CharacterSpec : IAsset { }

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

//Used for both the Goblin or the Enemy to perform an attack so the Unity-side may know of that moment and activate animations, SFX, etc
public class CharacterAttack : IEvent {
    EntityRef Character;
    ProjectileType ProjectileType;
}