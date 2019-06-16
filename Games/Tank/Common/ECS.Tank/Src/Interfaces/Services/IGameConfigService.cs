using System.Collections.Generic;
using Lockstep.Math;

namespace Lockstep.Game {
    public interface IGameConfigService:IService {
        List<ConfigEnemy> enemyPrefabs { get; }
        List<ConfigPlayer> playerPrefabs { get; }
        List<ConfigBullet> bulletPrefabs { get; }
        List<ConfigItem> itemPrefabs { get; }
        List<ConfigCamp> CampPrefabs { get; }
        short BornPrefabAssetId { get; }
        short DiedPrefabAssetId { get; }
        float bornEnemyInterval { get; }
        int MAX_ENEMY_COUNT { get; }
        int initEnemyCount { get; }
        
        int MaxPlayerCount { get; } 
        LVector2 TankBornOffset { get; } 
        LFloat TankBornDelay { get; } 
        LFloat DeltaTime { get; } 
    }
}