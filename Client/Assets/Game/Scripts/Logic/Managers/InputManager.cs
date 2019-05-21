using System;
using System.Collections.Generic;
using Lockstep.Math;
using NetMsg.Game;
using UnityEngine;

namespace Lockstep.Game {
    [Serializable]
    public class InputInfo {
        public float horizontal; //Float that stores horizontal input
        public float vertical; //Float that stores horizontal input
        public bool fireHeld; //Bool that stores jump pressed
        public bool firePressed; //Bool that stores jump held

        public void ClearInput(){
            //Reset all inputs
            horizontal = 0;
            vertical = 0;
            firePressed = false;
            fireHeld = false;
        }
    }

    [System.Serializable]
    public class InputManager : SingletonManager<InputManager>,IInputService {
        public List<InputInfo> inputs = new List<InputInfo>();

        bool readyToClear; //Bool used to keep input in sync


        public override void DoStart(){
            base.DoStart();
            inputs.Clear();
            inputs.Add(new InputInfo());
            inputs.Add(new InputInfo());
        }

        void ProcessInputs(){
            //player1 input
            var input = inputs[0];
            input.horizontal = UnityEngine.Input.GetKey(KeyCode.D) ? 1 : (UnityEngine.Input.GetKey(KeyCode.A) ? -1 : 0);
            input.vertical = UnityEngine.Input.GetKey(KeyCode.W) ? 1 : (UnityEngine.Input.GetKey(KeyCode.S) ? -1 : 0);
            input.firePressed = UnityEngine.Input.GetButtonDown("Jump");
            input.fireHeld = UnityEngine.Input.GetButton("Jump");
            input.horizontal = Mathf.Clamp(input.horizontal, -1f, 1f);
            input.vertical = Mathf.Clamp(input.vertical, -1f, 1f);

            //player2 input
            input = inputs[1];
            input.horizontal = UnityEngine.Input.GetKey(KeyCode.RightArrow) ? 1 : (UnityEngine.Input.GetKey(KeyCode.LeftArrow) ? -1 : 0);
            input.vertical = UnityEngine.Input.GetKey(KeyCode.UpArrow) ? 1 : (UnityEngine.Input.GetKey(KeyCode.DownArrow) ? -1 : 0);
            input.firePressed = UnityEngine.Input.GetKey(KeyCode.Keypad0);
            input.fireHeld = UnityEngine.Input.GetKeyDown(KeyCode.Keypad0);

            input.horizontal = Mathf.Clamp(input.horizontal, -1f, 1f);
            input.vertical = Mathf.Clamp(input.vertical, -1f, 1f);
        }


        public override void DoUpdate(float deltaTime){
            ClearInput();

            //if (main.IsGameOver())
            //    return;

            ProcessInputs();
        }

        //public override void DoFixedUpdate(){
        //    readyToClear = true;
        //}

        void ClearInput(){
            if (!readyToClear)
                return;
            foreach (var input in inputs) {
                input.ClearInput();
            }

            readyToClear = false;
        }
        
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


        public static partial class BitUtil {
            public static byte ToByte(ECmdType idx){
                return (byte) (1 << (byte) idx);
            }

            public static bool HasBit(byte val, ECmdType idx){
                return (val & 1 << (byte) idx) != 0;
            }
        }


        private static float lastFireTimer;
        private static float fireCD = 1;
        
        
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
                cmds.Add(new InputCmd() {type = BitUtil.ToByte(dir)});
            }

            if (isFire && lastFireTimer < Time.realtimeSinceStartup) {
                lastFireTimer = Time.realtimeSinceStartup + 4;
                cmds.Add(new InputCmd() {type = BitUtil.ToByte(ECmdType.Fire)});
            }

            return cmds;
        }

        public void Execute(InputCmd cmd, InputEntity entity){
            var type = (ECmdType) cmd.type;
            if (BitUtil.HasBit(cmd.type, ECmdType.Up)) entity.AddMoveDir(  EDir.Up);
            if (BitUtil.HasBit(cmd.type, ECmdType.Left)) entity.AddMoveDir(EDir.Left);
            if (BitUtil.HasBit(cmd.type, ECmdType.Down)) entity.AddMoveDir(EDir.Down);
            if (BitUtil.HasBit(cmd.type, ECmdType.Right)) entity.AddMoveDir(EDir.Right);
            if (BitUtil.HasBit(cmd.type, ECmdType.Fire)) {
                entity.isFire = true;
            }
        }
    }
}