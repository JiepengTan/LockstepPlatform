using System.Collections.Generic;
using System.IO;
using LitJson;
using Lockstep.ECS;
using Lockstep.Math;
using Lockstep.Serialization;

namespace Lockstep.Game {
    public partial class GameConfigService : BaseService, IGameConfigService {
        public List<BaseEntitySetter> _enemyPrefabs = new List<BaseEntitySetter>();
        public List<BaseEntitySetter> _playerPrefabs = new List<BaseEntitySetter>();
        public List<BaseEntitySetter> _bulletPrefabs = new List<BaseEntitySetter>();
        public List<BaseEntitySetter> _itemPrefabs = new List<BaseEntitySetter>();
        public List<BaseEntitySetter> _CampPrefabs = new List<BaseEntitySetter>();
        public int MaxPlayerCount { get; set; } = 2;
        public LVector2 TankBornOffset { get; set; } = LVector2.one;
        public LFloat TankBornDelay { get; set; } = LFloat.one;
        public LFloat DeltaTime { get; set; } = new LFloat(true, 16);
        public string RelPath { get; set; } = "";

        public List<BaseEntitySetter> enemyPrefabs {
            get => _enemyPrefabs;
            set => _enemyPrefabs = value;
        }

        public List<BaseEntitySetter> playerPrefabs {
            get => _playerPrefabs;
            set => _playerPrefabs = value;
        }

        public List<BaseEntitySetter> bulletPrefabs {
            get => _bulletPrefabs;
            set => _bulletPrefabs = value;
        }

        public List<BaseEntitySetter> itemPrefabs {
            get => _itemPrefabs;
            set => _itemPrefabs = value;
        }

        public List<BaseEntitySetter> CampPrefabs {
            get => _CampPrefabs;
            set => _CampPrefabs = value;
        }

        public short BornPrefabAssetId { get; set; } = 60;
        public short DiedPrefabAssetId { get; set; } = 61;

        public float bornEnemyInterval => 3;
        public int MAX_ENEMY_COUNT => 6;
        public int initEnemyCount => 20;

        public override void DoAwake(IServiceContainer services){
            this.Read(_constStateService.ClientConfigPath + "GameConfig.bytes");
            //if (_constStateService.RunMode == EPureModeType.Pure) {
            //    var txt = JsonMapper.ToJson(this);
            //    File.WriteAllText(JsonPath, txt);
            //}
        }

        public void Write(){ }

        public void Read(string path){
            var bytes = File.ReadAllBytes(path);
            var reader = new Deserializer(bytes);
            Deserialize(reader);
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
            writer.Write(enemyPrefabs);
            writer.Write(playerPrefabs);
            writer.Write(bulletPrefabs);
            writer.Write(itemPrefabs);
            writer.Write(CampPrefabs);
        }

        public void Deserialize(Deserializer reader){
            enemyPrefabs = GetList<BaseEntitySetter, ConfigEnemy>(reader);
            playerPrefabs = GetList<BaseEntitySetter, ConfigPlayer>(reader);
            bulletPrefabs = GetList<BaseEntitySetter, ConfigBullet>(reader);
            itemPrefabs = GetList<BaseEntitySetter, ConfigItem>(reader);
            CampPrefabs = GetList<BaseEntitySetter, ConfigCamp>(reader);
        }

        List<TRet> GetList<TRet, TParam>(Deserializer reader) where TParam : BaseEntitySetter, new()
            where TRet : BaseEntitySetter{
            var lst = new List<TParam>();
            lst = reader.ReadList(lst);
            var lst2 = new List<TRet>(lst.Count);
            foreach (var item in lst) {
                lst2.Add((TRet) (object) item);
            }

            return lst2;
        }
    }
}