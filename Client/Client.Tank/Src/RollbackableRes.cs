using System.Collections;
using System.Collections.Generic;
using Lockstep.Math;
using NetMsg.Common;
using UnityEngine;

namespace Lockstep.Game {
    public class RollbackableRes : UnityEngine.MonoBehaviour {
        [HideInInspector] public ResProxy __proxy;

        [HideInInspector] public int createTick => __proxy.createTick;
        [HideInInspector] public int diedTick => __proxy.diedTick;
        public LFloat liveTime;

        public virtual void DoStart(int curTick){ }
        public virtual void DoUpdate(int tick){ }
    }

    public class ResProxy {
        public RollbackableRes res;
        public ResProxy pre;
        public ResProxy next;

        public int createTick;
        public int diedTick;
        public LFloat liveTime;

        public virtual void DoStart(int curTick, RollbackableRes res, LFloat liveTime){
            this.liveTime = liveTime;
            createTick = curTick;
            diedTick = curTick + (liveTime * NetworkDefine.FRAME_RATE).ToInt();
            this.res = res;
            if (res != null) {
                res.__proxy = this;
                res.DoStart(curTick);
            }
        }

        public bool IsLive(int curTick){
            return curTick >= createTick && curTick <= diedTick;
        }

        public virtual void DoUpdate(int tick){
            this.res?.DoUpdate(tick);
        }
    }
}