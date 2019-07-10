using Lockstep.ECS.ECDefine;
using Lockstep.Game;

namespace Lockstep.ECS.Actor {
    [NeedAttribute("Actor")]
    //An ActorEntity with BackupComponent refers to an actor in the past
    public partial class BackupComponent : IComponent {
        public byte actorId;

        public int tick;
    }
}


namespace Lockstep.ECS.Actor {
    [NeedAttribute("Actor")]
    public partial class EntityCountComponent : IComponent {
        public uint value;
    }
}


namespace Lockstep.ECS.Actor {
    /// <summary>
    /// 标志 当前Entity 是当前Tick 中存在  非Backup
    /// </summary>
    [NeedAttribute("Actor")]
    public partial class IdComponent : IComponent {
        [NeedAttribute("PrimaryEntityIndex")] public byte value;
    }
}


namespace Lockstep.ECS.Debug {
    [NeedAttribute("Debugging")]
    public partial class HashCodeComponent : IComponent {
        public long value;
    }
}


namespace Lockstep.ECS.Debug {
    [NeedAttribute("Debugging")]
    public partial class TickComponent : IComponent {
        public uint value;
    }
}


namespace Lockstep.ECS.Game {
    [NeedAttribute("Game")]
    public partial class ActorIdComponent : IComponent {
        public byte value;
    }
}


namespace Lockstep.ECS.Game {
    [NeedAttribute("Game")]
    //A GameEntity with BackupComponent refers to an entity in the past
    public partial class BackupComponent : IComponent {
        public uint localEntityId;

        public int tick;
    }
}


namespace Lockstep.ECS.Game {
    public partial class DestroyedComponent : IComponent { }
}


namespace Lockstep.ECS.Game {
    /// <summary>
    /// 标志 当前Entity 是当前Tick 中存在 非Backup
    /// </summary>
    [NeedAttribute("Game")]
    public partial class LocalIdComponent : IComponent {
        [NeedAttribute("PrimaryEntityIndex")] public uint value;
    }
}


namespace Lockstep.ECS.GameState {
    [NeedAttribute("GameState")]
    [NeedAttribute("Unique")]
    public partial class BackupCurFrameComponent : IComponent { }
}


namespace Lockstep.ECS.GameState {
    /// <summary>
    /// 执行前的hash
    /// </summary>
    [NeedAttribute("GameState")]
    [NeedAttribute("Unique")]
    public partial class BeforeExecuteHashCodeComponent : IComponent {
        public long value;
    }

    /// <summary>
    /// 执行后的hash
    /// </summary>
    [NeedAttribute("GameState")]
    [NeedAttribute("Unique")]
    public partial class HashCodeComponent : IComponent {
        public long value;
    }
}


namespace Lockstep.ECS.GameState {
    [NeedAttribute("GameState")]
    [NeedAttribute("Unique")]
    public partial class TickComponent : IComponent {
        public int value;
    }
}


namespace Lockstep.ECS.Input {
    [NeedAttribute("Input")]
    public partial class ActorIdComponent : IComponent {
        public byte value;
    }
}


namespace Lockstep.ECS.Input {
    [NeedAttribute("Input")]
    public partial class DestroyedComponent : IComponent { }
}


namespace Lockstep.ECS.Input {
    [NeedAttribute("Input")]
    public partial class EntityConfigIdComponent : IComponent {
        public int value;
    }
}


namespace Lockstep.ECS.Input {
    [NeedAttribute("Input")]
    public partial class TickComponent : IComponent {
        public int value;
    }
}


namespace Lockstep.ECS.Snapshot {
    [NeedAttribute("Snapshot")]
    public partial class HashCodeComponent : IComponent {
        public long value;
    }
}


namespace Lockstep.ECS.Snapshot {
    [NeedAttribute("Snapshot")]
    public partial class TickComponent : IComponent {
        [NeedAttribute("PrimaryEntityIndex")] public int value;
    }
}


namespace Lockstep.ECS.Actor {
    [NeedAttribute("Actor")]
    public partial class GameLocalIdComponent : IComponent {
        public uint value;
    }
}


namespace Lockstep.ECS.Game {
    [NeedAttribute("Actor")]
    [NeedAttribute("Event(EventTarget.Self)")]
    public partial class LifeComponent : IComponent {
        public int value;
    }
}


