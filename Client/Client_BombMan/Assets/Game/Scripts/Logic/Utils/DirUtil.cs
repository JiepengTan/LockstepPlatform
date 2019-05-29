using Entitas;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    public enum EDir {
        Up,
        Left,
        Down,
        Right,
        EnumCount,
    }
    
    public static class DirUtil {
        public static Vector2Int GetDirVec(EDir dir){
            switch (dir) {
                case EDir.Up: return Vector2Int.up;
                case EDir.Right: return Vector2Int.right;
                case EDir.Down: return Vector2Int.down;
                case EDir.Left: return Vector2Int.left;
            }

            return Vector2Int.up;
        }
        public static LVector2 GetDirLVec(EDir dir){
            switch (dir) {
                case EDir.Up: return LVector2.up;
                case EDir.Right: return LVector2.right;
                case EDir.Down: return LVector2.down;
                case EDir.Left: return LVector2.left;
            }

            return LVector2.up;
        }
        public static int GetDirDeg(EDir dir){
            return ((int) dir) * 90;
        }

        public static Vector2Int GetBorderDir(EDir dir){
            var isUpDown = (int) (dir) % 2 == 0;
            var borderDir = Vector2Int.up;
            if (isUpDown) {
                borderDir = Vector2Int.right;
            }

            return borderDir;
        }
        public static EDir GetEDirFromVec(Vector2Int vec){
            if (vec.sqrMagnitude != 1) {
                return EDir.EnumCount;
            }

            var num = vec.y * 2 + vec.x;
            switch (num) {
                case 2: return  EDir.Up;
                case -2: return EDir.Down;
                case 1: return  EDir.Right;
                case -1: return EDir.Left;
            }
            return EDir.EnumCount;
        }
    }
}