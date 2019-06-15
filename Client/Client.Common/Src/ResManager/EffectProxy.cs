using Lockstep.Math;
using NetMsg.Common;

namespace Lockstep.Game {

    public class EffectProxy {
        public IRollbackEffect Effect;
        public EffectProxy pre;
        public EffectProxy next;

        public int createTick;
        public int diedTick;
        public LFloat liveTime;

        public virtual void DoStart(int curTick, IRollbackEffect effect, LFloat liveTime){
            this.liveTime = liveTime;
            createTick = curTick;
            diedTick = curTick + (liveTime * NetworkDefine.FRAME_RATE).ToInt();
            this.Effect = effect;
            if (effect != null) {
                effect.__proxy = this;
                effect.DoStart(curTick);
            }
        }

        public bool IsLive(int curTick){
            return curTick >= createTick && curTick <= diedTick;
        }

        public virtual void DoUpdate(int tick){
            this.Effect?.DoUpdate(tick);
        }
    }
}