using System.Collections.Generic;
using Lockstep.Game;
using UnityEngine;

namespace Lockstep.Game {
    public enum EDir {
        Up,
        Left,
        Down,
        Right,
        EnumCount,
    }
}


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
        //update position
        var deg = CollisionHelper.GetDirDeg(dir);
        transform.rotation = Quaternion.Euler(0, 0, deg);
        //can move 判定
        var dirVec = CollisionHelper.GetDirVec(dir);
        var moveDist = (moveSpd * deltaTime);
        var fTargetHead = pos + (TANK_HALF_LEN + moveDist) * (Vector2) dirVec;
        var fPreviewHead = pos + (TANK_HALF_LEN + FORWARD_HEAD_DIST) * (Vector2) dirVec;

        float maxMoveDist = moveSpd * deltaTime;
        var headPos = pos + (TANK_HALF_LEN) * (Vector2) dirVec;
        var dist = CollisionHelper.GetMaxMoveDist(dir, headPos, fTargetHead);
        var dist2 = CollisionHelper.GetMaxMoveDist(dir, headPos, fPreviewHead);
        maxMoveDist = Mathf.Min(maxMoveDist, dist, dist2);

        var diffPos = maxMoveDist * (Vector2) dirVec;
        pos = pos + diffPos;
        if (camp == Global.PlayerCamp) {
            if (isChangedDir) {
                var idir = (int) (dir);
                var isUD = idir % 2 == 0;
                if (isUD) {
                    pos.x = CollisionHelper.RoundIfNear(pos.x, SNAP_DIST);
                }
                else {
                    pos.y = CollisionHelper.RoundIfNear(pos.y, SNAP_DIST);
                }
            }
        }

        transform.localPosition = pos;
        isChangedDir = false;
    }


    public void DoDestroy(){
        GameObject.Destroy(gameObject);
    }

    #region  debug infos

    #endregion
}