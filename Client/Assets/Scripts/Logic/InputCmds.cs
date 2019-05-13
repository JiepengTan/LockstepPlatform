using Lockstep.Logging;
using Lockstep.Math;
using NetMsg.Game.Tank;

namespace Lockstep.Game {
    public interface  IInputCommand {
        void Execute(BasePlayer player,LFloat deltaTime);
    }

    public enum EInputKeys {
        Up,
        Right,
        Down,
        Left,
        Fire,
        Jump
    }
    public class InputHelper {
        public static IInputCommand[] EmptyInputs = new IInputCommand[0];

        public static IInputCommand[] ParseCmd(PlayerInput input){
            IInputCommand[] retVal = new IInputCommand[input.allInputs.Length];
            for (int i = 0; i < input.allInputs.Length; i++) {
                var inputCmd = input.allInputs[i];
                if ( inputCmd== null) break;
                retVal[i] = ParseCmd(inputCmd.key, inputCmd.val1,inputCmd.val2);
            }
            return retVal;
        }

        static IInputCommand ParseCmd(int key, int val1,int val2){
            var type = (EInputKeys) key;
            switch (type) {
                case EInputKeys.Up:
                    return new IInputCmdDir(LVector2.up);
                    break;
                case EInputKeys.Right:
                    return new IInputCmdDir(LVector2.right);
                    break;
                case EInputKeys.Down:
                    return new IInputCmdDir(LVector2.down);
                    break;
                case EInputKeys.Left:
                    return new IInputCmdDir(LVector2.left);
                    break;
                case EInputKeys.Fire:
                     return new IInputCmdFire(val1);
                    break;
            }

            return null;
        }
    }


    public class IInputCmdDir : IInputCommand {
        public LVector2 dir;

        public IInputCmdDir(LVector2 val){
            dir = val;
        }

        public void Execute(BasePlayer player,LFloat deltaTime){
            player.Position = player.Position + dir * deltaTime;
            Debug.Log($"Player{player.Name} Move {dir}");
        }
    }

    public class IInputCmdFire : IInputCommand {
        public int skillID;

        public IInputCmdFire(int val){
            skillID = val;
        }

        public void Execute(BasePlayer player,LFloat deltaTime){
            Debug.Log($"Player{player.Name} Fire ");
        }
    }
}