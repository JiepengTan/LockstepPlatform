using System.Runtime.InteropServices;

namespace Lockstep.AI {
    [StructLayout(LayoutKind.Sequential, Pack = NativeHelper.STRUCT_PACK)]
    public unsafe partial struct BTCActionSequence 
    {
        internal int currentSelectedIndex;
        public BTCActionSequence(int idx = -1)
        {
            currentSelectedIndex = idx;
        }
    }
}