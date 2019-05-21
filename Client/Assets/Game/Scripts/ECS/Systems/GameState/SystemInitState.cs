using Lockstep.ECS.GameState;
using Lockstep.Logging;
using Lockstep.Math;

namespace Lockstep.Game.Systems.GameState {
    public class SystemInitState : BaseSystem, Entitas.IInitializeSystem {
        public SystemInitState(Contexts contexts, IServiceContainer serviceContainer) :
            base(contexts, serviceContainer){ }

        public void Initialize(){
            //create camps 
            var campPos = _globalStateService.campPos;
            _unitService.CreateCamp(campPos,0);
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
                var actorId = _globalStateService.allActorIds[i];
                _unitService.CreatePlayer(actorId,0);
            }

            //reset status
            _gameStateContext.ReplaceGameResult(EGameResult.Playing);
            _gameStateContext.ReplaceEnemyCountState(
                6,
                20,
                0,
                20,
                LFloat.zero,
                new LFloat(3));

            //
        }
    }
}