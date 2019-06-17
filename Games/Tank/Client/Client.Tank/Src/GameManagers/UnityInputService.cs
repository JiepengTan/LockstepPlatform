using System.Collections.Generic;
using Entitas;
using NetMsg.Common;
using UnityEngine;

namespace Lockstep.Game {
    [System.Serializable]
    public class UnityInputService : UnityBaseService, IInputService {
        public List<InputCmd> GetInputCmds(){
            var cmds = new List<InputCmd>();
            var isFire = UnityEngine.Input.GetKey(KeyCode.Space);
            var dir = EInputCmdType.Up;
            if (UnityEngine.Input.GetKey(KeyCode.W)) {
                dir = EInputCmdType.Up;
            }
            else if (UnityEngine.Input.GetKey(KeyCode.D)) {
                dir = EInputCmdType.Right;
            }
            else if (UnityEngine.Input.GetKey(KeyCode.S)) {
                dir = EInputCmdType.Down;
            }
            else if (UnityEngine.Input.GetKey(KeyCode.A)) {
                dir = EInputCmdType.Left;
            }
            else {
                dir = EInputCmdType.Fire;
            }

            if (dir != EInputCmdType.Fire) {
                cmds.Add(new InputCmd(EnumBitUtil.ToByte(dir)));
            }

            if (isFire) {
                cmds.Add(new InputCmd(EnumBitUtil.ToByte(EInputCmdType.Fire)));
            }

            return cmds;
        }

        public void Execute(InputCmd cmd, IEntity sentity){
            InputUtil.Execute(cmd, sentity);
        }
    }
}