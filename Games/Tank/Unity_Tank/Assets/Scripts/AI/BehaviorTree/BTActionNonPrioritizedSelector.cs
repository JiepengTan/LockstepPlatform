using System;
using System.Collections.Generic;

namespace Lockstep.AI
{
    public class BTActionNonPrioritizedSelector : BTActionPrioritizedSelector
    {
        public BTActionNonPrioritizedSelector()
            : base()
        {
        }
        protected override bool OnEvaluate(/*in*/BTWorkingData wData)
        {
            BTActionPrioritizedSelector.BTActionPrioritizedSelectorContext thisContext = 
                GetContext<BTActionPrioritizedSelector.BTActionPrioritizedSelectorContext>(wData);
            //check last node first
            if (IsIndexValid(thisContext.currentSelectedIndex)) {
                BTAction node = GetChild<BTAction>(thisContext.currentSelectedIndex);
                if (node.Evaluate(wData)) {
                    return true;
                }
            }
            return base.OnEvaluate(wData);
        }
    }
}
