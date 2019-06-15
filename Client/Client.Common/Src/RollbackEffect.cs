using System.Collections;
using System.Collections.Generic;
using Lockstep.Math;
using NetMsg.Common;
using UnityEngine;

namespace Lockstep.Game {
    public class RollbackEffect : UnityEngine.MonoBehaviour,IRollbackEffect {
        [HideInInspector] public EffectProxy __proxy { get;set; }

        [HideInInspector] public int createTick => __proxy.createTick;
        [HideInInspector] public int diedTick => __proxy.diedTick;
        public LFloat liveTime;

        public virtual void DoStart(int curTick){ }
        public virtual void DoUpdate(int tick){ }
        
    }
}