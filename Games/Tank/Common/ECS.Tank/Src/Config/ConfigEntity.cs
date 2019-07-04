using Lockstep.ECS;
using Lockstep.ECS.Game;
using Lockstep.Serialization;

namespace Lockstep.Game {
   
    #region configs
    public partial class ConfigUnit : BaseEntitySetter {
        public AssetComponent asset = new AssetComponent();
        public DirComponent dir = new DirComponent();
        public PosComponent pos = new PosComponent();
        public ColliderComponent collider = new ColliderComponent();
    }

    public partial class ConfigItem : ConfigUnit {
        public ItemTypeComponent type = new ItemTypeComponent();
    }

    public partial class ConfigCamp : ConfigUnit {
        public UnitComponent unit = new UnitComponent();
        public TagCampComponent tagCamp = new TagCampComponent();
    }

    public partial class ConfigMover : ConfigUnit {
        public UnitComponent unit = new UnitComponent();

        //public ColliderComponent collider = new ColliderComponent();
        public MoveComponent move = new MoveComponent();
        //public PositionComponent position = new PositionComponent();
    }

    public partial class ConfigTank : ConfigMover {
        public SkillComponent skill = new SkillComponent();
        public TagTankComponent tagTank = new TagTankComponent();
    }

    public partial class ConfigPlayer : ConfigTank {
        ActorIdComponent actorId = new ActorIdComponent();
    }

    public partial class ConfigEnemy : ConfigTank {
        public AIComponent ai = new AIComponent();
        public TagEnemyComponent tagEnemy = new TagEnemyComponent();
        public MoveRequestComponent moveReq = new MoveRequestComponent();
        public DropRateComponent dropRate = new DropRateComponent();
    }

    public partial class ConfigBullet : ConfigMover {
        public OwnerComponent owner = new OwnerComponent();
        public BulletComponent bullet = new BulletComponent();
        public TagBulletComponent tagBullet = new TagBulletComponent();
    }

    [System.Serializable]
    public partial class ConfigBullet {
        public override void Serialize(Serializer writer){
            writer.Write(asset);
            writer.Write(bullet);
            writer.Write(collider);
            writer.Write(dir);
            writer.Write(move);
            writer.Write(owner);
            writer.Write(pos);
            writer.Write(tagBullet);
            writer.Write(unit);
        }

        public override void Deserialize(Deserializer reader){
            asset = reader.ReadRef(ref this.asset);
            bullet = reader.ReadRef(ref this.bullet);
            collider = reader.ReadRef(ref this.collider);
            dir = reader.ReadRef(ref this.dir);
            move = reader.ReadRef(ref this.move);
            owner = reader.ReadRef(ref this.owner);
            pos = reader.ReadRef(ref this.pos);
            tagBullet = reader.ReadRef(ref this.tagBullet);
            unit = reader.ReadRef(ref this.unit);
        }
    }


    [System.Serializable]
    public partial class ConfigCamp {
        public override void Serialize(Serializer writer){
            writer.Write(asset);
            writer.Write(collider);
            writer.Write(dir);
            writer.Write(pos);
            writer.Write(tagCamp);
            writer.Write(unit);
        }

        public override void Deserialize(Deserializer reader){
            asset = reader.ReadRef(ref this.asset);
            collider = reader.ReadRef(ref this.collider);
            dir = reader.ReadRef(ref this.dir);
            pos = reader.ReadRef(ref this.pos);
            tagCamp = reader.ReadRef(ref this.tagCamp);
            unit = reader.ReadRef(ref this.unit);
        }
    }


    [System.Serializable]
    public partial class ConfigEnemy {
        public override void Serialize(Serializer writer){
            writer.Write(ai);
            writer.Write(asset);
            writer.Write(collider);
            writer.Write(dir);
            writer.Write(dropRate);
            writer.Write(move);
            writer.Write(moveReq);
            writer.Write(pos);
            writer.Write(skill);
            writer.Write(tagEnemy);
            writer.Write(tagTank);
            writer.Write(unit);
        }

