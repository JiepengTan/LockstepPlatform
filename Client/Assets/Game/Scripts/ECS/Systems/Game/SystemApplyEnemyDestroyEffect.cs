using Entitas;
using Lockstep.Math;

namespace Lockstep.Game.Systems.Game {
    public class SystemApplyEnemyDestroyEffect : BaseSystem, IExecuteSystem {
        readonly IGroup<GameEntity> _destroyedGroup;

        public SystemApplyEnemyDestroyEffect(Contexts contexts, IServiceContainer serviceContainer) : base(contexts,
            serviceContainer){
            _destroyedGroup = _gameContext.GetGroup(GameMatcher.AllOf(
                GameMatcher.Destroyed,
                GameMatcher.LocalId,
                GameMatcher.TagEnemy));
        }


        public void Execute(){
            foreach (var entity in _destroyedGroup.GetEntities()) {
                var tank = entity.unit;
                _resourceService.ShowDiedEffect(entity.move.pos);
                _audioService.PlayClipDied();
                var info = _actorContext.GetEntityWithId(tank.killerActorId);
                info.score.value += (tank.detailType + 1) * 100;
                if (entity.hasDropRate) {
                    UnitUtil.DropItem(entity.dropRate.value);
                }
            }
        }
    }
}