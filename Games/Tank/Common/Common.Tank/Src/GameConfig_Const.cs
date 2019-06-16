using System.Collections.Generic;

namespace Lockstep.Game {


    [System.Serializable]
    public  class ConstGameConfig {
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

        public static string ConfigPath = "GameConfig";
    }
}