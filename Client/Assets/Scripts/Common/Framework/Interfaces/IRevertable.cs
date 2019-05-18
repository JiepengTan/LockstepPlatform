using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lockstep.Game {
    public interface IRevertable {
        void RevertTo(uint tick);
        void BackUp(uint tick);
    }
}