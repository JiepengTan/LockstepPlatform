using System.Collections.Generic;
using System.Linq;
using Lockstep.Math;
using Entitas;
using Lockstep.Math;
using Lockstep.Logging;
using Lockstep.Game.Interfaces;

namespace Lockstep.Game.Features.Input
{
    public class ExecuteSpawnInput : IExecuteSystem
    {                                              
        private readonly IResourceService _resourceService;
        private readonly GameContext _gameContext;
        private readonly GameStateContext _gameStateContext;   
        private readonly IGroup<InputEntity> _spawnInputs;    

        private uint _localIdCounter;
        private readonly ActorContext _actorContext;
        public ExecuteSpawnInput(Contexts contexts, IServiceContainer serviceContainer)
        {                                                  
            _resourceService = serviceContainer.GetService<IResourceService>();              
            _gameContext = contexts.game;
            _gameStateContext = contexts.gameState;
            _actorContext = contexts.actor;

            _spawnInputs = contexts.input.GetGroup(
                InputMatcher.AllOf(
                    InputMatcher.EntityConfigId,
                    InputMatcher.ActorId,
                    InputMatcher.Fire,
                    InputMatcher.Tick));
        }       

        public void Execute()
        {                                                             
            foreach (var input in _spawnInputs.GetEntities().Where(entity => entity.tick.value == _gameStateContext.tick.value))
            {           
                var actor = _actorContext.GetEntityWithId(input.actorId.value);
                var nextEntityId = actor.entityCount.value;

                var e = _gameContext.CreateEntity();        

                Log.Trace(this, actor.id.value + " -> " + nextEntityId);
                //composite primary key
                e.AddId(nextEntityId);
                e.AddActorId(input.actorId.value);

                //unique id for internal usage
                e.AddLocalId(_localIdCounter);
                
              // //some default components that every game-entity must have
              // e.AddVelocity(LVector2.zero);
              // e.AddPosition(input.coordinate.value);

                _resourceService.LoadView(e, input.entityConfigId.value,_actorContext);

            

                actor.ReplaceEntityCount(nextEntityId + 1);
                _localIdCounter += 1;
            }                                                                                    
        }    
    }
}
