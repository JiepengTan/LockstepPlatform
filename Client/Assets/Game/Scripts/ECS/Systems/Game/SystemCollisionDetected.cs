using System.Collections.Generic;
using Entitas;
using Lockstep.Core.Logic;
using Lockstep.ECS.Game;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game.Systems.Game {
    public class SystemCollisionDetected : BaseSystem, IExecuteSystem {
        readonly IGroup<GameEntity> allPlayer;
        readonly IGroup<GameEntity> allBullet;
        readonly IGroup<GameEntity> allEnmey;
        readonly IGroup<GameEntity> allItem;
        readonly IGroup<GameEntity> allCamp;

        public SystemCollisionDetected(Contexts contexts, IServiceContainer serviceContainer):base(contexts,serviceContainer){
            allPlayer = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.ActorId));
            allBullet = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.TagBullet));

            allEnmey = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.TagEnemy));

            allItem = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.ItemType));
            
            allCamp = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.ItemType));
        }


        public void Execute(){
            // bullet and tank
            foreach (var bullet in allBullet) {
                if (bullet.isDestroyed) continue;
                var bulletCamp = bullet.unit.camp;
                 foreach (var tank in allPlayer) {
                    if (tank.isDestroyed) continue;
                    if (tank.unit.camp != bulletCamp && CollisionUtil.CheckCollision(bullet, tank)) {
                        _audioService.PlayClipHitTank();
                        _unitService.TakeDamage(bullet,tank );
                    }
                }

                foreach (var tank in allEnmey) {
                    if (tank.isDestroyed) continue;
                    if (tank.unit.camp != bulletCamp && CollisionUtil.CheckCollision(bullet, tank)) {
                        _audioService.PlayClipHitTank();
                        _unitService.TakeDamage(bullet,tank );
                    }
                }
            }

            // bullet and camp
            foreach (var camp in allCamp) {
                foreach (var bullet in allBullet) {
                    if (bullet.isDestroyed) continue;
                    if (camp.isDestroyed) continue;
                    if (CollisionUtil.CheckCollision(bullet, camp)) {
                        _audioService.PlayClipHitTank();
                        _unitService.TakeDamage(bullet,camp);
                    }
                }
            }

            HashSet<Vector2Int> tempPoss = new HashSet<Vector2Int>();
            // bullet and map
            foreach (var bullet in allBullet) {
                if (bullet.isDestroyed) continue;
                var pos = bullet.pos.value;
                var borderDir = DirUtil.GetBorderDir(bullet.dir.value).ToLVector2();
                var borderPos1 = pos + borderDir * bullet.collider.radius;
                var borderPos2 = pos - borderDir * bullet.collider.radius;
                tempPoss.Add(pos.Floor());
                tempPoss.Add(borderPos1.Floor());
                tempPoss.Add(borderPos2.Floor());
                foreach (var iPos in tempPoss) {
                    TilemapUtil.CheckBulletWithMap(iPos, bullet, _audioService,_mapService);
                }

                if (bullet.unit.health == 0) {
                    bullet.isDestroyed = true;
                }

                tempPoss.Clear();
            }

            var min = _globalStateService.mapMin.ToLVector2();
            var max = _globalStateService.mapMax.ToLVector2();
            // bullet bound detected 
            foreach (var bullet in allBullet) {
                if (CollisionUtil.IsOutOfBound(bullet.pos.value, min, max)) {
                    bullet.isDestroyed = true;
                }
            }

            // tank  and item
            //var players = allPlayer.ToArray(); //item may modified the allPlayer list so copy it
            foreach (var player in allPlayer) {
                if (player.isDestroyed) continue;
                foreach (var item in allItem) {
                    if (item.isDestroyed) continue;
                    if (CollisionUtil.CheckCollision(player, item)) {
                        item.itemType.killerActorId = player.actorId.value;
                        item.isDestroyed = true;
                    }
                }
            }
        }
    }
}