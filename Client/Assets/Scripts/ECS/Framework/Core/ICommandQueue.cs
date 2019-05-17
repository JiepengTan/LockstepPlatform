using System.Collections.Generic;
using System.Linq;
using Lockstep.Game;
using NetMsg.Game.Tank;

namespace Lockstep.Core.Logic.Interfaces
{
    public interface ICommandQueue
    {
        void Enqueue(Msg_PlayerInput msg);

        List<Msg_PlayerInput> Dequeue();
    }
    
    public class CommandQueue : ICommandQueue
    {
        /// <summary>
        /// Mapping: FrameNumber -> Commands per player(Id)
        /// </summary>    
        public Dictionary<uint, List<Msg_PlayerInput>> Buffer { get; } = new Dictionary<uint, List<Msg_PlayerInput>>(5000);


        public void Enqueue(uint tick, byte actorId, List<InputCmd> commands)
        {
            Enqueue(new Msg_PlayerInput(tick, actorId, commands));
        }

        public virtual void Enqueue(Msg_PlayerInput msg)
        {
            lock (Buffer)
            {
                if (!Buffer.ContainsKey(msg.Tick))
                {
                    Buffer.Add(msg.Tick, new List<Msg_PlayerInput>(10)); //Initial size for 10 players
                }       

                Buffer[msg.Tick].Add(msg);
            }
        }

        public virtual List<Msg_PlayerInput> Dequeue()
        {
            lock (Buffer)
            {
                var result = Buffer.SelectMany(pair => pair.Value).ToList();
                Buffer.Clear();
                return result;
            }
        }
    }

}