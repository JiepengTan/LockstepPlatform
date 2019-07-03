using System.Runtime.InteropServices;

namespace Lockstep.AI {
    [StructLayout(LayoutKind.Sequential, Pack = NativeHelper.STRUCT_PACK)]
    public unsafe partial struct BTCActionPrioritizedSelector {
        public int currentSelectedIndex;
        public int lastSelectedIndex;

        public BTCActionPrioritizedSelector(int curIdx = -1, int lastIdx = -1){
            currentSelectedIndex = curIdx;
            lastSelectedIndex = lastIdx;
        }
    }
}