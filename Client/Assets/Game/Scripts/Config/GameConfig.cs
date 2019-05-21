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
    public class GameConfig : UnityEngine.ScriptableObject {
        public class Unit : BaseEntitySetter{
            public AssetComponent asset = new AssetComponent();
            public DirComponent dir = new DirComponent();
            public PosComponent pos = new PosComponent();
        }
        [System.Serializable]
        public class Item : Unit {
            public ItemTypeComponent type = new ItemTypeComponent();
        }
        [System.Serializable]
        public class Camp : Unit { 
            public UnitComponent unit = new UnitComponent();
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
            public AIComponent ai = new AIComponent();
            public SkillComponent skill = new SkillComponent();
            public TagTankComponent tag = new TagTankComponent();
        }

        [System.Serializable]
        public class Player : Mover {
            public SkillComponent skill = new SkillComponent();
        }


        [System.Serializable]
        public class Bullet : Mover {
            public OwnerComponent owner = new OwnerComponent();
            public TagBulletComponent tag = new TagBulletComponent();
        }


        [Header("Prefab info")] public List<Tank> enemyPrefabs = new List<Tank>();
        public List<Player> playerPrefabs = new List<Player>();
        public List<Bullet> bulletPrefabs = new List<Bullet>();
        public List<Item> itemPrefabs = new List<Item>();
        public List<Camp> CampPrefabs = new List<Camp>();
        public GameObject BornPrefab;
        public GameObject DiedPrefab;

        [Header("SpawnerInfos")] public float bornEnemyInterval = 3;
        public int MAX_ENEMY_COUNT = 6;
        public int initEnemyCount = 20;
    }
}