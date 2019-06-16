using System.Linq;
using Entitas;
using Lockstep.Logging;
using Lockstep.Math;

namespace Lockstep.Game.Systems.Input {
    public class SystemMoveInput : BaseSystem, IExecuteSystem {
        readonly IGroup<InputEntity> _inputGroup;

        public SystemMoveInput(Contexts contexts, IServiceContainer serviceContainer) :
            base(contexts, serviceContainer){
            _inputGroup = contexts.input.GetGroup(InputMatcher.AllOf(
                InputMatcher.MoveDir,
                InputMatcher.ActorId,
                InputMatcher.Tick));
        }

        public void Execute(){
            foreach (var input in _inputGroup.GetEntities()
                .Where(entity => entity.tick.value == _gameStateContext.tick.value)) {
                var actorEntity = _actorContext.GetEntityWithId(input.actorId.value);
                if (!actorEntity.hasGameLocalId) continue;
                var gameLocalId = actorEntity.gameLocalId.value;
                var gameEntity =  _gameContext.GetEntityWithLocalId(gameLocalId);
                if (gameEntity != null) {
                    gameEntity.ReplaceMoveRequest(input.moveDir.value);
                    if (gameEntity.dir.value != input.moveDir.value) {
                        gameEntity.move.isChangedDir = true;
                    }
                    gameEntity.dir.value = input.moveDir.value;
                }

            }
        }
    }
}