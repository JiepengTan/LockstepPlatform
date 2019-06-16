using UnityEngine;

namespace Lockstep.Game.UI {
    public interface IReferenceHolder {
        T GetRef<T>(string name) where T : UnityEngine.Object;
    }
}