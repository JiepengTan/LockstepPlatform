using System;
using System.Collections.Generic;
using System.Reflection;
using Entitas;
using Lockstep.ECS.Game;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Lockstep.Game {
    [CreateAssetMenu]
    public partial class GameConfig : UnityEngine.ScriptableObject {
        public class Unit : BaseEntitySetter {
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
            public MoveComponent move = new MoveComponent();
        }

        [System.Serializable]
        public class Player : Mover {
            public SkillComponent skill = new SkillComponent();
            public ActorIdComponent actorId = new ActorIdComponent();
        }

        [System.Serializable]
        public class Enemy : Mover {
            public AIComponent ai = new AIComponent();
            public TagEnemyComponent tagEnemy = new TagEnemyComponent();
            public MoveRequestComponent moveReq = new MoveRequestComponent();
            public DropRateComponent dropRate = new DropRateComponent();
        }

        [System.Serializable]
        public class Bomb : Unit {
            public UnitComponent unit = new UnitComponent();
            public OwnerComponent owner = new OwnerComponent();
            public BombComponent bomb = new BombComponent();
        }


        public List<Enemy> enemyPrefabs = new List<Enemy>();
        public List<Player> playerPrefabs = new List<Player>();
        public List<Bomb> bulletPrefabs = new List<Bomb>();
        public List<Item> itemPrefabs = new List<Item>();
        public List<Camp> CampPrefabs = new List<Camp>();
        public GameObject BornPrefab;
        public GameObject DiedPrefab;

        [Header("SpawnerInfos")] public float bornEnemyInterval = 3;
        public int MAX_ENEMY_COUNT = 6;
        public int initEnemyCount = 20;
        public bool isNetworkMode = true;
    }
}