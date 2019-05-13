using System.Runtime.Serialization;
using Lockstep.Logging;
using Lockstep.Math;
using Lockstep.Serialization;
using NetMsg.Game.Tank;
using ISerializable = Lockstep.Serialization.ISerializable;

namespace Lockstep.Game {
    public interface ICommand : ISerializable {
        void Execute(object player);
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
        public static ICommand[] Empty = new ICommand[0];

        public static ICommand[] ParseCmd(Input input){
            ICommand[] retVal = new ICommand[input.Commands.Count];
            for (int i = 0; i < input.Commands.Count; i++) {
                var inputCmd = input.Commands[i];
                if (inputCmd == null) break;
                //retVal[i] = ParseCmd(inputCmd.key, inputCmd.val1, inputCmd.val2);
            }

            return retVal;
        }

        static ICommand ParseCmd(int key, int val1, int val2){
            var type = (EInputKeys) key;
            switch (type) {
                case EInputKeys.Up:
                    return new CmdDir(LVector2.up);
                    break;
                case EInputKeys.Right:
                    return new CmdDir(LVector2.right);
                    break;
                case EInputKeys.Down:
                    return new CmdDir(LVector2.down);
                    break;
                case EInputKeys.Left:
                    return new CmdDir(LVector2.left);
                    break;
                case EInputKeys.Fire:
                    return new CmdFire(val1);
                    break;
            }

            return null;
        }
    }

    public class BaseCmd : BaseFormater, ICommand {
        public void Execute(object entity){ }
    }

    public class CmdDir : BaseCmd {
        public LVector2 dir;

        public CmdDir(LVector2 val){
            dir = val;
        }

        public void Execute(object entity){
            var player = (Entity) entity;
            //player.Position = player.Position + dir * deltaTime;
            //Debug.Log($"Player{player.Name} Move {dir}");
        }

        public override void Serialize(Serializer writer){ }

        public override void Deserialize(Deserializer reader){ }
    }

    public class CmdFire : BaseCmd {
        public int skillID;

        public CmdFire(int val){
            skillID = val;
        }

        public void Execute(object entity){
            //Debug.Log($"Player{player.Name} Fire ");
        }
        public override void Serialize(Serializer writer){ }

        public override void Deserialize(Deserializer reader){ }
    }
}