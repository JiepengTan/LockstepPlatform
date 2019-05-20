using Entitas;
using Lockstep.Core.Logic;
using Lockstep.ECS.Game;
using Lockstep.Math;

namespace Lockstep.Game.Systems.Game {
    public class SystemCollisionDetected : IExecuteSystem {
        readonly IGroup<GameEntity> allPlayer;
        readonly IGroup<GameEntity> allBullet;
        readonly IGroup<GameEntity> allEnmey;
        readonly IGroup<GameEntity> allItem;
        private readonly GameContext _gameContext;
        private readonly GameStateContext _gameStateContext;

        public SystemCollisionDetected(Contexts contexts, IServiceContainer serviceContainer){
            _gameContext = contexts.game;
            _gameStateContext = contexts.gameState;
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
                        AudioManager.PlayClipHitTank();
                        UnitUtil.TakeDamage(tank, bullet);
                    }
                }

                foreach (var tank in allEnmey) {
                    if (tank.isDestroyed) continue;
                    if (tank.unit.camp != bulletCamp && CollisionUtil.CheckCollision(bullet, tank)) {
                        AudioManager.PlayClipHitTank();
                        UnitUtil.TakeDamage(tank, bullet);
                    }
                }
            }

            // bullet and camp
            foreach (var bullet in allBullet) {
                if (bullet.isDestroyed) continue;
                if (camp.isDestroyed) continue;
                if (CollisionUtil.CheckCollision(bullet, camp)) {
                    UnitUtil.TakeDamage(camp, bullet);
                    break;
                }
            }
/*
            // bullet and map
            foreach (var bullet in allBullet) {
                var pos = bullet.pos;
                Vector2 borderDir = CollisionUtil.GetBorderDir(bullet.dir);
                var borderPos1 = pos + borderDir * bullet.radius;
                var borderPos2 = pos - borderDir * bullet.radius;
                tempPoss.Add(pos.Floor());
                tempPoss.Add(borderPos1.Floor());
                tempPoss.Add(borderPos2.Floor());
                foreach (var iPos in tempPoss) {
                    CheckBulletWithMap(iPos, tempLst, bullet);
                }

                tempPoss.Clear();
            }

            var min = _gameStateContext.worldBound.min;
            var max = _gameStateContext.worldBound.max;
            // bullet bound detected 
            foreach (var bullet in allBullet) {
                if (CollisionUtil.IsOutOfBound(bullet.move.pos, min, max)) {
                    bullet.unit.health = 0;
                }
            }

            // tank  and item
            var players = allPlayer.ToArray(); //item may modified the allPlayer list so copy it
            foreach (var tank in players) {
                foreach (var item in allItem) {
                    if (CollisionUtil.CheckCollision(tank, item)) {
                        item.TriggelEffect(tank);
                        tempLst.Add(item);
                    }
                }
            }

            foreach (var bullet in allBullet) {
                if (bullet.health <= 0) {
                    tempLst.Add(bullet);
                }
            }

            foreach (var bullet in allEnmey) {
                if (bullet.health <= 0) {
                    tempLst.Add(bullet);
                }
            }

            foreach (var bullet in allPlayer) {
                if (bullet.health <= 0) {
                    tempLst.Add(bullet);
                }
            }

            // destroy unit
            foreach (var unit in tempLst) {
                GameManager.Instance.DestroyUnit(unit as Bullet, GameManager.Instance.allBullet);
                GameManager.Instance.DestroyUnit(unit as Tank, GameManager.Instance.allPlayer);
                GameManager.Instance.DestroyUnit(unit as Tank, GameManager.Instance.allEnmey);
                GameManager.Instance.DestroyUnit(unit as Item, GameManager.Instance.allItem);
                GameManager.Instance.DestroyUnit(unit, ref camp);
            }

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