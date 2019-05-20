using Lockstep.ECS.Game;
using Lockstep.Math;

namespace Lockstep.Game {
    public static class TankUtil {
        public static LFloat TANK_HALF_LEN = new LFloat(1);
        public static LFloat FORWARD_HEAD_DIST = new LFloat(true, 0.05f);
        public static LFloat SNAP_DIST = new LFloat(true, 0.4f);

        public static LVector2 FireOffsetPos(EDir dir){
            return DirUtil.GetDirLVec(dir);
        }
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
        
        public static void DoUpdate(LFloat deltaTime,GameEntity entity){
            
        }
    }
}