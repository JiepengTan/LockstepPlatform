#define DEBUG_FRAME_DELAY
using System.Collections.Generic;
using Lockstep.Game;
using Lockstep.Serialization;

namespace NetMsg.Game.Tank{
    public partial class Input : BaseFormater{
        public byte ActorId;
        public uint Tick;
        public List<ICommand> Commands = new List<ICommand>();
#if DEBUG_FRAME_DELAY
        public float timeSinceStartUp;
#endif
        public Input( uint tick,byte actorID, ICommand[] inputs){
            
        }

        public Input(){ }

        /// <summary>
        /// TODO     合并 输入
        /// </summary>
        /// <param name="inputb"></param>
        public void Combine(Input inputb){ }

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
                var cmd = new InputCmd();
                cmd.Deserialize(reader);
                //Commands.Add(cmd);
            }
        }
    }
}