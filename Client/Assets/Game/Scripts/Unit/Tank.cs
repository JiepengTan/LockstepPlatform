using System;
using System.Collections.Generic;
using Lockstep.Game;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tank : Unit {
    public Skill skill = new Skill();
    public AIProxy brain = new AIProxy();
    public Tank killer;
    public Vector2 FireOffsetPos {
        get { return DirUtil.GetDirVec(dir); }
    }
    
    public override void DoStart(){
        base.DoStart();
        skill.owner = this;
        brain.owner = this;
    }

    public bool TakeDamage(Bullet bullet){
        if (bullet.health >= health) {
            bullet.health -= health;
            this.health = 0;
            killer = bullet.owner;
            return true;
        }
        health -= bullet.health;
        bullet.health = 0;
        return false;
    }

    public bool Fire(){
        if (skill.CanFire()) {
            skill.Fire();
            return true;
        }

        return false;
    }

    public override void DoUpdate(float deltaTime){
        base.DoUpdate(deltaTime);
        if (brain.enabled) {
            brain.DoUpdate(deltaTime);
        }

        if (skill != null) {
            skill.DoUpdate(deltaTime);
        }
    }

    public Vector2 GetHeadPos(float len = FORWARD_HEAD_DIST){
        var dirVec = DirUtil.GetDirVec(dir);
        var fTargetHead = pos + (TANK_HALF_LEN + len) * (Vector2) dirVec;
        return fTargetHead;
    }

    private void OnDrawGizmos(){
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
        //var debugInfo = CollisionUtil.DebugQueryCollider(dir, GetHeadPos(FORWARD_HEAD_DIST));
        //foreach (var info in debugInfo) {
        //    if (info.z == 1) {
        //        Gizmos.DrawWireCube(info + transform.parent.position + new Vector3(0.5f, 0.5f, 0), Vector3.one * 0.5f);
        //    }
        //    else {
        //        Gizmos.DrawWireSphere(info + transform.parent.position + new Vector3(0.5f, 0.5f, 0), 0.5f);
        //    }
        //}

        //debugInfo.Clear();
#endif
    }
}

[Serializable]
public class AIProxy {
    public Tank owner;
    private float timer;
    public float updateInterval = 1;
    public float fireRate = 0.3f;
    public bool enabled = true;

    
    public void DoUpdate(float deltaTime){
       
    }
}

[Serializable]
public class Skill {
    public Tank owner;
    public float CD = 0.2f;
    private float CDTimer;
    public int prefabType;

    public bool CanFire(){
        return CDTimer <= 0;
    }

    public void Fire(){
        CDTimer = CD;
        //var bullet = GameManager.Instance.CreateBullet(owner.pos, owner.dir, owner.FireOffsetPos, prefabType);
        //bullet.owner = owner;
        //bullet.camp = owner.camp;
    }

    public void DoUpdate(float deltaTime){
        CDTimer -= deltaTime;
        if (CDTimer < 0) {
            CDTimer = 0;
        }
    }
}