        public override void Deserialize(Deserializer reader){
            ai = reader.ReadRef(ref this.ai);
            asset = reader.ReadRef(ref this.asset);
            collider = reader.ReadRef(ref this.collider);
            dir = reader.ReadRef(ref this.dir);
            dropRate = reader.ReadRef(ref this.dropRate);
            move = reader.ReadRef(ref this.move);
            moveReq = reader.ReadRef(ref this.moveReq);
            pos = reader.ReadRef(ref this.pos);
            skill = reader.ReadRef(ref this.skill);
            tagEnemy = reader.ReadRef(ref this.tagEnemy);
            tagTank = reader.ReadRef(ref this.tagTank);
            unit = reader.ReadRef(ref this.unit);
        }
    }


    [System.Serializable]
    public partial class ConfigItem {
        public override void Serialize(Serializer writer){
            writer.Write(asset);
            writer.Write(collider);
            writer.Write(dir);
            writer.Write(pos);
            writer.Write(type);
        }

        public override void Deserialize(Deserializer reader){
            asset = reader.ReadRef(ref this.asset);
            collider = reader.ReadRef(ref this.collider);
            dir = reader.ReadRef(ref this.dir);
            pos = reader.ReadRef(ref this.pos);
            type = reader.ReadRef(ref this.type);
        }
    }


    [System.Serializable]
    public partial class ConfigMover {
        public override void Serialize(Serializer writer){
            writer.Write(asset);
            writer.Write(collider);
            writer.Write(dir);
            writer.Write(move);
            writer.Write(pos);
            writer.Write(unit);
        }

        public override void Deserialize(Deserializer reader){
            asset = reader.ReadRef(ref this.asset);
            collider = reader.ReadRef(ref this.collider);
            dir = reader.ReadRef(ref this.dir);
            move = reader.ReadRef(ref this.move);
            pos = reader.ReadRef(ref this.pos);
            unit = reader.ReadRef(ref this.unit);
        }
    }


    [System.Serializable]
    public partial class ConfigPlayer {
        public override void Serialize(Serializer writer){
            writer.Write(asset);
            writer.Write(collider);
            writer.Write(dir);
            writer.Write(move);
            writer.Write(pos);
            writer.Write(skill);
            writer.Write(tagTank);
            writer.Write(unit);
        }

        public override void Deserialize(Deserializer reader){
            asset = reader.ReadRef(ref this.asset);
            collider = reader.ReadRef(ref this.collider);
            dir = reader.ReadRef(ref this.dir);
            move = reader.ReadRef(ref this.move);
            pos = reader.ReadRef(ref this.pos);
            skill = reader.ReadRef(ref this.skill);
            tagTank = reader.ReadRef(ref this.tagTank);
            unit = reader.ReadRef(ref this.unit);
        }
    }


    [System.Serializable]
    public partial class ConfigTank {
        public override void Serialize(Serializer writer){
            writer.Write(asset);
            writer.Write(collider);
            writer.Write(dir);
            writer.Write(move);
            writer.Write(pos);
            writer.Write(skill);
            writer.Write(tagTank);
            writer.Write(unit);
        }

        public override void Deserialize(Deserializer reader){
            asset = reader.ReadRef(ref this.asset);
            collider = reader.ReadRef(ref this.collider);
            dir = reader.ReadRef(ref this.dir);
            move = reader.ReadRef(ref this.move);
            pos = reader.ReadRef(ref this.pos);
            skill = reader.ReadRef(ref this.skill);
            tagTank = reader.ReadRef(ref this.tagTank);
            unit = reader.ReadRef(ref this.unit);
        }
    }


    [System.Serializable]
    public partial class ConfigUnit {
        public override void Serialize(Serializer writer){
            writer.Write(asset);
            writer.Write(collider);
            writer.Write(dir);
            writer.Write(pos);
        }

        public override void Deserialize(Deserializer reader){
            asset = reader.ReadRef(ref this.asset);
            collider = reader.ReadRef(ref this.collider);
            dir = reader.ReadRef(ref this.dir);
            pos = reader.ReadRef(ref this.pos);
        }
    }

    #endregion
}