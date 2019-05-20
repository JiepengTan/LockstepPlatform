using Entitas;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    public static class UnitUtil {

        ///TODO 创建子弹 
        public static void CreateBullet(LVector2 pos, EDir dir, EAssetID type, GameEntity owner){
            var  createPos = pos + DirUtil.GetDirLVec(dir) * TankUtil.TANK_HALF_LEN;
            
        }
        ///TODO 创建Enemy 
        public static void CreateEnemy(Vector2Int pos, int type){
        }
        public static void CreatePlayer(byte actorId, int type){
        }

        public static void TakeDamage( GameEntity atker, GameEntity suffer){
            
        }

        private static void CreateItem(){ }

        public static void DropItem(LFloat rate){
            //var x = Random.Range(min.x + 1.0f, max.x - 3.0f);
            //var y = Random.Range(min.y + 1.0f, max.y - 3.0f);
            //CreateItem(new Vector2(x, y), Random.Range(0, itemPrefabs.Count));
        }

    }
}