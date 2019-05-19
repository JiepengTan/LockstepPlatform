using System.Collections.Generic;

namespace Lockstep.Game {
    [System.Serializable]
    public enum ECampType :byte{
        Player,
        Enemy,
        Other,
    }

    [System.Serializable]
    public enum EAssetID :ushort{
        PlayerTank0 = 10,
        PlayerTank1,
        PlayerTank2,
        PlayerTank3,
            
        EnemyTank0 = 20,
        EnemyTank1,
        EnemyTank2,
        EnemyTank3,
        EnemyTank4,
            
        Bullet0 = 30,
        Bullet1,
        Bullet2,
            
        ItemAddLife = 40,
        ItemBoom,
        ItemUpgrade,
    }
    [System.Serializable]
    public class Define {


        public static Dictionary<ushort, string> id2Path = new Dictionary<ushort, string>()
        {
            {(ushort)EAssetID.PlayerTank0,"Prefabs/Player/Player0"},
            {(ushort)EAssetID.PlayerTank1,"Prefabs/Player/Player1"},
            {(ushort)EAssetID.PlayerTank2,"Prefabs/Player/Player2"},
            {(ushort)EAssetID.PlayerTank3,"Prefabs/Player/Player3"},
            
            {(ushort)EAssetID.EnemyTank0, "Prefabs/Tanks/Player0"},
            {(ushort)EAssetID.EnemyTank1, "Prefabs/Tanks/Player1"},
            {(ushort)EAssetID.EnemyTank2, "Prefabs/Tanks/Player2"},
            {(ushort)EAssetID.EnemyTank3, "Prefabs/Tanks/Player3"},
            {(ushort)EAssetID.EnemyTank4, "Prefabs/Tanks/Player4"},
            
            {(ushort)EAssetID.Bullet0, "Prefabs/Bullet/Player0"},
            {(ushort)EAssetID.Bullet1, "Prefabs/Bullet/Player0"},
            {(ushort)EAssetID.Bullet2, "Prefabs/Bullet/Player0"},
            
            {(ushort)EAssetID.ItemAddLife, "Prefabs/Items/ItemAddLife"},
            {(ushort)EAssetID.ItemBoom, "Prefabs/Items/ItemBoom"},
            {(ushort)EAssetID.ItemUpgrade, "Prefabs/Items/ItemUpgrade"},
        };

        public static string GetAssetPath(ushort assetId){
            if (id2Path.TryGetValue(assetId, out string path)) {
                return path;
            }
            return null;
        }
    }
}