using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using Lockstep.Game;
using Lockstep.Math;
using Random = UnityEngine.Random;

public static class LVector2Extension {
    public static Vector2Int Floor(this Vector2 vec){
        return new Vector2Int(Mathf.FloorToInt(vec.x),Mathf.FloorToInt(vec.y));
    }  
    public static Vector2Int Floor(this LVector2 vec){
        return vec.ToVector2Int();
    }
}

public static class CollisionUtil {
    
    public static LFloat TANK_BORDER_SIZE =new LFloat(true, 900);
    
  


    public static LFloat RoundIfNear(LFloat val, LFloat roundDist){
        var roundVal = LMath.Round(val);
        var diff = LMath.Abs(val - roundVal);
        if (diff < roundDist) {
            return roundVal;
        }

        return val;
    }

    public static LFloat GetMaxMoveDist(EDir dir, LVector2 fHeadPos, LVector2 fTargetHeadPos){
        return GetMaxMoveDist(dir, fHeadPos, fTargetHeadPos, TANK_BORDER_SIZE);
    }

    public static LFloat GetMaxMoveDist(EDir dir, LVector2 fHeadPos, LVector2 fTargetHeadPos,LFloat borderSize ){
        var iTargetHeadPos = new Vector2Int(LMath.FloorToInt(fTargetHeadPos.x), LMath.FloorToInt(fTargetHeadPos.y));
        var hasCollider = HasColliderWithBorder(dir, fTargetHeadPos, borderSize);
        var maxMoveDist = LFloat.MaxValue;
        if (hasCollider) {
            switch (dir) {
                case EDir.Up:
                    maxMoveDist = iTargetHeadPos.y - fHeadPos.y;
                    break;
                case EDir.Right:
                    maxMoveDist = iTargetHeadPos.x - fHeadPos.x;
                    break;
                case EDir.Down:
                    maxMoveDist = fHeadPos.y - iTargetHeadPos.y - 1;
                    break;
                case EDir.Left:
                    maxMoveDist = fHeadPos.x - iTargetHeadPos.x - 1;
                    break;
            }
        }

        return maxMoveDist;
    }

    public static bool HasColliderWithBorder(EDir dir, LVector2 fTargetHead){
        return HasColliderWithBorder(dir, fTargetHead, TANK_BORDER_SIZE);
    }

    public static bool HasColliderWithBorder(EDir dir, LVector2 fTargetHead, LFloat size){
        LVector2 borderDir =DirUtil. GetBorderDir(dir).ToLVector2();
        var fBorder1 = fTargetHead + borderDir * size;
        var fBorder2 = fTargetHead - borderDir * size;
        var isColHead = HasCollider(fTargetHead);
        var isColBorder1 = HasCollider(fBorder1);
        var isColBorder2 = HasCollider(fBorder2);
        return isColHead
               || isColBorder1
               || isColBorder2;
    }

    public static List<Vector3Int> DebugQueryCollider(EDir dir, LVector2 fTargetHead){
        return DebugQueryCollider(dir, fTargetHead, TANK_BORDER_SIZE);
    }

    public static List<Vector3Int> DebugQueryCollider(EDir dir, LVector2 fTargetHead, LFloat size ){
        var ret = new List<Vector3Int>();
        LVector2 borderDir = DirUtil.GetBorderDir(dir).ToLVector2();
        var fBorder1 = fTargetHead + borderDir * size;
        var fBorder2 = fTargetHead - borderDir * size;
        var isColHead = HasCollider(fTargetHead);
        var isColBorder1 = HasCollider(fBorder1);
        var isColBorder2 = HasCollider(fBorder2);
        ret.Add(new Vector3Int(fTargetHead.Floor().x, fTargetHead.Floor().y, isColHead ? 1 : 0));
        ret.Add(new Vector3Int(fBorder1.Floor().x, fBorder1.Floor().y, isColBorder1 ? 1 : 0));
        ret.Add(new Vector3Int(fBorder2.Floor().x, fBorder2.Floor().y, isColBorder2 ? 1 : 0));
        return ret;
    }

    public static bool HasCollider(LVector2 pos){
        var iPos = pos.Floor();
        var id = MapManager.Instance.Pos2TileID(iPos, true);
        return id != 0;
    }

    public static bool IsOutOfBound(LVector2 fpos, LVector2 min, LVector2 max){
        var pos = fpos.Floor();
        if (pos.x < min.x || pos.x > max.x
                          || pos.y < min.y || pos.y > max.y
        ) {
            return true;
        }

        return false;
    }

  
    public static bool CheckCollision(GameEntity a, GameEntity b){
        var cola = a.collider;
        var colb = b.collider;
        var posa = a.move.pos;
        var posb = b.move.pos;
        return CollisionUtil.CheckCollision(
            posa, cola.radius, cola.size,
            posb, colb.radius, colb.size);
    }
    public static bool CheckCollision(LVector2 posA, LFloat rA, LVector2 sizeA, LVector2 posB, LFloat rB, LVector2 sizeB){
        var diff = posA - posB;
        var allRadius = rA + rB;
        //circle 判定
        if (diff.sqrMagnitude > allRadius * allRadius) {
            return false;
        }

        var isBoxA = sizeA != LVector2.zero;
        var isBoxB = sizeB != LVector2.zero;
        if (!isBoxA && !isBoxB)
            return true;
        var absX = LMath.Abs(diff.x);
        var absY = LMath.Abs(diff.y);
        if (isBoxA && isBoxB) {
            //AABB and AABB
            var allSize = sizeA + sizeB;
            if (absX > allSize.x) return false;
            if (absY > allSize.y) return false;
            return true;
        }
        else {
            //AABB & circle
            var size = sizeB;
            var radius = rA;
            if (isBoxA) {
                size = sizeA;
                radius = rB;
            }

            var x = LMath.Max(absX - size.x, LFloat.zero);
            var y = LMath.Max(absY - size.y, LFloat.zero);
            return x * x + y * y < radius * radius;
        }
    }
}