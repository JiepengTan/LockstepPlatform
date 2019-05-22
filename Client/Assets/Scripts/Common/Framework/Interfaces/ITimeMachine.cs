using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lockstep.Game {
    public interface ITimeMachine {
        uint CurTick { get; set; }
        ///Rollback to tick , so all cmd between [tick,~)(Include tick) should undo
        void RollbackTo(uint tick);
        void Backup(uint tick);
        ///Discard all cmd between [0,maxVerifiedTick] (Include maxVerifiedTick)
        void Clean(uint maxVerifiedTick);
    }
}