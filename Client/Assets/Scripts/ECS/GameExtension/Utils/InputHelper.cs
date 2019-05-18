using System.Collections.Generic;
using Lockstep.Math;
using NetMsg.Game.Tank;
using UnityEngine;
using Input = UnityEngine.Input;

namespace Lockstep.Game {
    public class InputHelper {
        public static int GetDeg(EDir dir){
            switch (dir) {
                case EDir.Up: return 0;
                case EDir.Left: return 90;
                case EDir.Down: return 180;
                case EDir.Right: return 270;
            }

            return 0;
        }

        public static LVector2 GetVector(int deg){
            switch (deg) {
                case 0: return LVector2.up;
                case 90: return LVector2.left;
                case 180: return LVector2.down;
                case 270: return LVector2.right;
            }

            return LVector2.zero;
        }

        public static LVector2 GetVector(ECmdType deg){
            switch (deg) {
                case ECmdType.Up: return LVector2.up;
                case ECmdType.Right: return LVector2.left;
                case ECmdType.Down: return LVector2.down;
                case ECmdType.Left: return LVector2.right;
            }

            return LVector2.zero;
        }

        public enum ECmdType : byte {
            Up,
            Right,
            Down,
            Left,
            Fire
        }


        public static partial class BitHelper {
            public static byte ToByte(ECmdType idx){
                return (byte) (1 << (byte) idx);
            }

            public static bool HasBit(byte val, ECmdType idx){
                return (val & 1 << (byte) idx) != 0;
            }
        }


        private static float lastFireTimer;
        private static float fireCD = 1;
        
        
        public static List<InputCmd> GetInputCmds(){
            var cmds = new List<InputCmd>();
            var isFire = Input.GetKey(KeyCode.Space);
            var dir = ECmdType.Up;
            if (Input.GetKey(KeyCode.W)) {
                dir = ECmdType.Up;
            }
            else if (Input.GetKey(KeyCode.D)) {
                dir = ECmdType.Right;
            }
            else if (Input.GetKey(KeyCode.S)) {
                dir = ECmdType.Down;
            }
            else if (Input.GetKey(KeyCode.A)) {
                dir = ECmdType.Left;
            }
            else {
                dir = ECmdType.Fire;
            }

            if (dir != ECmdType.Fire) {
                cmds.Add(new InputCmd() {type = BitHelper.ToByte(dir)});
            }

            if (isFire && lastFireTimer < Time.realtimeSinceStartup) {
                lastFireTimer = Time.realtimeSinceStartup + 4;
                cmds.Add(new InputCmd() {type = BitHelper.ToByte(ECmdType.Fire)});
            }

            return cmds;
        }

        public static void Execute(InputCmd cmd, InputEntity entity){
            var type = (ECmdType) cmd.type;
            if (BitHelper.HasBit(cmd.type, ECmdType.Up)) entity.AddMoveDir(LVector2.up);
            if (BitHelper.HasBit(cmd.type, ECmdType.Left)) entity.AddMoveDir(LVector2.left);
            if (BitHelper.HasBit(cmd.type, ECmdType.Down)) entity.AddMoveDir(LVector2.down);
            if (BitHelper.HasBit(cmd.type, ECmdType.Right)) entity.AddMoveDir(LVector2.right);
            if (BitHelper.HasBit(cmd.type, ECmdType.Fire)) {
                entity.AddEntityConfigId(0);
                entity.AddCoordinate(LVector2.zero);
            }
        }
    }
}