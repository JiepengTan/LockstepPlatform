using System;
using System.Collections.Generic;
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
    public class InputManager : SingletonManager<InputManager> {
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
            input.horizontal = Input.GetKey(KeyCode.D) ? 1 : (Input.GetKey(KeyCode.A) ? -1 : 0);
            input.vertical = Input.GetKey(KeyCode.W) ? 1 : (Input.GetKey(KeyCode.S) ? -1 : 0);
            input.firePressed = Input.GetButtonDown("Jump");
            input.fireHeld = Input.GetButton("Jump");
            input.horizontal = Mathf.Clamp(input.horizontal, -1f, 1f);
            input.vertical = Mathf.Clamp(input.vertical, -1f, 1f);

            //player2 input
            input = inputs[1];
            input.horizontal = Input.GetKey(KeyCode.RightArrow) ? 1 : (Input.GetKey(KeyCode.LeftArrow) ? -1 : 0);
            input.vertical = Input.GetKey(KeyCode.UpArrow) ? 1 : (Input.GetKey(KeyCode.DownArrow) ? -1 : 0);
            input.firePressed = Input.GetKey(KeyCode.Keypad0);
            input.fireHeld = Input.GetKeyDown(KeyCode.Keypad0);

            input.horizontal = Mathf.Clamp(input.horizontal, -1f, 1f);
            input.vertical = Mathf.Clamp(input.vertical, -1f, 1f);
        }


        public override void DoUpdate(float deltaTime){
            ClearInput();

            if (main.IsGameOver())
                return;

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
    }
}