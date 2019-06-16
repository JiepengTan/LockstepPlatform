using System.Collections.Generic;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using Lockstep.Game;
using Lockstep.Math;


namespace Lockstep.Game {
    public static class GameCollisionUtil {
        public static LFloat RoundIfNear(LFloat val, LFloat roundDist){
            var roundVal = LMath.Round(val);
            var diff = LMath.Abs(val - roundVal);
            if (diff < roundDist) {
                return roundVal;
            }

            return val;
        }

        public static bool CheckCollision(GameEntity a, GameEntity b){
            var cola = a.collider;
            var colb = b.collider;
            var posa = a.pos.value;
            var posb = b.pos.value;
            return GameCollisionUtil.CheckCollision(
                posa, cola.radius, cola.size,
                posb, colb.radius, colb.size);
        }

        public static bool CheckCollision(LVector2 posA, LFloat rA, LVector2 sizeA, LVector2 posB, LFloat rB,
            LVector2 sizeB){
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
}