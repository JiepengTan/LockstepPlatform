using System;

namespace Lockstep.AI
{
    public class BTActionLoop : BTAction
    {
        public const int INFINITY = -1;
        //--------------------------------------------------------
        protected class BTActionLoopContext : BTActionContext
        {
            internal int currentCount;

            public BTActionLoopContext()
            {
                currentCount = 0;
            }
        }
        //--------------------------------------------------------
        private int _loopCount;
        //--------------------------------------------------------
        public BTActionLoop()
            : base(1)
        {
            _loopCount = INFINITY;
        }
        public BTActionLoop SetLoopCount(int count)
        {
            _loopCount = count;
            return this;
        }
        //-------------------------------------------------------
        protected override bool OnEvaluate(/*in*/BTWorkingData wData)
        {
            BTActionLoopContext thisContext = GetContext<BTActionLoopContext>(wData);
            bool checkLoopCount = (_loopCount == INFINITY || thisContext.currentCount < _loopCount);
            if (checkLoopCount == false) {
                return false;
            }
            if (IsIndexValid(0)) {
                BTAction node = GetChild<BTAction>(0);
                return node.Evaluate(wData);
            }
            return false;
        }
        protected override int OnUpdate(BTWorkingData wData)
        {
            BTActionLoopContext thisContext = GetContext<BTActionLoopContext>(wData);
            int runningStatus = BTRunningStatus.FINISHED;
            if (IsIndexValid(0)) {
                BTAction node = GetChild<BTAction>(0);
                runningStatus = node.Update(wData);
                if (BTRunningStatus.IsFinished(runningStatus)) {
                    thisContext.currentCount++;
                    if (thisContext.currentCount < _loopCount || _loopCount == INFINITY) {
                        runningStatus = BTRunningStatus.EXECUTING;
                    }
                }
            }
            return runningStatus;
        }
        protected override void OnTransition(BTWorkingData wData)
        {
            BTActionLoopContext thisContext = GetContext<BTActionLoopContext>(wData);
            if (IsIndexValid(0)) {
                BTAction node = GetChild<BTAction>(0);
                node.Transition(wData);
            }
            thisContext.currentCount = 0;
        }
    }
}
