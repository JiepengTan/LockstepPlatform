using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lockstep.Game {
    public interface IRollbackable {
        ///RevertTo tick , so all cmd between [tick,~)(Include tick) should undo
        void RevertTo(uint tick);
        void BackUp(uint tick);
        ///Discard all cmd between [0,maxVerifiedTick] (Include maxVerifiedTick)
        void Clean(uint maxVerifiedTick);
    }
}