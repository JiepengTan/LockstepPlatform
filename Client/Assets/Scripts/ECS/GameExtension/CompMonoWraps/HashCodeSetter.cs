using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HashCodeSetter : MonoBehaviour, IComponentSetter
{
    public void SetComponent(GameEntity entity)
    {
        entity.isHashable = true;
    }
}
