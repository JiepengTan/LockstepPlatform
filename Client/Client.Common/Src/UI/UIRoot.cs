
using UnityEngine;

namespace Lockstep.Game.UI {

    public class UIRoot : UIBaseWindow {
        public Transform TransNormal=> GetRef<Transform>("TransNormal");
        public Transform TransNotice=> GetRef<Transform>("TransNotice");
        public Transform TransForward=> GetRef<Transform>("TransForward");
        public Transform TransMask=> GetRef<Transform>("TransMask");
    }
}