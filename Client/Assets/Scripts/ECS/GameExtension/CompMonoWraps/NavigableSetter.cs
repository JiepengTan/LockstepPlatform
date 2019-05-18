using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lockstep.Game {
    public class NavigableSetter : MonoBehaviour, IComponentSetter {
        public void SetComponent(GameEntity entity){
            entity.isNavigable = true;
        }
    }
}