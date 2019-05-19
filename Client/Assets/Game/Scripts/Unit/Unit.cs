using System.Collections.Generic;
using Lockstep.Game;
using UnityEngine;




public class Unit : MonoBehaviour {
    /// <summary> 阵营</summary>
    public int camp; //阵营

    public int detailType;

    [Header("Move Infos")] public int health = 1;
    public float moveSpd = 2;
    public float MaxMoveSpd = 2;
    public EDir _dir;

    public EDir dir {
        get { return _dir; }
        set {
            if (_dir != value) {
                isChangedDir = true;
            }

            _dir = value;
        }
    }

    private bool isChangedDir = false;

    [Header("Collision")]
    //for collision
    public Vector2 pos; //center

    public Vector2 size; //for aabb
    public float radius; //for circle

    //const vals
    public const int TANK_HALF_LEN = 1;
    public const float FORWARD_HEAD_DIST = 0.05f;
    public const float SNAP_DIST = 0.4f;

    public virtual bool CanMove(){
        return true;
    }

    public virtual void DoStart(){ }


    public virtual void DoUpdate(float deltaTime){
        
    }


    public void DoDestroy(){
        GameObject.Destroy(gameObject);
    }

    #region  debug infos

    #endregion
}