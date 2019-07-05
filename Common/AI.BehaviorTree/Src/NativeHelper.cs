using System;
using System.Runtime.InteropServices;

namespace Lockstep.AI {
    public class NativeHelper {
        public const int STRUCT_PACK = 4;

        public static void Free(IntPtr ptr){
            Marshal.FreeHGlobal(ptr);
        }

        public static IntPtr Alloc(int bytes){
            return Marshal.AllocHGlobal(bytes);
        }
        
        //TODO 调用c接口
        public static unsafe void Zero(byte* ptr, int size){
            for (int index = size - 1; index >= 0; --index)
                ptr[index] = (byte) 0;
        }
        //TODO 调用c接口
        public static unsafe void MemCpy(byte* srcptr, byte* dstptr, int size){
            for (int index = size - 1; index >= 0; --index)
                dstptr[index] = srcptr[index];
        }
        public static void NullPointer()
        {
            throw new NullReferenceException("Method invoked on null pointer.");
        }

        public static void ArrayOutOfRange()
        {
            throw new ArgumentOutOfRangeException("Array index out of range");
        }
    }
}