using Entitas;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    public static class UnitUtil {

        public static void CreateBullet(LVector2 pos, EDir dir, EAssetID type, GameEntity owner){
            var  createPos = pos + DirUtil.GetDirLVec(dir) * TankUtil.TANK_HALF_LEN;
            
        }

        public static GameEntity CreateEnemy(Vector2Int pos, int type){
            return null;
        }
        
    }
}