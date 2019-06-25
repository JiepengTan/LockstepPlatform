using System;
using System.Collections.Generic;

namespace Lockstep.AI
{
    public class BTActionParallel : BTAction
    {
        public enum ECHILDREN_RELATIONSHIP
        {
            AND, OR
        }
        //-------------------------------------------------------
        protected class BTActionParallelContext : BTActionContext
        {
            internal List<bool> evaluationStatus;
            internal List<int> runningStatus;

            public BTActionParallelContext()
            {
                evaluationStatus = new List<bool>();
                runningStatus = new List<int>();
            }
        }
        //-------------------------------------------------------
        private ECHILDREN_RELATIONSHIP _evaluationRelationship;
        private ECHILDREN_RELATIONSHIP _runningStatusRelationship;
        //-------------------------------------------------------
        public BTActionParallel()
            : base(-1)
        {
            _evaluationRelationship = ECHILDREN_RELATIONSHIP.AND;
            _runningStatusRelationship = ECHILDREN_RELATIONSHIP.OR;
        }
        public BTActionParallel SetEvaluationRelationship(ECHILDREN_RELATIONSHIP v)
        {
            _evaluationRelationship = v;
            return this;
        }
        public BTActionParallel SetRunningStatusRelationship(ECHILDREN_RELATIONSHIP v)
        {
            _runningStatusRelationship = v;
            return this;
        }
        //------------------------------------------------------
        protected override bool OnEvaluate(/*in*/BTWorkingData wData)
        {
            BTActionParallelContext thisContext = GetContext<BTActionParallelContext>(wData);
            initListTo<bool>(thisContext.evaluationStatus, false);
            bool finalResult = false;
            for (int i = 0; i < GetChildCount(); ++i) {
                BTAction node = GetChild<BTAction>(i);
                bool ret = node.Evaluate(wData);
                //early break
                if (_evaluationRelationship == ECHILDREN_RELATIONSHIP.AND && ret == false) {
                    finalResult = false;
                    break;
                }
                if (ret == true){
                    finalResult = true;
                }
                thisContext.evaluationStatus[i] = ret;
            }
            return finalResult;
        }
        protected override int OnUpdate(BTWorkingData wData)
        {
            BTActionParallelContext thisContext = GetContext<BTActionParallelContext>(wData);
            //first time initialization
            if (thisContext.runningStatus.Count != GetChildCount()) {
                initListTo<int>(thisContext.runningStatus, BTRunningStatus.EXECUTING);
            }
            bool hasFinished  = false;
            bool hasExecuting = false;
            for (int i = 0; i < GetChildCount(); ++i) {
                if (thisContext.evaluationStatus[i] == false) {
                    continue;
                }
                if (BTRunningStatus.IsFinished(thisContext.runningStatus[i])) {
                    hasFinished = true;
                    continue;
                }
                BTAction node = GetChild<BTAction>(i);
                int runningStatus = node.Update(wData);
                if (BTRunningStatus.IsFinished(runningStatus)) {
                    hasFinished  = true;
                } else {
                    hasExecuting = true;
                }
                thisContext.runningStatus[i] = runningStatus;
            }
            if (_runningStatusRelationship == ECHILDREN_RELATIONSHIP.OR && hasFinished || _runningStatusRelationship == ECHILDREN_RELATIONSHIP.AND && hasExecuting == false) {
                initListTo<int>(thisContext.runningStatus, BTRunningStatus.EXECUTING);
                return BTRunningStatus.FINISHED;
            }
            return BTRunningStatus.EXECUTING;
        }
        protected override void OnTransition(BTWorkingData wData)
        {
            BTActionParallelContext thisContext = GetContext<BTActionParallelContext>(wData);
            for (int i = 0; i < GetChildCount(); ++i) {
                BTAction node = GetChild<BTAction>(i);
                node.Transition(wData);
            }
            //clear running status
            initListTo<int>(thisContext.runningStatus, BTRunningStatus.EXECUTING);
        }
        private void initListTo<T>(List<T> list, T value)
        {
            int childCount = GetChildCount();
            if (list.Count != childCount) {
                list.Clear();
                for (int i = 0; i < childCount; ++i) {
                    list.Add(value);
                }
            } else {
                for (int i = 0; i < childCount; ++i) {
                    list[i] = value;
                }
            }
        }
    }
}
