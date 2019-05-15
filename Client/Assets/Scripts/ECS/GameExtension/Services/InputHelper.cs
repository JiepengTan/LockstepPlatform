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
        public static List<ICommand> GetInputCmds(){
            var cmds = new List<ICommand>();
            var isFire = Input.GetKey(KeyCode.Space);
            var dir = EDir.Up;
            if (Input.GetKey(KeyCode.W)) {
                dir = EDir.Up;
            }
            else  if (Input.GetKey(KeyCode.D)) {
                dir = EDir.Right;
            }
            else  if (Input.GetKey(KeyCode.S)) {
                dir = EDir.Down;
            }
            else  if (Input.GetKey(KeyCode.A)) {
                dir = EDir.Left;
            }
            else {
                dir = EDir.EnumCount;
            }
            if (dir != EDir.EnumCount) {
                cmds.Add(new CmdDir(){deg = GetDeg(dir)});
            }

            if (isFire && lastFireTimer < Time.realtimeSinceStartup) {
                lastFireTimer = Time.realtimeSinceStartup + fireCD;
                cmds.Add(new CmdFire(){skillID =  1});
            }

            return cmds;
        }

        private static float lastFireTimer;
        private static float fireCD = 1;

        public static void Execute(ICommand cmd, InputEntity entity){
            var type = (ECmdType) cmd.type;
            switch (type) {
                case ECmdType.Dir:
                    entity.AddMoveDir(GetVector((cmd as CmdDir).deg));
                    break;
                case ECmdType.Fire:
                    entity.AddEntityConfigId(0);
                    entity.AddCoordinate(LVector2.zero);
                    break;
            }
            
        }
    }
}