namespace Lockstep.ECS.Game {
    [NeedAttribute("Actor")]
    [NeedAttribute("Event(EventTarget.Self)")]
    public partial class ScoreComponent : IComponent {
        public int value;
    }
}


namespace Lockstep.ECS.Game {
    [NeedAttribute("Game")]
    public partial class AIComponent : IComponent {
        public float timer;
        public float updateInterval;
        public float fireRate;
    }
}


namespace Lockstep.ECS.Game {
    [NeedAttribute("Game")]
    public partial class AssetComponent : IComponent {
        public EAssetID assetId = EAssetID.Bullet0;
    }
}


namespace Lockstep.ECS.Game {
    //[Attribute("Game")]
    //public partial class BornPointComponent : IComponent {
    //    public Vector2 coord;
    //}
}


namespace Lockstep.ECS.Game {
    [NeedAttribute("Game")]
    public partial class BulletComponent : IComponent {
        public bool canDestoryIron = false;
        public bool canDestoryGrass = false;
        public uint ownerLocalId;
    }
}


namespace Lockstep.ECS.Game {
    [NeedAttribute("Game")]
    public partial class ColliderComponent : IComponent {
        public Vector2 size;
        public float radius;
    }
}


namespace Lockstep.ECS.Game {
    [NeedAttribute("Game")]
    public partial class DelayCallComponent : IComponent {
        public float delayTimer;
        
        [NeedAttribute("NoGenCode")]
        public System.Action callBack;
    }
}


namespace Lockstep.ECS.Game {
    [NeedAttribute("Game")]
    [NeedAttribute("Event(EventTarget.Self)")]
    public partial class DirComponent : IComponent {
        public EDir value;
    }
}


namespace Lockstep.ECS.Game {
    [NeedAttribute("Game")]
    public partial class DropRateComponent : IComponent {
        public float value;
    }
}


namespace Lockstep.ECS.Game {
    public partial class FireRequestComponent : IComponent { }
}


namespace Lockstep.ECS.Game {

    [NeedAttribute("Game")]
    public partial class ItemTypeComponent : IComponent {
        public EItemType type;
        public byte killerActorId;
    }
}


namespace Lockstep.ECS.Game {
    [NeedAttribute("Game")]
    public partial class MoveComponent : IComponent {
        public float moveSpd;
        public float maxMoveSpd;
        public bool isChangedDir;
    }
}


namespace Lockstep.ECS.Game {
    public partial class MoveRequestComponent : IComponent {
        public EDir value;
    }
}


namespace Lockstep.ECS.Game {
    [NeedAttribute("Game")]
    public partial class OwnerComponent : IComponent {
        public uint localId;
    }
}


namespace Lockstep.ECS.Game {
    [NeedAttribute("Game")]
    [NeedAttribute("Event(EventTarget.Self)")]
    public partial class PosComponent : IComponent {
        public Vector2 value;
    }
}


namespace Lockstep.ECS.Game {
    [NeedAttribute("Game")]
    public partial class SkillComponent : IComponent {
        public float cd;

        /// <=0 表示cd 冷却
        public float cdTimer;

        public int bulletId;
        public bool isNeedFire;
    }
}


namespace Lockstep.ECS.Game {
    [NeedAttribute("Game")]
    public partial class TagBulletComponent : IComponent { }
}


namespace Lockstep.ECS.Game {
    [NeedAttribute("Game")]
    public partial class TagCampComponent : IComponent { }
}


namespace Lockstep.ECS.Game {
    [NeedAttribute("Game")]
    public partial class TagEnemyComponent : IComponent { }
}


namespace Lockstep.ECS.Game {
    [NeedAttribute("Game")]
    public partial class TagTankComponent : IComponent { }
}


namespace Lockstep.ECS.Game {
    [NeedAttribute("Game")]
    public partial class UnitComponent : IComponent {
        public string name;
        public ECampType camp;
        public int detailType;
        public int health;
        public int damage;
        public uint killerLocalId;
    }
}


namespace Lockstep.ECS.Input {
    [NeedAttribute("Input")]
    public partial class FireComponent : IComponent { }
}


namespace Lockstep.ECS.Input {
    [NeedAttribute("Input")]
    public partial class MoveDirComponent : IComponent {
        public EDir value;
    }
}