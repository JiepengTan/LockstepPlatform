using DesperateDevs.Utils;
using Entitas;
using Lockstep.Math;

namespace Lockstep.Game.Systems.Game {
    public class SystemApplyItemDestroyEffect : BaseSystem, IExecuteSystem {
        readonly IGroup<GameEntity> _destroyedGroup;

        public SystemApplyItemDestroyEffect(Contexts contexts, IServiceContainer serviceContainer) {
            _destroyedGroup = _gameContext.GetGroup(GameMatcher.AllOf(
                GameMatcher.Destroyed,
                GameMatcher.LocalId,
                GameMatcher.ItemType));
        }


        public void Execute(){
            foreach (var entity in _destroyedGroup.GetEntities()) {
                _audioService.PlayMusicGetItem();
                var actor = _actorContext.GetEntityWithId(entity.itemType.killerActorId);
                UnityEngine.Debug.Assert(actor != null, " player's tank have no owner");
                if (actor.life.value > 0) {
                    
                    ItemUtil.TriggerEffect(entity.itemType.type, actor, _gameContext);
                }
            }
        }
    }
}