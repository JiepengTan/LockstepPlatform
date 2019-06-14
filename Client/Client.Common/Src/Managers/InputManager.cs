using System;
using System.Collections.Generic;
using Entitas;
using Lockstep.Math;
using NetMsg.Common;
using UnityEngine;

namespace Lockstep.Game {
    [System.Serializable]
    public class InputManager : SingletonManager<InputManager>, IInputService {
        public enum ECmdType : byte {
            Up,
            Right,
            Down,
            Left,
            Fire
        }


        public List<InputCmd> GetInputCmds(){
            var cmds = new List<InputCmd>();
            var isFire = UnityEngine.Input.GetKey(KeyCode.Space);
            var dir = ECmdType.Up;
            if (UnityEngine.Input.GetKey(KeyCode.W)) {
                dir = ECmdType.Up;
            }
            else if (UnityEngine.Input.GetKey(KeyCode.D)) {
                dir = ECmdType.Right;
            }
            else if (UnityEngine.Input.GetKey(KeyCode.S)) {
                dir = ECmdType.Down;
            }
            else if (UnityEngine.Input.GetKey(KeyCode.A)) {
                dir = ECmdType.Left;
            }
            else {
                dir = ECmdType.Fire;
            }

            if (dir != ECmdType.Fire) {
                cmds.Add(new InputCmd(BitUtil.ToByte(dir)));
            }

            if (isFire) {
                cmds.Add(new InputCmd(BitUtil.ToByte(ECmdType.Fire)));
            }

            return cmds;
        }

        public static partial class BitUtil {
            public static byte ToByte(ECmdType idx){
                return (byte) (1 << (byte) idx);
            }

            public static bool HasBit(byte val, ECmdType idx){
                return (val & (1 << (byte) idx)) != 0;
            }
        }

        public void Execute(InputCmd cmd, IEntity sentity){
            if (cmd.content == null) {
                return;
            }

            var entity = sentity as InputEntity;

            var type = cmd.content[0];
            if (BitUtil.HasBit(type, ECmdType.Up)) entity.AddMoveDir(EDir.Up);
            if (BitUtil.HasBit(type, ECmdType.Left)) entity.AddMoveDir(EDir.Left);
            if (BitUtil.HasBit(type, ECmdType.Down)) entity.AddMoveDir(EDir.Down);
            if (BitUtil.HasBit(type, ECmdType.Right)) entity.AddMoveDir(EDir.Right);
            if (BitUtil.HasBit(type, ECmdType.Fire)) {
                entity.isFire = true;
            }
        }
    }
}