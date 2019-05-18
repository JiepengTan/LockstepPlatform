using System.Collections.Generic;
using UnityEngine;



public class Bullet : Unit {
    public Tank owner;
    public bool canDestoryIron = false;
    public bool canDestoryGrass = false;
    public virtual void DoUpdate(float deltaTime){
        //update position
        Vector2 dirVec =CollisionHelper. GetDirVec(dir);
        var offset = (moveSpd * deltaTime) * dirVec;
        pos += offset;
        transform.localPosition = pos;
    }
}
