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
        [System.Serializable]
        public class Unit : BaseEntitySetter {
            public UnitComponent unit = new UnitComponent();

            //public ColliderComponent collider = new ColliderComponent();
            public MoveComponent move = new MoveComponent();

            //public PositionComponent position = new PositionComponent();
            public AssetComponent asset = new AssetComponent();
        }

        [System.Serializable]
        public class Tank : Unit {
            public AIComponent ai = new AIComponent();
            public SkillComponent skill = new SkillComponent();
            public TagEnemyComponent tag = new TagEnemyComponent();
        }

        [System.Serializable]
        public class Player : Unit {
            public SkillComponent skill = new SkillComponent();
            public TagPlayerComponent tag = new TagPlayerComponent();
        }

        [System.Serializable]
        public class Camp : Unit { }

        [System.Serializable]
        public class Bullet : Unit {
            public OwnerComponent owner = new OwnerComponent();
            public TagBulletComponent tag = new TagBulletComponent();
        }

        [System.Serializable]
        public class Item : BaseEntitySetter {
            public ItemTypeComponent type = new ItemTypeComponent();
            public AssetComponent asset = new AssetComponent();
        }

        [Header("Prefab info")] public List<Tank> enemyPrefabs = new List<Tank>();
        public List<Player> playerPrefabs = new List<Player>();
        public List<Bullet> bulletPrefabs = new List<Bullet>();
        public List<Item> itemPrefabs = new List<Item>();
        public Camp CampPrefab = new Camp();
        public GameObject BornPrefab;
        public GameObject DiedPrefab;

        [Header("SpawnerInfos")] public float bornEnemyInterval = 3;
        public int MAX_ENEMY_COUNT = 6;
        public int initEnemyCount = 20;
    }
}