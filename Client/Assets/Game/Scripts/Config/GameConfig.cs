using System;
using System.Collections.Generic;
using System.Reflection;
using DesperateDevs.Utils;
using Entitas;
using Lockstep.ECS.Game;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Lockstep.Game {
    [System.Serializable]
    public class BaseEntitySetter {
        public static Dictionary<Type, int> type2Idx = new Dictionary<Type, int>();
        public static Dictionary<string, int> name2Idx;

        public virtual void CopyTo(Entity entity){
            var allMemberInfos = this.GetType().GetPublicMemberInfos();
            foreach (var memberInfo in allMemberInfos) {
                int index = 0;
                var memType = memberInfo.GetType();
                if (type2Idx.TryGetValue(memType, out int qidx)) {
                    index = qidx;
                }
                else {
                    if (name2Idx == null) {
                        name2Idx = new Dictionary<string, int>();
                        var fileds = typeof(GameComponentsLookup).GetFields(BindingFlags.Static | BindingFlags.Public);
                        foreach (var filed in fileds) {
                            if (filed.IsLiteral && !filed.IsInitOnly && filed.DeclaringType == typeof(int)) {
                                name2Idx.Add(filed.Name + "Component", (int) filed.GetRawConstantValue());
                            }
                        }
                    }

                    if (name2Idx.TryGetValue(memType.Name, out int nidx)) {
                        index = qidx;
                        type2Idx.Add(memType, nidx);
                    }
                    else {
                        Debug.LogError("Do not have type" + memType.ToString());
                        return;
                    }
                }

                IComponent srcComp = memberInfo.GetValue(this) as IComponent;
                if (entity.HasComponent(index)) {
                    IComponent dstComp = entity.GetComponent(index);
                    srcComp.CopyPublicMemberValues((object) dstComp);
                }
                else {
                    IComponent dstComp = entity.CreateComponent(index, srcComp.GetType());
                    srcComp.CopyPublicMemberValues((object) dstComp);
                    entity.AddComponent(index, dstComp);
                }
            }
        }
    }

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
            public TankAIComponent ai = new TankAIComponent();
            public TankSkillComponent skill = new TankSkillComponent();
            public TagEnemyComponent tag = new TagEnemyComponent();
        }

        [System.Serializable]
        public class Player : Unit {
            public TankSkillComponent skill = new TankSkillComponent();
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