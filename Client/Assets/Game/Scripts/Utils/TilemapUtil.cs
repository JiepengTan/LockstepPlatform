using UnityEngine;

namespace Lockstep.Game {
    public static class TilemapUtil {
        private const byte AirId = 0;
        private const byte GrassId = 1;
        private const byte WaterId = 2;
        private const byte BrickId = 3;
        private const byte IronId = 4;
        private const byte OutOfRangeId = byte.MaxValue;

        private static bool IsBullet(byte type){
            return type >= (byte) EAssetID.Bullet0;
        }

        public static bool CanMove(byte[,] tileIds, Vector2Int coord){
            var id = GetTileId(tileIds, coord);
            return id < WaterId;
        }

        public static bool IsWater(byte[,] tileIds, Vector2Int coord){
            return IsWater(GetTileId(tileIds,coord));
        }

        public static bool IsIron(byte[,] tileIds, Vector2Int coord){
            return IsIron(GetTileId(tileIds,coord));
        }

        public static bool IsAir(byte[,] tileIds, Vector2Int coord){
            return IsAir(GetTileId(tileIds,coord));
        }

        public static bool IsGrass(byte[,] tileIds, Vector2Int coord){
            return IsGrass(GetTileId(tileIds,coord));
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