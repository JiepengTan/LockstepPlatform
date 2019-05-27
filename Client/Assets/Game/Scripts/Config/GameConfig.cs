using System;
using System.Collections.Generic;
using System.Reflection;
using DesperateDevs.Utils;
using Entitas;
using Lockstep.ECS.Game;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Lockstep.Game {
   
    [CreateAssetMenu]
    public partial class GameConfig : UnityEngine.ScriptableObject {
        public class Unit : BaseEntitySetter{
            public AssetComponent asset = new AssetComponent();
            public DirComponent dir = new DirComponent();
            public PosComponent pos = new PosComponent();
            public ColliderComponent collider = new ColliderComponent();
        }
        [System.Serializable]
        public class Item : Unit {
            public ItemTypeComponent type = new ItemTypeComponent();
        }
        [System.Serializable]
        public class Camp : Unit { 
            public UnitComponent unit = new UnitComponent();
            public TagCampComponent tagCamp = new TagCampComponent();
        }
        [System.Serializable]
        public class Mover : Unit {
            public UnitComponent unit = new UnitComponent();
            //public ColliderComponent collider = new ColliderComponent();
            public MoveComponent move = new MoveComponent();
            //public PositionComponent position = new PositionComponent();
        }
        
        [System.Serializable]
        public class Tank : Mover {
            public SkillComponent skill = new SkillComponent();
            public TagTankComponent tagTank = new TagTankComponent();
        }

        [System.Serializable]
        public class Player : Tank {
            ActorIdComponent actorId = new ActorIdComponent();
        }
        [System.Serializable]
        public class Enemy : Tank {
            public AIComponent ai = new AIComponent();
            public TagEnemyComponent tagEnemy = new TagEnemyComponent();
            public MoveRequestComponent moveReq = new MoveRequestComponent();
            public DropRateComponent dropRate = new DropRateComponent();
        }

        [System.Serializable]
        public class Bullet : Mover {
            public OwnerComponent owner = new OwnerComponent();
            public BulletComponent bullet = new BulletComponent();
            public TagBulletComponent tagBullet = new TagBulletComponent();
        }


        public List<Enemy> enemyPrefabs = new List<Enemy>();
        public List<Player> playerPrefabs = new List<Player>();
        public List<Bullet> bulletPrefabs = new List<Bullet>();
        public List<Item> itemPrefabs = new List<Item>();
        public List<Camp> CampPrefabs = new List<Camp>();
        public GameObject BornPrefab;
        public GameObject DiedPrefab;

        [Header("SpawnerInfos")] 
        public float bornEnemyInterval = 3;
        public int MAX_ENEMY_COUNT = 6;
        public int initEnemyCount = 20;
    }
}