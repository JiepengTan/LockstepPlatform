using System;
using System.Collections.Generic;
using System.IO;
using Lockstep.ECS;
using Lockstep.ECS.Game;
using Lockstep.Math;
using Lockstep.Serialization;
using UnityEngine;

namespace Lockstep.Game {
    [CreateAssetMenu]
    public partial class GameConfig : UnityEngine.ScriptableObject, IGameConfigService {
        public List<ConfigEnemy> _enemyPrefabs = new List<ConfigEnemy>();
        public List<ConfigPlayer> _playerPrefabs = new List<ConfigPlayer>();
        public List<ConfigBullet> _bulletPrefabs = new List<ConfigBullet>();
        public List<ConfigItem> _itemPrefabs = new List<ConfigItem>();
        public List<ConfigCamp> _CampPrefabs = new List<ConfigCamp>();

        public int MaxPlayerCount { get; } = 2;
        public LVector2 TankBornOffset { get; } = LVector2.one;
        public LFloat TankBornDelay { get; } = LFloat.one;
        public LFloat DeltaTime { get; } = new LFloat(true, 16);


        public List<ConfigEnemy> enemyPrefabs {
            get => _enemyPrefabs;
            set => _enemyPrefabs = value;
        }

        public List<ConfigPlayer> playerPrefabs {
            get => _playerPrefabs;
            set => _playerPrefabs = value;
        }

        public List<ConfigBullet> bulletPrefabs {
            get => _bulletPrefabs;
            set => _bulletPrefabs = value;
        }

        public List<ConfigItem> itemPrefabs {
            get => _itemPrefabs;
            set => _itemPrefabs = value;
        }

        public List<ConfigCamp> CampPrefabs {
            get => _CampPrefabs;
            set => _CampPrefabs = value;
        }

        public short BornPrefabAssetId { get; set; }
        public short DiedPrefabAssetId { get; set; }

        public GameObject BornPrefab;
        public GameObject DiedPrefab;

        public float bornEnemyInterval => 3;
        public int MAX_ENEMY_COUNT => 6;
        public int initEnemyCount => 20;

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