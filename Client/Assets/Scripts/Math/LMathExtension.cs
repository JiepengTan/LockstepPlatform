#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif
using Lockstep.Math;

namespace Lockstep.Math {
#if UNITY_5_3_OR_NEWER
    public static partial class LMathExtension {
        public static Vector2Int Floor(this LVector2 vec){
            return new Vector2Int(
                LMath.FloorToInt(vec.x),
                LMath.FloorToInt(vec.y));
        }

        public static Vector3Int Floor(this LVector3 vec){
            return new Vector3Int(
                LMath.FloorToInt(vec.x),
                LMath.FloorToInt(vec.y),
                LMath.FloorToInt(vec.z)
            );
        }

        public static LVector2 ToLVector2(this Vector2Int vec){
            return new LVector2(true,vec.x * LFloat.Precision, vec.y * LFloat.Precision);
        }

        public static LVector3 ToLVector3(this Vector3Int vec){
            return new LVector3(true,vec.x * LFloat.Precision, vec.y * LFloat.Precision, vec.z * LFloat.Precision);
        }

        public static Vector2Int ToVector2Int(this LVector2 vec){
            return new Vector2Int(vec.x.ToInt(), vec.y.ToInt());
        }

        public static Vector3Int ToVector2Int(this LVector3 vec){
            return new Vector3Int(vec.x.ToInt(), vec.y.ToInt(), vec.z.ToInt());
        }

        public static LVector2 ToLVector2(this Vector2 vec){
            return new LVector2(
                LMath.ToLFloat(vec.x),
                LMath.ToLFloat(vec.y));
        }

        public static LVector3 ToLVector3(this Vector3 vec){
            return new LVector3(
                LMath.ToLFloat(vec.x),
                LMath.ToLFloat(vec.y),
                LMath.ToLFloat(vec.z));
        }

        public static Vector2 ToVector2(this LVector2 vec){
            return new Vector2(vec.x.ToFloat(), vec.y.ToFloat());
        }

        public static Vector3 ToVector3(this LVector3 vec){
            return new Vector3(vec.x.ToFloat(), vec.y.ToFloat(), vec.z.ToFloat());
        }
    }
#endif

    public  static partial class LMathExtension {
        public static LFloat ToLFloat(this float v){
            return LMath.ToLFloat(v);
        }

        public static LFloat ToLFloat(this int v){
            return LMath.ToLFloat(v);
        }

        public static LFloat ToLFloat(this long v){
            return LMath.ToLFloat(v);
        }
    }
}