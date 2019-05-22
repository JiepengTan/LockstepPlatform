using UnityEngine;

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
        
        
        private const byte AirId = 0;
        private const byte GrassId = 1;
        private const byte WaterId = 2;
        private const byte BrickId = 3;
        private const byte IronId = 4;
        private const byte OutOfRangeId = byte.MaxValue;

        public static void CheckBulletWithMap(Vector2Int iPos, GameEntity entity,IAudioService audioService,IMapService mapService){
            var unit = entity.unit;
            var bullet = entity.bullet;
            var id = mapService.Pos2TileId(iPos, false);
            if (id != 0 && unit.health > 0) {
                //collide bullet with world
                if (id == TilemapUtil.TileID_Brick) {
                    if (unit.camp == ECampType.Player) {
                        audioService.PlayClipHitBrick();
                    }

                    mapService.ReplaceTile(iPos, id, 0);
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

                        unit.health = Mathf.Max(unit.health - 2, 0);
                        mapService.ReplaceTile(iPos, id, 0);
                    }
                }
                else if (id == TilemapUtil.TileID_Grass) {
                    if (bullet.canDestoryGrass) {
                        if (unit.camp == ECampType.Player) {
                            audioService.PlayClipDestroyGrass();
                        }

                        unit.health -= 0;
                        mapService.ReplaceTile(iPos, id, 0);
                    }
                }
                else if (id == TilemapUtil.TileID_Wall) {
                    unit.health = 0;
                }
            }
        }


        private static bool IsBullet(byte type){
            return type >= (byte) EAssetID.Bullet0;
        }

        public static bool CanMove(byte[,] tileIds, Vector2Int coord){
            var id = GetTileId(tileIds, coord);
            return id < WaterId;
        }

        public static bool IsWater(byte[,] tileIds, Vector2Int coord){
            return IsWater(GetTileId(tileIds, coord));
        }

        public static bool IsIron(byte[,] tileIds, Vector2Int coord){
            return IsIron(GetTileId(tileIds, coord));
        }

        public static bool IsAir(byte[,] tileIds, Vector2Int coord){
            return IsAir(GetTileId(tileIds, coord));
        }

        public static bool IsGrass(byte[,] tileIds, Vector2Int coord){
            return IsGrass(GetTileId(tileIds, coord));
        }

        public static byte GetTileId(byte[,] tileIds, Vector2Int coord){
            if (IsOutOfRange(tileIds, coord)) return OutOfRangeId;
            return tileIds[coord.x, coord.y];
        }

        public static bool IsOutOfRange(byte[,] tileIds, Vector2Int coord){
            var maxX = tileIds.GetLength(0);
            var maxY = tileIds.GetLength(1);
            return coord.x >= 0
                   && coord.x < maxX
                   && coord.y >= 0
                   && coord.y < maxY;
        }

        public static bool IsWater(byte id){
            return id == WaterId;
        }

        public static bool IsIron(byte id){
            return id == IronId;
        }

        public static bool IsBrick(byte id){
            return id == BrickId;
        }

        public static bool IsAir(byte id){
            return id == AirId;
        }

        public static bool IsGrass(byte id){
            return id == GrassId;
        }
    }
}