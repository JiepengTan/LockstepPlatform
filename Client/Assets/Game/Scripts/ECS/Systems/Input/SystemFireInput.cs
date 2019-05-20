using System.Collections.Generic;
using System.Linq;
using Lockstep.Math;
using Entitas;
using Lockstep.Math;
using Lockstep.Logging;

namespace Lockstep.Game.Systems.Input {
    public class SystemFireInput :BaseSystem, IExecuteSystem {
        readonly IGroup<InputEntity> _inputGroup;

        public SystemFireInput(Contexts contexts, IServiceContainer serviceContainer):base(contexts,serviceContainer)
        {         
            _inputGroup = contexts.input.GetGroup(InputMatcher.AllOf(
                InputMatcher.Fire,
                InputMatcher.ActorId, 
                InputMatcher.Tick));
        }    

        public void Execute()
        {
            foreach (var input in _inputGroup.GetEntities().
                Where(entity => entity.tick.value == _gameStateContext.tick.value))
            {
                var gameEntity = _gameContext.GetEntityWithLocalId(input.actorId.value);
                gameEntity.isFireRequest = true;
            }
        }
    }
}