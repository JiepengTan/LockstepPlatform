using System;
using System.Collections.Generic;
using System.IO;
using Lockstep.ECS;
using Lockstep.ECS.Game;
using Lockstep.Serialization;
using UnityEngine;

namespace Lockstep.Game {

#region ss
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
                writer.Put(asset);
                writer.Put(bullet);
                writer.Put(collider);
                writer.Put(dir);
                writer.Put(move);
                writer.Put(owner);
                writer.Put(pos);
                writer.Put(tagBullet);
                writer.Put(unit);
            }

            public override void Deserialize(Deserializer reader){
                asset = reader.Get(ref this.asset);
                bullet = reader.Get(ref this.bullet);
                collider = reader.Get(ref this.collider);
                dir = reader.Get(ref this.dir);
                move = reader.Get(ref this.move);
                owner = reader.Get(ref this.owner);
                pos = reader.Get(ref this.pos);
                tagBullet = reader.Get(ref this.tagBullet);
                unit = reader.Get(ref this.unit);
            }
        }


        [System.Serializable]
        public partial class ConfigCamp {
            public override void Serialize(Serializer writer){
                writer.Put(asset);
                writer.Put(collider);
                writer.Put(dir);
                writer.Put(pos);
                writer.Put(tagCamp);
                writer.Put(unit);
            }

            public override void Deserialize(Deserializer reader){
                asset = reader.Get(ref this.asset);
                collider = reader.Get(ref this.collider);
                dir = reader.Get(ref this.dir);
                pos = reader.Get(ref this.pos);
                tagCamp = reader.Get(ref this.tagCamp);
                unit = reader.Get(ref this.unit);
            }
        }


        [System.Serializable]
        public partial class ConfigEnemy {
            public override void Serialize(Serializer writer){
                writer.Put(ai);
                writer.Put(asset);
                writer.Put(collider);
                writer.Put(dir);
                writer.Put(dropRate);
                writer.Put(move);
                writer.Put(moveReq);
                writer.Put(pos);
                writer.Put(skill);
                writer.Put(tagEnemy);
                writer.Put(tagTank);
                writer.Put(unit);
            }

            public override void Deserialize(Deserializer reader){
                ai = reader.Get(ref this.ai);
                asset = reader.Get(ref this.asset);
                collider = reader.Get(ref this.collider);
                dir = reader.Get(ref this.dir);
                dropRate = reader.Get(ref this.dropRate);
                move = reader.Get(ref this.move);
                moveReq = reader.Get(ref this.moveReq);
                pos = reader.Get(ref this.pos);
                skill = reader.Get(ref this.skill);
                tagEnemy = reader.Get(ref this.tagEnemy);
                tagTank = reader.Get(ref this.tagTank);
                unit = reader.Get(ref this.unit);
            }
        }


        [System.Serializable]
        public partial class ConfigItem {
            public override void Serialize(Serializer writer){
                writer.Put(asset);
                writer.Put(collider);
                writer.Put(dir);
                writer.Put(pos);
                writer.Put(type);
            }

            public override void Deserialize(Deserializer reader){
                asset = reader.Get(ref this.asset);
                collider = reader.Get(ref this.collider);
                dir = reader.Get(ref this.dir);
                pos = reader.Get(ref this.pos);
                type = reader.Get(ref this.type);
            }
        }


        [System.Serializable]
        public partial class ConfigMover {
            public override void Serialize(Serializer writer){
                writer.Put(asset);
                writer.Put(collider);
                writer.Put(dir);
                writer.Put(move);
                writer.Put(pos);
                writer.Put(unit);
            }

            public override void Deserialize(Deserializer reader){
                asset = reader.Get(ref this.asset);
                collider = reader.Get(ref this.collider);
                dir = reader.Get(ref this.dir);
                move = reader.Get(ref this.move);
                pos = reader.Get(ref this.pos);
                unit = reader.Get(ref this.unit);
            }
        }


        [System.Serializable]
        public partial class ConfigPlayer {
            public override void Serialize(Serializer writer){
                writer.Put(asset);
                writer.Put(collider);
                writer.Put(dir);
                writer.Put(move);
                writer.Put(pos);
                writer.Put(skill);
                writer.Put(tagTank);
                writer.Put(unit);
            }

            public override void Deserialize(Deserializer reader){
                asset = reader.Get(ref this.asset);
                collider = reader.Get(ref this.collider);
                dir = reader.Get(ref this.dir);
                move = reader.Get(ref this.move);
                pos = reader.Get(ref this.pos);
                skill = reader.Get(ref this.skill);
                tagTank = reader.Get(ref this.tagTank);
                unit = reader.Get(ref this.unit);
            }
        }


        [System.Serializable]
        public partial class ConfigTank {
            public override void Serialize(Serializer writer){
                writer.Put(asset);
                writer.Put(collider);
                writer.Put(dir);
                writer.Put(move);
                writer.Put(pos);
                writer.Put(skill);
                writer.Put(tagTank);
                writer.Put(unit);
            }

            public override void Deserialize(Deserializer reader){
                asset = reader.Get(ref this.asset);
                collider = reader.Get(ref this.collider);
                dir = reader.Get(ref this.dir);
                move = reader.Get(ref this.move);
                pos = reader.Get(ref this.pos);
                skill = reader.Get(ref this.skill);
                tagTank = reader.Get(ref this.tagTank);
                unit = reader.Get(ref this.unit);
            }
        }


        [System.Serializable]
        public partial class ConfigUnit {
            public override void Serialize(Serializer writer){
                writer.Put(asset);
                writer.Put(collider);
                writer.Put(dir);
                writer.Put(pos);
            }

            public override void Deserialize(Deserializer reader){
                asset = reader.Get(ref this.asset);
                collider = reader.Get(ref this.collider);
                dir = reader.Get(ref this.dir);
                pos = reader.Get(ref this.pos);
            }
        }

    #endregion
    [CreateAssetMenu]
    public partial class GameConfig : UnityEngine.ScriptableObject {
      
        public List<ConfigEnemy> enemyPrefabs = new List<ConfigEnemy>();
        public List<ConfigPlayer> playerPrefabs = new List<ConfigPlayer>();
        public List<ConfigBullet> bulletPrefabs = new List<ConfigBullet>();
        public List<ConfigItem> itemPrefabs = new List<ConfigItem>();
        public List<ConfigCamp> CampPrefabs = new List<ConfigCamp>();
        public GameObject BornPrefab;
        public GameObject DiedPrefab;

        [Header("SpawnerInfos")] public float bornEnemyInterval = 3;
        public int MAX_ENEMY_COUNT = 6;
        public int initEnemyCount = 20;

        public void Write(){ }

        public void Read(string path){
            var obj = new GameConfig();
            var bytes = File.ReadAllBytes(path);
            var reader = new Deserializer(bytes);
            obj.Deserialize(reader);
            var sss = obj;
        }

        public void Write(string path){
            var writer = new Serializer();
            Serialize(writer);
            var data = writer.CopyData();
            if (!Directory.Exists(Path.GetDirectoryName(path))) {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            File.WriteAllBytes(path, data);
        }

        public void Serialize(Serializer writer){
            writer.PutList(enemyPrefabs);
            writer.PutList(playerPrefabs);
            writer.PutList(bulletPrefabs);
            writer.PutList(itemPrefabs);
            writer.PutList(CampPrefabs);
        }

       public void Deserialize(Deserializer reader){
            enemyPrefabs = reader.GetList(this.enemyPrefabs);
            playerPrefabs = reader.GetList(this.playerPrefabs);
            bulletPrefabs = reader.GetList(this.bulletPrefabs);
            itemPrefabs = reader.GetList(this.itemPrefabs);
            CampPrefabs = reader.GetList(this.CampPrefabs);
        }
    }
}