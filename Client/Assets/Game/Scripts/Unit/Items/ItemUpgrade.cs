using Lockstep.Game;
using UnityEngine;

public class ItemUpgrade : Item {
    protected override void OnTriggerEffect(Tank trigger){
        GameManager.Instance.Upgrade(trigger, 1);
    }
}