using Lockstep.ECS.ECDefine;

public class SpawnersData : IAsset { }

public enum SpawnType {
    Player,
    Enemy,
    Powerup
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

public class CharacterSpawn : IEvent {
    public EntityRef SpawnedEntity;
    public Vector2 Position;
    public bool IsPlayer;
}

//Every type of spawner have a limited number of instances. They are separated to simplify manegement at the spawn systems
public partial class Global : IGlobal {
    [EntityCount(4)] public Spawner[] PlayerSpawners;
    [EntityCount(30)] public Spawner[] EnemySpawners;
}