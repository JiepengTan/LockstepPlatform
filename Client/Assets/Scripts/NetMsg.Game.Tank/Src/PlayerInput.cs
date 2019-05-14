#define DEBUG_FRAME_DELAY
using System.Collections.Generic;
using Lockstep.Game;
using Lockstep.Serialization;

namespace NetMsg.Game.Tank{
    public partial class PlayerInput : BaseFormater{
        public byte ActorId;
        public uint Tick;
        public List<ICommand> Commands = new List<ICommand>();
#if DEBUG_FRAME_DELAY
        public float timeSinceStartUp;
#endif
        public PlayerInput( uint tick,byte actorID, List<ICommand> inputs){
            this.Tick = tick;
            this.ActorId = actorID;
            if (inputs != null) {
                this.Commands.AddRange(inputs);    
            }
        }

        public PlayerInput(){ }

        /// <summary>
        /// TODO     合并 输入
        /// </summary>
        /// <param name="inputb"></param>
        public void Combine(PlayerInput inputb){ }

        public override void Serialize(Serializer writer){
#if DEBUG_FRAME_DELAY
            writer.Put(timeSinceStartUp);
#endif
            writer.Put(ActorId);
            int count = Commands.Count;
            writer.Put((byte) count);
            for (int i = 0; i < count; i++) {
                Commands[i].Serialize(writer);
            }
        }

        public override void Deserialize(Deserializer reader){
#if DEBUG_FRAME_DELAY
            timeSinceStartUp = reader.GetFloat();
#endif
            ActorId = reader.GetByte();
            int count = reader.GetByte();
            for (int i = 0; i < count; i++) {
                var cmd = BaseCmd.Parse(reader);
                Commands.Add(cmd);
            }
        }
        
        public bool IsSame(PlayerInput playerInput){
            if (playerInput == null) return false;
            if (Tick != playerInput.Tick) return false;
            var count = Commands.Count;
            if (count != playerInput.Commands.Count) return false;

            for (int i = 0; i < count; i++) {
                if (!BaseCmd.IsSame(Commands[i], playerInput.Commands[i])) return false;
            }
            return true;
        }
    }
}