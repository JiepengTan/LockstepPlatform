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
            Debug.Assert(_globalStateService.actorCount <= _globalStateService.playerBornPoss.Count,"");
            var allActorIds = _globalStateService.allActorIds;
            for (int i = 0; i < _globalStateService.actorCount; i++) {
                var entity = _actorContext.CreateEntity();
                entity.AddId(_globalStateService.allActorIds[i]);
                entity.AddName("" + i);
                entity.AddScore(0);
                entity.AddLife(_globalStateService.playerInitLifeCount);
            }
            //born Player
            for (int i = 0; i < _globalStateService.actorCount; i++) {
                var id = _globalStateService.allActorIds[i];
                
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