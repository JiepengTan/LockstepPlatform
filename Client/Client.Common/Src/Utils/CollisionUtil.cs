using System.Collections.Generic;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    public class CollisionUtil__ {
        public static List<Vector3Int> DebugQueryCollider(EDir dir, LVector2 fTargetHead){
            return DebugQueryCollider(dir, fTargetHead,CollisionUtil. TANK_BORDER_SIZE);
        }

        public static List<Vector3Int> DebugQueryCollider(EDir dir, LVector2 fTargetHead, LFloat size ){
            var ret = new List<Vector3Int>();
            LVector2 borderDir = DirUtil.GetBorderDir(dir);
            var fBorder1 = fTargetHead + borderDir * size;
            var fBorder2 = fTargetHead - borderDir * size;
            var isColHead = CollisionUtil.HasCollider(fTargetHead);
            var isColBorder1 = CollisionUtil.HasCollider(fBorder1);
            var isColBorder2 = CollisionUtil.HasCollider(fBorder2);
            ret.Add(new Vector3Int(fTargetHead.Floor().x, fTargetHead.Floor().y, isColHead ? 1 : 0));
            ret.Add(new Vector3Int(fBorder1.Floor().x, fBorder1.Floor().y, isColBorder1 ? 1 : 0));
            ret.Add(new Vector3Int(fBorder2.Floor().x, fBorder2.Floor().y, isColBorder2 ? 1 : 0));
            return ret;
        }
    }
}