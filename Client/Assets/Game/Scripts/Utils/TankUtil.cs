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
            var dir = entity.dir.value;
            var pos = entity.position.value;
            var moveSpd = entity.move.moveSpd;
            var camp = entity.unit.camp;
            var isChangedDir = entity.move.isChangedDir;
            
            //can move 判定
            var dirVec = DirUtil.GetDirVec(dir).ToLVector2();
            var moveDist = (moveSpd * deltaTime);
            var fTargetHead = pos + (TANK_HALF_LEN + moveDist) *  dirVec;
            var fPreviewHead = pos + (TANK_HALF_LEN + FORWARD_HEAD_DIST) *  dirVec;

            LFloat maxMoveDist = moveSpd * deltaTime;
            var headPos = pos + (TANK_HALF_LEN) *  dirVec;
            var dist = CollisionUtil.GetMaxMoveDist(dir, headPos, fTargetHead);
            var dist2 = CollisionUtil.GetMaxMoveDist(dir, headPos, fPreviewHead);
            maxMoveDist = LMath.Min(maxMoveDist, dist, dist2);

            var diffPos = maxMoveDist *  dirVec;
            pos = pos + diffPos;
            if (camp == ECampType.Player) {
                if (isChangedDir) {
                    var idir = (int) (dir);
                    var isUD = idir % 2 == 0;
                    if (isUD) {
                        pos.x = CollisionUtil.RoundIfNear(pos.x, SNAP_DIST);
                    }
                    else {
                        pos.y = CollisionUtil.RoundIfNear(pos.y, SNAP_DIST);
                    }
                }
            }

            entity.position.value = pos;
            entity.move.isChangedDir = false;
        }
    }
}