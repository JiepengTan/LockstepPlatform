using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigableSetter : MonoBehaviour, IComponentSetter
{      
    public void SetComponent(GameEntity entity)
    {
        entity.isNavigable = true;
    }
}
