using Lockstep.ECS.ECDefine;
namespace  Lockstep.ECS.ECDefine.UnsafeECS {

    //Every type of spawner have a limited number of instances. They are separated to simplify manegement at the spawn systems
    public partial class Global : IGlobal {
        [EntityCount(4)] public Spawner[] PlayerSpawners;
        [EntityCount(30)] public Spawner[] EnemySpawners;
    }
}