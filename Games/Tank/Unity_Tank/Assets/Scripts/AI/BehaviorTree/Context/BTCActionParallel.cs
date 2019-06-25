using System;
using System.Runtime.InteropServices;

namespace Lockstep.AI {
    [StructLayout(LayoutKind.Sequential, Pack = NativeHelper.STRUCT_PACK)]
    public unsafe partial struct BTCActionParallel {
        public struct BTCParallelStatusEval {
            private bool status0;
            private bool status1;
            private bool status2;
            private bool status3;
            private bool status4;
            private bool status5;
            private bool status6;
            private bool status7;
            private bool status8;
            private bool status9;
            private bool status10;
            private bool status11;
            private bool status12;
            private bool status13;
            private bool status14;
            private bool status15;

            public bool* Status(Int32 index){
                if (index < 0 || index >= 16) {
                    NativeHelper.ArrayOutOfRange();
                }

                fixed (bool* p = &status0) {
                    return p + index;
                }
            }

            public bool this[int index] {
                get => *Status(index);
                set => *Status(index) = value;
            }

            public const int Size = 16;

            public void Init(bool val){
                for (int i = 0; i < Size; i++) {
                    this[i] = val;
                }
            }
        }

        public struct BTCParallelStatusRunning {
            private byte status0;
            private byte status1;
            private byte status2;
            private byte status3;
            private byte status4;
            private byte status5;
            private byte status6;
            private byte status7;
            private byte status8;
            private byte status9;
            private byte status10;
            private byte status11;
            private byte status12;
            private byte status13;
            private byte status14;
            private byte status15;

            public byte* Status(Int32 index){
                if (index < 0 || index >= Size) {
                    NativeHelper.ArrayOutOfRange();
                }

                fixed (byte* p = &status0) {
                    return p + index;
                }
            }

            public byte this[int index] {
                get => *Status(index);
                set => *Status(index) = value;
            }

            public const int Size = 16;

            public void Init(byte val){
                for (int i = 0; i < Size; i++) {
                    this[i] = val;
                }
            }
        }

        internal BTCParallelStatusEval evaluationStatus;
        internal BTCParallelStatusRunning StatusRunning;
    }
}