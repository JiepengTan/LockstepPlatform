using System.Collections.Generic;
using Entitas;
using Lockstep.ECS;
using Lockstep.ECS.Game;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game.Systems.Game {
    public class SystemCollisionDetected : BaseSystem, IExecuteSystem {
        readonly IGroup<GameEntity> allPlayer;
        readonly IGroup<GameEntity> allEnmey;
        readonly IGroup<GameEntity> allItem;
        readonly IGroup<GameEntity> allCamp;

        public SystemCollisionDetected(Contexts contexts, IServiceContainer serviceContainer):base(contexts,serviceContainer){
            allPlayer = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.ActorId));

            allEnmey = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.TagEnemy));

            allItem = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.ItemType));
            
            allCamp = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.TagCamp));
        }

        public void Execute(){
            // enemy and player
            foreach (var player in allPlayer) {
                if (player.isDestroyed) continue;
                foreach (var enemy in allEnmey) {
                    if (enemy.isDestroyed) continue;
                    if (CollisionUtil.CheckCollision(enemy, player)) {
                        _audioService.PlayClipHitTank();
                        _unitService.TakeDamage(enemy,player);
                    }
                }
            }

            var min = _constStateService.mapMin.ToLVector2();
            var max = _constStateService.mapMax.ToLVector2();
            // tank  and item
            //var players = allPlayer.ToArray(); //item may modified the allPlayer list so copy it
            foreach (var player in allPlayer) {
                if (player.isDestroyed) continue;
                foreach (var item in allItem) {
                    if (item.isDestroyed) continue;
                    if (CollisionUtil.CheckCollision(player, item)) {
                        _audioService.PlayMusicGetItem();
                        item.itemType.killerActorId = player.actorId.value;
                        item.isDestroyed = true;
                    }
                }
            }
        }
    }
}