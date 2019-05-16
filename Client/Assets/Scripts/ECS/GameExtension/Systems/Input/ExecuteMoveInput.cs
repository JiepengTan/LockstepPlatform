using System.Linq;
using Entitas;
using Lockstep.Logging;
using Lockstep.Math;

namespace Lockstep.Game.Features.Input
{
    public class ExecuteMoveInput : IExecuteSystem
    {                                                           
        private readonly GameContext _gameContext;
        readonly IGroup<InputEntity> _navigationInput;
        private readonly GameStateContext _gameStateContext;

        public ExecuteMoveInput(Contexts contexts, ServiceContainer serviceContainer)
        {                                             
            _gameContext = contexts.game;
            _gameStateContext = contexts.gameState;                          

            _navigationInput = contexts.input.GetGroup(InputMatcher.AllOf(
                InputMatcher.MoveDir,
                InputMatcher.ActorId, 
                InputMatcher.Tick));
        }    


        public void Execute()
        {
            foreach (var input in _navigationInput.GetEntities().
                Where(entity => entity.tick.value == _gameStateContext.tick.value))
            {
                var dir = input.moveDir.value;
                var selectedEntities = _gameContext
                    .GetEntities(GameMatcher.LocalId)
                    .Where(entity => entity.actorId.value == input.actorId.value);


                Log.Trace(this, input.actorId.value + " moving " + string.Join(", ", selectedEntities.Select(entity => entity.id.value)));
                var accuCount = Simulation.allAccumInputCount[input.actorId.value, _gameStateContext.tick.value] * 16;
                foreach (var entity in selectedEntities)
                {
                    entity.ReplacePosition(entity.position.value + dir * new LFloat(true,16) );
                    Debug.Log($"tick{_gameStateContext.tick.value}Replace POs id: {entity.actorId.value} pos: {entity.position.value}" );
                    var pos = entity.position.value;
                    int ss = 0;
                }
            }
        }
    }
}
