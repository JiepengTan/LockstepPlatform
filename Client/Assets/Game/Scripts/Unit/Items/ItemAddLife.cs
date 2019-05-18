using Lockstep.Game;
using UnityEngine;

public class ItemAddLife : Item {
    protected override void OnTriggerEffect(Tank trigger){
        var gameMgr = GameManager.Instance;
        var info =gameMgr. GetPlayerFormTank(trigger);
        if (info != null) {
            info.remainPlayerLife++;
            if (gameMgr.OnLifeCountChanged != null) {
                gameMgr.OnLifeCountChanged(info);
            }
        }
    }
}