using System.Collections.Generic;
using Entitas;
using Lockstep.ECS.Game;
using Lockstep.ECS.GameState;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game.Systems.Game {
    public class SystemApplyExplodeEffect : BaseSystem, IExecuteSystem {
        private HashSet<Vector2Int> explodedPoss = new HashSet<Vector2Int>();
        private HashSet<Vector2Int> explodeEffectPos = new HashSet<Vector2Int>();
        private Queue<GameEntity> pendingExplodeBombs = new Queue<GameEntity>();

        IGroup<GameEntity> _bombGroup;
        private GameEntity[] _allBomb;

        IGroup<GameEntity> _playerGroup;
        private GameEntity[] _allPlayer;

        IGroup<GameEntity> _enemyGroup;
        private GameEntity[] _allEnemy;

        private List<GameEntity> _tempUnitList = new List<GameEntity>();
        private HashSet<GameEntity> _tempBombLst = new HashSet<GameEntity>();

        public SystemApplyExplodeEffect(Contexts contexts, IServiceContainer serviceContainer) : base(contexts,
            serviceContainer){
            _bombGroup = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.Bomb));
            _playerGroup = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.ActorId));

            _enemyGroup = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.TagEnemy));
        }

        public void Execute(){
            explodedPoss.Clear();
            explodeEffectPos.Clear();
            pendingExplodeBombs.Clear();
            _allBomb = _bombGroup.GetEntities();
            _allPlayer = _playerGroup.GetEntities();
            _allEnemy = _enemyGroup.GetEntities();

            //Apply Bomb Effect
            foreach (var entity in _allBomb) {
                entity.bomb.timer -= GameConfig.DeltaTime;
                if (entity.bomb.timer < 0) {
                    ApplyExplodeCross(entity);
                }
            }

            //连炸效果
            while (pendingExplodeBombs.Count > 0) {
                var bomb = pendingExplodeBombs.Dequeue();
                ApplyExplodeCross(bomb);
            }
        }


        public void ApplyExplodeCross(GameEntity entity){
            var dist = entity.bomb.damageRange;
            var rawPos = entity.pos.value.Floor();
            _tempBombLst.Add(entity);
            explodedPoss.Add(rawPos);
            entity.isDestroyed = true;
            //Check 4 dir
            for (int i = 0; i < (int) EDir.EnumCount; i++) {
                var dirVec = DirUtil.GetDirVec((EDir) i);
                ApplyExplodeLine(entity, rawPos, dirVec);
            }

            //check the centerPos
            ApplyExplodePoint(entity, rawPos);
        }

        void ApplyExplodeLine(GameEntity entity, Vector2Int rawPos, Vector2Int dirVec){
            var dist = entity.bomb.damageRange;
            for (int i = 1; i <= dist; i++) {
                var iPos = rawPos + dirVec * i;
                if (ApplyExplodePoint(entity, iPos)) {
                    break;
                }
            }
        }

        private bool ApplyExplodePoint(GameEntity entity, Vector2Int iPos){
            var id = _mapService.Pos2TileId(iPos, true);
            if (id == TilemapUtil.TileID_Iron) {
                //停下来
                return true;
            }

            if (id == TilemapUtil.TileID_Brick) {
                _mapService.ReplaceTile(iPos, id, 0);
            }
            else if (id == 0) {
                var allWalkers = GetWalkerFormPos(iPos);
                foreach (var walker in allWalkers) {
                    var unit = walker.unit;
                    if (!unit.isInvincible && unit.health > 0) {
                        unit.health = 0;
                        walker.isDestroyed = true;
                        unit.killerLocalId = entity.owner.localId;
                    }
                }

                //连炸
                foreach (var oBomb in _allBomb) {
                    if (oBomb.pos.value.Floor() == iPos) {
                        if (explodedPoss.Add(iPos)) {
                            pendingExplodeBombs.Enqueue(oBomb);
                        }
                    }
                }
            }

            if (explodeEffectPos.Add(iPos)) {
                _resourceService.ShowDiedEffect(iPos.ToLVector2() + TankUtil.UNIT_SIZE);
            }

            return false;
        }

        public List<GameEntity> GetWalkerFormPos(Vector2Int iPos){
            _tempUnitList.Clear();
            foreach (var walker in _allEnemy) {
                if (!walker.isDestroyed &&  CollisionUtil.CheckCollision(walker, iPos)) {
                    _tempUnitList.Add(walker);
                }
            }

            foreach (var walker in _allPlayer) {
                if (!walker.isDestroyed && CollisionUtil.CheckCollision(walker, iPos)) {
                    _tempUnitList.Add(walker);
                }
            }

            return _tempUnitList;
        }
    }
}