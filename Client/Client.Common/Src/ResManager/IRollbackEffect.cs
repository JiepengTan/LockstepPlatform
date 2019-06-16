using Lockstep.Math;

namespace Lockstep.Game {
    public interface IRollbackEffect {
        EffectProxy __proxy { get;set; }
        void DoStart(int curTick);
        void DoUpdate(int tick);
        LFloat LiveTime { get; set; }
    }
}