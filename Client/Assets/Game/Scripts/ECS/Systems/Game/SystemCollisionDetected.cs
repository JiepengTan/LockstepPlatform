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
        }


        public void Execute(){
            var camp = _gameContext.campEntity;
            // bullet and tank
            foreach (var bullet in allBullet) {
                if (bullet.isDestroyed) continue;
                var bulletCamp = bullet.unit.camp;
                foreach (var tank in allPlayer) {
                    if (tank.isDestroyed) continue;
                    if (tank.unit.camp != bulletCamp && CollisionUtil.CheckCollision(bullet, tank)) {
                        _audioService.PlayClipHitTank();
                        UnitUtil.TakeDamage(tank, bullet);
                    }
                }

                foreach (var tank in allEnmey) {
                    if (tank.isDestroyed) continue;
                    if (tank.unit.camp != bulletCamp && CollisionUtil.CheckCollision(bullet, tank)) {
                        _audioService.PlayClipHitTank();
                        UnitUtil.TakeDamage(tank, bullet);
                    }
                }
            }

            // bullet and camp
            foreach (var bullet in allBullet) {
                if (bullet.isDestroyed) continue;
                if (camp.isDestroyed) continue;
                if (CollisionUtil.CheckCollision(bullet, camp)) {
                    _audioService.PlayClipHitTank();
                    UnitUtil.TakeDamage(camp, bullet);
                }
            }

            HashSet<Vector2Int> tempPoss = new HashSet<Vector2Int>();
            // bullet and map
            foreach (var bullet in allBullet) {
                var pos = bullet.move.pos;
                var borderDir = DirUtil.GetBorderDir(bullet.move.dir).ToLVector2();
                var borderPos1 = pos + borderDir * bullet.collider.radius;
                var borderPos2 = pos - borderDir * bullet.collider.radius;
                tempPoss.Add(pos.Floor());
                tempPoss.Add(borderPos1.Floor());
                tempPoss.Add(borderPos2.Floor());
                foreach (var iPos in tempPoss) {
                    TilemapUtil.CheckBulletWithMap(iPos, bullet, _audioService,_mapService);
                }

                tempPoss.Clear();
            }

            var min = _gameStateContext.worldBound.min;
            var max = _gameStateContext.worldBound.max;
            // bullet bound detected 
            foreach (var bullet in allBullet) {
                if (CollisionUtil.IsOutOfBound(bullet.move.pos, min, max)) {
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

            // destroy unit
/*
            if (allPlayer.Count == 0) {
                bool hasNoLife = true;
                foreach (var info in allPlayerInfos) {
                    if (info != null && info.remainPlayerLife > 0) {
                        hasNoLife = false;
                        break;
                    }
                }

                if (hasNoLife) {
                    GameFalied();
                }
            }

            if (allEnmey.Count == 0 && RemainEnemyCount <= 0) {
                foreach (var playerInfo in allPlayerInfos) {
                    if (playerInfo != null) {
                        playerInfo.lastLevelTankType = playerInfo.tank == null ? 0 : playerInfo.tank.detailType;
                        playerInfo.isLiveInLastLevel = playerInfo.tank != null;
                    }
                }

                GameWin();
            }

            if (camp == null) {
                GameFalied();
            }

            tempLst.Clear();


            foreach (var entity in _bullets.GetEntities()) {
                var move = entity.move;
                var dirVec = DirUtil.GetDirLVec(entity.dir.value);
                var offset = (move.moveSpd * Define.DeltaTime) * dirVec;
                entity.position.value = offset;
            }
            */
        }
    }
}