using System;
using System.Collections;
using System.Collections.Generic;
using Lockstep.Game;
using Lockstep.Math;
using NetMsg.Common;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

public class SpriteEffect : RollbackEffect {
    public List<Sprite> sprites = new List<Sprite>();
    private SpriteRenderer render;
    public LFloat interval = new LFloat(1);

    public override void DoStart(int curTick){
        base.DoStart(curTick);
        render = GetComponent<SpriteRenderer>();
    }

    public override void DoUpdate(int tick){
        var timer = (tick - createTick) * LFloat.one / NetworkDefine.FRAME_RATE;
        var idx = (timer * sprites.Count / interval).Floor() % sprites.Count;
        render.sprite = sprites[idx];
    }
}