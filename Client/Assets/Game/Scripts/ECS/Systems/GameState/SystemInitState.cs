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
            
            //create actors
            Debug.Assert(_configService.actorCount <= _configService.playerBornPoss.Count,"");
            var allActorIds = _configService.allActorIds;
            for (int i = 0; i < _configService.actorCount; i++) {
                var entity = _actorContext.CreateEntity();
                entity.AddId(_configService.allActorIds[i]);
                entity.AddName("" + i);
                entity.AddScore(0);
                entity.AddLife(_configService.playerInitLifeCount);
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