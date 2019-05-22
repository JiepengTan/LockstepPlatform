using System.Collections.Generic;
using Lockstep.Math;

namespace Lockstep.Game {
    [System.Serializable]
    public enum ECampType : byte {
        Player,
        Enemy,
        Other,
    }

    [System.Serializable]
    public enum EAssetID : ushort {
        EnemyTank0 = 10,
        EnemyTank1,
        EnemyTank2,
        EnemyTank3,
        EnemyTank4,

        ItemAddLife = 20,
        ItemBoom,
        ItemUpgrade,

        Camp = 30,

        PlayerTank0 = 40,
        PlayerTank1,
        PlayerTank2,
        PlayerTank3,

        Bullet0 = 50,
        Bullet1,
        Bullet2,
    }

    [System.Serializable]
    public partial class GameConfig {
        private static Dictionary<ushort, string> _id2Path = new Dictionary<ushort, string>() {
            {(ushort) EAssetID.PlayerTank0, "Prefabs/Player/Player0"},
            {(ushort) EAssetID.PlayerTank1, "Prefabs/Player/Player1"},
            {(ushort) EAssetID.PlayerTank2, "Prefabs/Player/Player2"},
            {(ushort) EAssetID.PlayerTank3, "Prefabs/Player/Player3"},

            {(ushort) EAssetID.EnemyTank0, "Prefabs/Tanks/Tank0"},
            {(ushort) EAssetID.EnemyTank1, "Prefabs/Tanks/Tank1"},
            {(ushort) EAssetID.EnemyTank2, "Prefabs/Tanks/Tank2"},
            {(ushort) EAssetID.EnemyTank3, "Prefabs/Tanks/Tank3"},
            {(ushort) EAssetID.EnemyTank4, "Prefabs/Tanks/Tank4"},

            {(ushort) EAssetID.Bullet0, "Prefabs/Bullet/Bullet0"},
            {(ushort) EAssetID.Bullet1, "Prefabs/Bullet/Bullet1"},
            {(ushort) EAssetID.Bullet2, "Prefabs/Bullet/Bullet2"},

            {(ushort) EAssetID.ItemAddLife, "Prefabs/Items/ItemAddLife"},
            {(ushort) EAssetID.ItemBoom, "Prefabs/Items/ItemBoom"},
            {(ushort) EAssetID.ItemUpgrade, "Prefabs/Items/ItemUpgrade"},

            {(ushort) EAssetID.Camp, "Prefabs/Camp/Camp"},
        };

        public static string GetAssetPath(EAssetID assetId){
            return GetAssetPath((ushort) assetId);
        }

        public static string GetAssetPath(ushort assetId){
            if (_id2Path.TryGetValue(assetId, out string path)) {
                return path;
            }

            return null;
        }

        public const int MaxPlayerCount = 2;
        public static LVector2 TankBornOffset = LVector2.one;
        public static LFloat TankBornDelay = LFloat.one;
        public static LFloat DeltaTime = new LFloat(true, 16);

        public static string ConfigPath = "GameConfig";
    }
}