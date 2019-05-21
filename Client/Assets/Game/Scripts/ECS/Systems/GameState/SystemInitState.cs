using Lockstep.ECS.GameState;
using Lockstep.Logging;
using Lockstep.Math;

namespace Lockstep.Game.Systems.GameState {
    public class SystemInitState : BaseSystem, Entitas.IInitializeSystem {
        public SystemInitState(Contexts contexts, IServiceContainer serviceContainer) :
            base(contexts, serviceContainer){ }

        //Load map Create camp and other Entity
        //Create Players
        public void Initialize(){
            //create camps 
            var entitys = _actorContext.GetEntities();
            //create actors
            Debug.Assert(_gameStateService.actorCount <= _gameStateService.playerBornPoss.Count,"");
            var allActorIds = _gameStateService.allActorIds;
            for (int i = 0; i < _gameStateService.actorCount; i++) {
                var entity = _actorContext.CreateEntity();
                entity.AddId(_gameStateService.allActorIds[i]);
                entity.AddName("" + i);
                entity.AddScore(0);
                entity.AddLife(_gameStateService.playerInitLifeCount);
            }
            //reset status
            _gameStateContext.ReplaceGameResult(EGameResult.Playing);
            _gameStateContext.ReplaceEnemyCountState(
                0,
                6,
                20,
                20,
                LFloat.zero,
                new LFloat(3));

            //
        }
    }
}