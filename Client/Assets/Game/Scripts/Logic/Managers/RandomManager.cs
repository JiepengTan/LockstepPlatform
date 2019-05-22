using Lockstep.Logging;
using Lockstep.Math;

namespace Lockstep.Game {
    public class RandomManager : SingletonManager<RandomManager>, IRandomService {
        Random _i = new Math.Random();
        public LFloat value => _i.value;

        public uint Next(){
            return _i.Next();
        }

        public uint Next(uint max){
            return _i.Next(max);
        }

        public int Next(int max){
            return _i.Next(max);
        }

        public uint Range(uint min, uint max){
            return _i.Range(min, max);
        }

        public int Range(int min, int max){
            return _i.Range(min, max);
        }

        public LFloat Range(LFloat min, LFloat max){
            return _i.Range(min, max);
        }


        public class RandomCmd : BaseCommand<RandomManager> {
            public ulong randSeed;

            public override void Do(RandomManager param){
                randSeed = param._i.randSeed;
            }

            public override void Undo(RandomManager param){
                param._i.randSeed = randSeed;
            }
        }

        public override void DoStart(){
            cmdBuffer.SetUndoCmdsFunc((from, to, param) => { from.cmd.Undo(param); });
        }

        public override void BackUp(uint tick){
            Debug.Assert(tick == CurTick);
            cmdBuffer.Execute(tick, new RandomCmd());
        }
    }
}