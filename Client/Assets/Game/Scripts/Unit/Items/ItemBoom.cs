using Lockstep.Game;
using UnityEngine;

public class ItemBoom : Item {
    protected override void OnTriggerEffect(Tank trigger){
        foreach (var tank in GameManager.Instance.allEnmey) {
            tank.health = 0;
            tank.killer = trigger;
        }
    }
}