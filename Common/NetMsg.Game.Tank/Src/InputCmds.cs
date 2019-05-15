using System.Runtime.Serialization;
using Lockstep.Logging;
using Lockstep.Serialization;
using NetMsg.Game.Tank;
using ISerializable = Lockstep.Serialization.ISerializable;

namespace NetMsg.Game.Tank {
    public interface ICommand : ISerializable {
        byte type { get; }
        bool IsSame(ICommand other);
    }

    public enum ECmdType {
        Dir,
        Fire,
        Jump,
        EnumCount,
    }


    public class BaseCmd : BaseFormater, ICommand {
        public virtual byte type { get; }
        public void Execute(object entity){ }

        public static BaseCmd Parse(Deserializer reader){
            var itype = reader.GetByte();
            var type = (ECmdType) itype;
            BaseCmd val = null;
            switch (type) {
                case ECmdType.Dir: {
                    val = reader.Parse<CmdDir>();
                    break;
                }
                case ECmdType.Fire: {
                    val = reader.Parse<CmdFire>();
                    break;
                }
                default:
                    Debug.LogError("Unknow InputCmd type" + itype);
                    break;
            }

            return val;
        }

        public static bool IsSame(ICommand a, ICommand b){
            if ((a == null) != (b == null)) return false;
            if (a == null) return true;
            return a.IsSame(b);
        }

        public virtual bool IsSame(ICommand other){
            if (other == null) return false;
            if (GetType() != other.GetType())
                return false;
            return IsSameAttri(other);
        }

        protected virtual bool IsSameAttri(ICommand other){
            return true;
        }
    }

    public class CmdDir : BaseCmd {
        public override byte type {
            get { return (byte) ECmdType.Dir; }
        }

        public int deg;

        public override void Serialize(Serializer writer){
            writer.Put(type);
            writer.Put(deg);
        }

        public override void Deserialize(Deserializer reader){
            deg = reader.GetInt();
        }

        protected override bool IsSameAttri(ICommand other){
            var cmd = other as CmdDir;
            return this.deg == cmd.deg;
        }
    }

    public class CmdFire : BaseCmd {
        public override byte type {
            get { return (byte) ECmdType.Fire; }
        }

        public int skillID;

        public override void Serialize(Serializer writer){
            writer.Put(type);
            writer.Put(skillID);
        }

        public override void Deserialize(Deserializer reader){
            skillID = reader.GetInt();
        }

        protected override bool IsSameAttri(ICommand other){
            var cmd = other as CmdFire;
            return this.skillID == cmd.skillID;
        }
    }
}