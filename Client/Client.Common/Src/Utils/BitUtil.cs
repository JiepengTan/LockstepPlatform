using UnityEngine;

namespace Lockstep.Game {
  
    public static partial class BitUtil {
        public static bool HasBit(byte val, byte idx){
            return (val & 1 << idx) != 0;
        }

        public static void SetBit(byte val, byte idx){
            val |= (byte) (1 << idx);
        }

        public static byte ToByte(byte idx){
            return (byte) (1 << idx);
        }
    }
}