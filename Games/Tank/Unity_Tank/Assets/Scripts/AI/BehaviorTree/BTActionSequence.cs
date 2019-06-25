using System;
using System.Collections.Generic;

namespace Lockstep.AI
{
    public class BTActionSequence : BTAction
    {
        //-------------------------------------------------------
        protected class BTActionSequenceContext : BTActionContext
        {
            internal int currentSelectedIndex;
            public BTActionSequenceContext()
            {
                currentSelectedIndex = -1;
            }
        }
        //-------------------------------------------------------
        private bool _continueIfErrorOccors;
        //-------------------------------------------------------
        public BTActionSequence()
            : base(-1)
        {
            _continueIfErrorOccors = false;
        }
        public BTActionSequence SetContinueIfErrorOccors(bool v)
        {
            _continueIfErrorOccors = v;
            return this;
        }
        //------------------------------------------------------
        protected override bool OnEvaluate(/*in*/BTWorkingData wData)
        {
            BTActionSequenceContext thisContext = GetContext<BTActionSequenceContext>(wData);
            int checkedNodeIndex = -1;
            if (IsIndexValid(thisContext.currentSelectedIndex)) {
                checkedNodeIndex = thisContext.currentSelectedIndex;
            } else {
                checkedNodeIndex = 0;
            }
            if (IsIndexValid(checkedNodeIndex)) {
                BTAction node = GetChild<BTAction>(checkedNodeIndex);
                if (node.Evaluate(wData)) {
                    thisContext.currentSelectedIndex = checkedNodeIndex;
                    return true;
                }
            }
            return false;
        }
        protected override int OnUpdate(BTWorkingData wData)
        {
            BTActionSequenceContext thisContext = GetContext<BTActionSequenceContext>(wData);
            int runningStatus = BTRunningStatus.FINISHED;
            BTAction node = GetChild<BTAction>(thisContext.currentSelectedIndex);
            runningStatus = node.Update(wData);
            if (_continueIfErrorOccors == false && BTRunningStatus.IsError(runningStatus)) {
                thisContext.currentSelectedIndex = -1;
                return runningStatus;
            }
            if (BTRunningStatus.IsFinished(runningStatus)) {
                thisContext.currentSelectedIndex++;
                if (IsIndexValid(thisContext.currentSelectedIndex)) {
                    runningStatus = BTRunningStatus.EXECUTING;
                } else {
                    thisContext.currentSelectedIndex = -1;
                }
            }
            return runningStatus;
        }
        protected override void OnTransition(BTWorkingData wData)
        {
            BTActionSequenceContext thisContext = GetContext<BTActionSequenceContext>(wData);
            BTAction node = GetChild<BTAction>(thisContext.currentSelectedIndex);
            if (node != null) {
                node.Transition(wData);
            }
            thisContext.currentSelectedIndex = -1;
        }
    }
}
