using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using Lockstep.Game;
using Random = UnityEngine.Random;

public static class Vector2Extension {
    public static Vector2Int Floor(this Vector2 vec){
        return new Vector2Int(Mathf.FloorToInt(vec.x), Mathf.FloorToInt(vec.y));
    }
}

public static class CollisionHelper {
    public static Vector2Int GetDirVec(EDir dir){
        switch (dir) {
            case EDir.Up: return Vector2Int.up;
            case EDir.Right: return Vector2Int.right;
            case EDir.Down: return Vector2Int.down;
            case EDir.Left: return Vector2Int.left;
        }

        return Vector2Int.up;
    }

    public static int GetDirDeg(EDir dir){
        return ((int) dir) * 90;
    }

    public static Vector2Int GetBorderDir(EDir dir){
        var isUpDown = (int) (dir) % 2 == 0;
        var borderDir = Vector2Int.up;
        if (isUpDown) {
            borderDir = Vector2Int.right;
        }

        return borderDir;
    }

    public const float TANK_BORDER_SIZE = 0.9f;

    public static float RoundIfNear(float val, float roundDist){
        var roundVal = Mathf.Round(val);
        var diff = Mathf.Abs(val - roundVal);
        if (diff < roundDist) {
            return roundVal;
        }

        return val;
    }

    public static float GetMaxMoveDist(EDir dir, Vector2 fHeadPos, Vector2 fTargetHeadPos,
        float borderSize = TANK_BORDER_SIZE){
        var iTargetHeadPos = new Vector2Int(Mathf.FloorToInt(fTargetHeadPos.x), Mathf.FloorToInt(fTargetHeadPos.y));
        var hasCollider = HasColliderWithBorder(dir, fTargetHeadPos, borderSize);
        var maxMoveDist = float.MaxValue;
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

    public static bool HasColliderWithBorder(EDir dir, Vector2 fTargetHead, float size = TANK_BORDER_SIZE){
        Vector2 borderDir = GetBorderDir(dir);
        var fBorder1 = fTargetHead + borderDir * size;
        var fBorder2 = fTargetHead - borderDir * size;
        var isColHead = HasCollider(fTargetHead);
        var isColBorder1 = HasCollider(fBorder1);
        var isColBorder2 = HasCollider(fBorder2);
        return isColHead
               || isColBorder1
               || isColBorder2;
    }

    public static List<Vector3Int> DebugQueryCollider(EDir dir, Vector2 fTargetHead, float size = TANK_BORDER_SIZE){
        var ret = new List<Vector3Int>();
        Vector2 borderDir = GetBorderDir(dir);
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

    public static bool HasCollider(Vector2 pos){
        var iPos = pos.Floor();
        var id = LevelManager.Instance.Pos2TileID(iPos, true);
        return id != 0;
    }

    public static bool IsOutOfBound(Vector2 fpos, Vector2 min, Vector2 max){
        var pos = fpos.Floor();
        if (pos.x < min.x || pos.x > max.x
                          || pos.y < min.y || pos.y > max.y
        ) {
            return true;
        }

        return false;
    }

    public static bool CheckCollision(Unit a, Unit b){
        return CheckCollision(a.pos, a.radius, a.size, b.pos, b.radius, b.size);
    }

    public static bool CheckCollision(Unit a, Vector2Int tilePos){
        return CheckCollision(a.pos, a.radius, a.size,
            tilePos + Vector2.one * 0.5f,
            0.7072f, Vector2.one * 0.5f);
    }

    public static bool CheckCollision(Vector2 posA, float rA, Vector2 sizeA, Vector2 posB, float rB, Vector2 sizeB){
        var diff = posA - posB;
        var allRadius = rA + rB;
        //circle 判定
        if (diff.sqrMagnitude > allRadius * allRadius) {
            return false;
        }

        var isBoxA = sizeA != Vector2.zero;
        var isBoxB = sizeB != Vector2.zero;
        if (!isBoxA && !isBoxB)
            return true;
        var absX = Mathf.Abs(diff.x);
        var absY = Mathf.Abs(diff.y);
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

            var x = Mathf.Max(absX - size.x, 0);
            var y = Mathf.Max(absY - size.y, 0);
            return x * x + y * y < radius * radius;
        }
    }
}