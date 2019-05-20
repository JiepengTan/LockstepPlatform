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
                var gameEntity = _gameContext.GetEntityWithLocalId(input.actorId.value);
                gameEntity.AddMoveRequest(input.moveDir.value);
            }
        }
    }
}