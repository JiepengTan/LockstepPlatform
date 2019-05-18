using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lockstep.Game {
    public class HashCodeSetter : MonoBehaviour, IComponentSetter {
        public void SetComponent(GameEntity entity){
            //entity.isHashable = true;
        }
    }
}