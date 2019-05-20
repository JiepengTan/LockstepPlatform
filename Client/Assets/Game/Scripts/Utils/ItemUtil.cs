using Entitas;
using Lockstep.ECS.Game;

namespace Lockstep.Game {
    public static class ItemUtil {
        //TODO
        public static void TriggerEffect(EItemType type,ActorEntity actor, GameContext gameContext){
        }

        static void OnTriggerBoom(Tank trigger){
           // foreach (var tank in GameManager.Instance.allEnmey) {
           //     tank.health = 0;
           //     tank.killer = trigger;
           // }
        }

        static void OnTriggerUpgrade(Tank trigger){
            //var level = playerTank.detailType + 1;
            //if (level >= playerPrefabs.Count) {
            //    return false;
            //}
//
            //var playerInfo = GetPlayerFormTank(playerTank);
            //// TODO 最好不要这样做 而是以直接修改属性的方式 
            //var tank = DirectCreatePlayer(playerTank.pos - TankBornOffset, level, playerInfo, false);
            //allPlayer.Remove(playerTank);
            //GameObject.Destroy(playerTank.gameObject);
            //return true;
            
        }

        static void OnTriggerAddLife(Tank trigger){
           // var gameMgr = GameManager.Instance;
           // var info = gameMgr.GetPlayerFormTank(trigger);
           // if (info != null) {
           //     info.remainPlayerLife++;
           //     if (gameMgr.OnLifeCountChanged != null) {
           //         gameMgr.OnLifeCountChanged(info);
           //     }
           // }
        }
    }
}