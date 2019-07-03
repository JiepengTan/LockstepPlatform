using System.Runtime.InteropServices;

namespace Lockstep.AI {
    [StructLayout(LayoutKind.Sequential, Pack = NativeHelper.STRUCT_PACK)]
    public unsafe partial struct BTCActionLeaf {
        internal int status;
        internal bool needExit;
    }
}