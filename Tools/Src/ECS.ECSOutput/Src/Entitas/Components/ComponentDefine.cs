using Entitas.CodeGeneration.Attributes;  
using Lockstep.Math;  
using Entitas;
namespace Lockstep.ECS.Actor{
    [Actor]
    public partial class GameLocalIdComponent :IComponent {
        public uint value;
    }

    [Actor]
    public partial class BackupComponent :IComponent {
        public byte actorId;
        public int tick;
    }

    [Actor]
    public partial class IdComponent :IComponent {
        [PrimaryEntityIndex]public byte value;
    }

    [Actor]
    public partial class EntityCountComponent :IComponent {
        public uint value;
    }

}
namespace Lockstep.ECS.Debug{
    [Debugging]
    public partial class TickComponent :IComponent {
        public uint value;
    }

    [Debugging]
    public partial class HashCodeComponent :IComponent {
        public long value;
    }

}
namespace Lockstep.ECS.Game{
    [Game]
    [Event(EventTarget.Self)]
    public partial class DirComponent :IComponent {
        public Lockstep.ECS.EDir value;
    }

    [Game]
    public partial class DropRateComponent :IComponent {
        public LFloat value;
    }

    public partial class FireRequestComponent :IComponent {
    }

    [Game]
    public partial class ItemTypeComponent :IComponent {
        public Lockstep.ECS.EItemType type;
        public byte killerActorId;
    }

    [Game]
    public partial class MoveComponent :IComponent {
        public LFloat moveSpd;
        public LFloat maxMoveSpd;
        public bool isChangedDir;
    }

    public partial class MoveRequestComponent :IComponent {
        public Lockstep.ECS.EDir value;
    }

    [Game]
    [Event(EventTarget.Self)]
    public partial class PosComponent :IComponent {
        public LVector2 value;
    }

    [Game]
    public partial class DelayCallComponent :IComponent {
        public LFloat delayTimer;
        public int callBack;
    }

    [Game]
    public partial class TagBulletComponent :IComponent {
    }

    [Game]
    public partial class TagCampComponent :IComponent {
    }

    [Game]
    public partial class TagEnemyComponent :IComponent {
    }

    [Game]
    public partial class TagTankComponent :IComponent {
    }

    [Game]
    public partial class UnitComponent :IComponent {
        public string name;
        public Lockstep.ECS.ECampType camp;
        public int detailType;
        public int health;
        public int damage;
        public uint killerLocalId;
    }

    [Game]
    public partial class OwnerComponent :IComponent {
        public uint localId;
    }

    [Game]
    public partial class SkillComponent :IComponent {
        public LFloat cd;
        public LFloat cdTimer;
        public int bulletId;
        public bool isNeedFire;
    }

    [Game]
    public partial class ColliderComponent :IComponent {
        public LVector2 size;
        public LFloat radius;
    }

    [Game]
    public partial class AssetComponent :IComponent {
        public Lockstep.ECS.EAssetID assetId;
    }

    [Game]
    public partial class AIComponent :IComponent {
        public LFloat timer;
        public LFloat updateInterval;
        public LFloat fireRate;
    }

    [Actor]
    [Event(EventTarget.Self)]
    public partial class ScoreComponent :IComponent {
        public int value;
    }

    [Actor]
    [Event(EventTarget.Self)]
    public partial class LifeComponent :IComponent {
        public int value;
    }

    [Game]
    public partial class LocalIdComponent :IComponent {
        [PrimaryEntityIndex]public uint value;
    }

    public partial class DestroyedComponent :IComponent {
    }

    [Game]
    public partial class BackupComponent :IComponent {
        public uint localEntityId;
        public int tick;
    }

    [Game]
    public partial class ActorIdComponent :IComponent {
        public byte value;
    }

    [Game]
    public partial class BulletComponent :IComponent {
        public bool canDestoryIron;
        public bool canDestoryGrass;
        public uint ownerLocalId;
    }

}
namespace Lockstep.ECS.GameState{
    [GameState]
    [Unique]
    public partial class BackupCurFrameComponent :IComponent {
    }

    [GameState]
    [Unique]
    public partial class BeforeExecuteHashCodeComponent :IComponent {
        public long value;
    }

    [GameState]
    [Unique]
    public partial class TickComponent :IComponent {
        public int value;
    }

    [GameState]
    [Unique]
    public partial class HashCodeComponent :IComponent {
        public long value;
    }

}
namespace Lockstep.ECS.Input{
    [Input]
    public partial class MoveDirComponent :IComponent {
        public Lockstep.ECS.EDir value;
    }

    [Input]
    public partial class FireComponent :IComponent {
    }

    [Input]
    public partial class TickComponent :IComponent {
        public int value;
    }

    [Input]
    public partial class EntityConfigIdComponent :IComponent {
        public int value;
    }

    [Input]
    public partial class DestroyedComponent :IComponent {
    }

    [Input]
    public partial class ActorIdComponent :IComponent {
        public byte value;
    }

}
namespace Lockstep.ECS.Snapshot{
    [Snapshot]
    public partial class TickComponent :IComponent {
        [PrimaryEntityIndex]public int value;
    }

    [Snapshot]
    public partial class HashCodeComponent :IComponent {
        public long value;
    }

}
