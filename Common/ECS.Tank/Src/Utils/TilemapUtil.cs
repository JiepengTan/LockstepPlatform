using Lockstep.Math;

namespace Lockstep.Game {
    public static class TilemapUtil {
        public const string TileMapName_BornPos = "BornPos";
        public const int TileID_Brick = 1;
        public const int TileID_Camp = 2;
        public const int TileID_Grass = 3;
        public const int TileID_Iron = 4;
        public const int TileID_Water = 5;
        public const int TileID_Wall = 8;
        public const int TileID_BornPosEnemy = 6;
        public const int TileID_BornPosHero = 7;
    
    
        public const int ItemTankType = 4;
        public const int EnemyCamp = 1;
        public const int PlayerCamp = 2;
        

        public static void CheckBulletWithMap(LVector2Int iPos, GameEntity entity,IGameAudioService audioService,IMap2DService map2DService){
            var unit = entity.unit;
            var bullet = entity.bullet;
            var id = map2DService.Pos2TileId(iPos, false);
            if (id != 0 && unit.health > 0) {
                //collide bullet with world
                if (id == TilemapUtil.TileID_Brick) {
                    if (unit.camp == ECampType.Player) {
                        audioService.PlayClipHitBrick();
                    }

                    map2DService.ReplaceTile(iPos, id, 0);
                    unit.health--;
                }
                else if (id == TilemapUtil.TileID_Iron) {
                    if (!bullet.canDestoryIron) {
                        if (unit.camp == ECampType.Player) {
                            audioService.PlayClipHitIron();
                        }

                        unit.health = 0;
                    }
                    else {
                        if (unit.camp == ECampType.Player) {
                            audioService.PlayClipDestroyIron();
                        }

                        unit.health = LMath.Max(unit.health - 2, 0);
                        map2DService.ReplaceTile(iPos, id, 0);
                    }
                }
                else if (id == TilemapUtil.TileID_Grass) {
                    if (bullet.canDestoryGrass) {
                        if (unit.camp == ECampType.Player) {
                            audioService.PlayClipDestroyGrass();
                        }

                        unit.health -= 0;
                        map2DService.ReplaceTile(iPos, id, 0);
                    }
                }
                else if (id == TilemapUtil.TileID_Wall) {
                    unit.health = 0;
                }
            }
        }
    }
}