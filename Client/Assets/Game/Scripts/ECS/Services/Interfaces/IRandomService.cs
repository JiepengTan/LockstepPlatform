using System.Collections.Generic;
using Lockstep.Game;
using Lockstep.Logging;
using Lockstep.Math;

namespace Lockstep.Game {
    public interface IRandomService : IService {
        LFloat value { get; }
        uint Next();
        uint Next(uint max);
        int Next(int max);
        uint Range(uint min, uint max);
        int Range(int min, int max);
        LFloat Range(LFloat min, LFloat max);
    }
}

namespace Lockstep.Math {
    public partial class RandomService : IRandomService, IRollbackable {
        public class RandomCmd : BaseCommand<RandomService> {
            public ulong randSeed;

            public override void Do(RandomService param){
                randSeed = param.randSeed;
            }

            public override void Undo(RandomService param){
                param.randSeed = randSeed;
            }
        }

        private CommandBuffer<RandomService> cmdBuffer;

        public void BackUp(uint tick){
            if (cmdBuffer == null) {
                cmdBuffer = new CommandBuffer<RandomService>(this,
                    (from, to, param) => { from.cmd.Undo(param); });
            }

            cmdBuffer.Execute(tick, new RandomCmd());
        }

        public void RevertTo(uint tick){
            cmdBuffer?.RevertTo(tick);
        }

        public void Clean(uint maxVerifiedTick){
            cmdBuffer?.Clean(maxVerifiedTick);
        }
    }
}