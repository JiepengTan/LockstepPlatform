using Entitas;
using Lockstep.Math;

namespace Lockstep.Game.Systems.Game {

    public class SystemApplyPlayerDestroyEffect : BaseSystem, IExecuteSystem {
        readonly IGroup<GameEntity> _destroyedGroup;

        public SystemApplyPlayerDestroyEffect(Contexts contexts, IServiceContainer serviceContainer):base(contexts,serviceContainer) {
            _destroyedGroup = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.Destroyed,
                GameMatcher.LocalId,
                GameMatcher.ActorId));
        }


        public void Execute(){
            foreach (var entity in _destroyedGroup.GetEntities()) {
                _resourceService.ShowDiedEffect(entity.move.pos);
                _audioService.PlayClipDied();
                var actor = _actorContext.GetEntityWithId(entity.actorId.value);
                UnityEngine.Debug.Assert(actor != null, " player's tank have no owner");
                if (actor.life.value > 0) {
                    _unitService.CreatePlayer(entity.actorId.value, 0);
                }
            }
        }
    }
}