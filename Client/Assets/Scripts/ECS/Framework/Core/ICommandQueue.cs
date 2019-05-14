using System.Collections.Generic;
using System.Linq;
using Lockstep.Game;
using NetMsg.Game.Tank;

namespace Lockstep.Core.Logic.Interfaces
{
    public interface ICommandQueue
    {
        void Enqueue(PlayerInput playerInput);

        List<PlayerInput> Dequeue();
    }
    
    public class CommandQueue : ICommandQueue
    {
        /// <summary>
        /// Mapping: FrameNumber -> Commands per player(Id)
        /// </summary>    
        public Dictionary<uint, List<PlayerInput>> Buffer { get; } = new Dictionary<uint, List<PlayerInput>>(5000);


        public void Enqueue(uint tick, byte actorId, List<ICommand> commands)
        {
            Enqueue(new PlayerInput(tick, actorId, commands));
        }

        public virtual void Enqueue(PlayerInput playerInput)
        {
            lock (Buffer)
            {
                if (!Buffer.ContainsKey(playerInput.Tick))
                {
                    Buffer.Add(playerInput.Tick, new List<PlayerInput>(10)); //Initial size for 10 players
                }       

                Buffer[playerInput.Tick].Add(playerInput);
            }
        }

        public virtual List<PlayerInput> Dequeue()
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