using System.Collections.Generic;
using Entitas;
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

        public static List<ICommand> GetInputCmds(){
            var cmds = new List<ICommand>();
            var vert = UnityEngine.Input.GetAxis("Vertical");
            var horz = UnityEngine.Input.GetAxis("Horizontal");
            var isFire = Input.GetKey(KeyCode.Space);
            var absv = Mathf.Abs(vert);
            var absh = Mathf.Abs(horz);
            var dir = (absv > absh ? (vert > 0 ? EDir.Up : EDir.Down) : (horz > 0 ? EDir.Right : EDir.Left));
            const float MIN_INPUT_VAL = 0.01f;
            if (absv > MIN_INPUT_VAL || absh < MIN_INPUT_VAL) {
                cmds.Add(new CmdDir(){deg = GetDeg(dir)});
            }

            if (isFire) {
                cmds.Add(new CmdFire(){skillID =  1});
            }

            return cmds;
        }

        public static void Execute(ICommand cmd, InputEntity entity){
            var type = (ECmdType) cmd.type;
            switch (type) {
                case ECmdType.Dir:
                    //entity.add
                    break;
                case ECmdType.Fire:
                    entity.AddEntityConfigId(0);
                    entity.AddCoordinate(LVector2.zero);
                    break;
            }
            
        }
    }
}