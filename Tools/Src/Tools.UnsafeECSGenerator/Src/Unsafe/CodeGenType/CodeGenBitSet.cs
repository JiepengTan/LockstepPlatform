using System;
using System.Collections.Generic;
using Lockstep.ECS.ECDefine;

namespace Lockstep.ECS.UnsafeECS.CodeGen {
    public class CodeGenBitSet : CodeGenBase {
        private string ClsPrototype = @"
    [StructLayoutAttribute(LayoutKind.Sequential, Pack=4)]
    public unsafe partial struct BitSet#BIT_SET_NUM {
        public const Int32 BitsSize = #BIT_SET_NUM;
        private const int Int32ArraySize = BitsSize / 32;
        public fixed uint bits[Int32ArraySize];
        public static void Set(BitSet#BIT_SET_NUM* set, Int32 bit) {
            set->bits[bit/32] |= (1u<<(bit%32));
        }
        public static void Clear(BitSet#BIT_SET_NUM* set, Int32 bit) {
            set->bits[bit/32] &= ~(1u<<(bit%32));
        }
        public static void ClearAll(BitSet#BIT_SET_NUM* set) {
            *set = __default.BitSet#BIT_SET_NUM;
        }
        public static Boolean IsSet(BitSet#BIT_SET_NUM* set, Int32 bit) {
            return (set->bits[bit/32]&(1u<<(bit%32))) != 0u;
        }
    }
";

        public override void GenCode(Type[] rawtypes){
            foreach (var size in GetAllSize()) {
                GenBitSet(size);
                
            }
        }
        public static int[] GetAllSize(){
            List<int> allSize = new List<int>();
            for (int i = 128; i <= 4098; i *= 2) {
                allSize.Add(i);
            }

            return allSize.ToArray();
        }

        void GenBitSet(int bitNum){
            sb.AppendLine(ClsPrototype.Replace("#BIT_SET_NUM", bitNum.ToString()));
        }
    }
}