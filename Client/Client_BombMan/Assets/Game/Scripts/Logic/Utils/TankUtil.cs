using Lockstep.ECS.Game;
using Lockstep.Math;

namespace Lockstep.Game {
    public static class TankUtil {
        public static LVector2 UNIT_SIZE = LVector2.half;
        public static LFloat TANK_HALF_LEN = LFloat.half;
        public static LFloat FORWARD_HEAD_DIST = new LFloat(true, 20);
        public static LFloat SNAP_DIST = new LFloat(true, 400);

        public static LFloat TANK_BORDER_SIZE =new LFloat(true, 450);
        
        
        private static LFloat TargetDist = new LFloat(true,100);
        public static LFloat sqrtTargetDist = TargetDist * TargetDist;
        public static LVector2 GetHeadPos(LVector2 pos, EDir dir){
            var dirVec = DirUtil.GetDirLVec(dir);
            var fTargetHead = pos + (TANK_HALF_LEN + FORWARD_HEAD_DIST) * dirVec;
            return fTargetHead;
        }
        public static LVector2 GetHeadPos(LVector2 pos, EDir dir, LFloat len){
            var dirVec = DirUtil.GetDirLVec(dir);
            var fTargetHead = pos + (TANK_HALF_LEN + len) * dirVec;
            return fTargetHead;
        }
    }
}