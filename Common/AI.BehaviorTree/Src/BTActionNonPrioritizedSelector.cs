using System;
using System.Collections.Generic;

namespace Lockstep.AI
{
    public unsafe partial class BTActionNonPrioritizedSelector : BTActionPrioritizedSelector
    {
        protected override int MemSize => sizeof(BTCActionNonPrioritizedSelector);
        public BTActionNonPrioritizedSelector()
            : base()
        {
        }
        protected override bool OnEvaluate(/*in*/BTWorkingData wData)
        {
            var thisContext = (BTCActionNonPrioritizedSelector*) wData.GetContext(_uniqueKey);
            //check last node first
            if (IsIndexValid(thisContext->currentSelectedIndex)) {
                BTAction node = GetChild<BTAction>(thisContext->currentSelectedIndex);
                if (node.Evaluate(wData)) {
                    return true;
                }
            }
            return base.OnEvaluate(wData);
        }
    }
}
