namespace Lockstep.ECS.ECDefine.UnsafeECS {
    public enum EEnemyType {
        None,
        GolemSmall,
        GolemBig,
        LavaGolem,
        RangedGolem
    }

    public enum EAttackType {
        Normal,
        Stomp
    }

    public enum EDamageType {
        Magical,
        Physical
    }

    public enum EProjectileType {
        Simple,
        Special
    }

    public enum ESpawnType {
        Player,
        Enemy,
        Powerup
    }
}