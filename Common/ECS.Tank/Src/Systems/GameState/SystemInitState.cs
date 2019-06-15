using Lockstep.ECS.GameState;
using Lockstep.Logging;
using Lockstep.Math;

namespace Lockstep.Game.Systems.GameState {
    public class SystemInitState : BaseSystem, Entitas.IInitializeSystem {
        public SystemInitState(Contexts contexts, IServiceContainer serviceContainer) :
            base(contexts, serviceContainer){ }

        public void Initialize(){
            //create camps 
            var campPos = _gameConstStateService.campPos;
            _gameUnitService.CreateCamp(campPos,0);
            //create actors
            Debug.Assert(_gameConstStateService.actorCount <= _gameConstStateService.playerBornPoss.Count,"");
            var allActorIds = _gameConstStateService.allActorIds;
            for (int i = 0; i < _gameConstStateService.actorCount; i++) {
                var entity = _actorContext.CreateEntity();
                entity.AddId(_gameConstStateService.allActorIds[i]);
                entity.AddScore(0);
                entity.AddLife(_gameConstStateService.playerInitLifeCount);
            }
            //born Player
            for (int i = 0; i < _gameConstStateService.actorCount; i++) {
                var actorId = _gameConstStateService.allActorIds[i];
                _gameUnitService.CreatePlayer(actorId,0);
            }

            //reset status
            _gameConstStateService.MaxEnemyCountInScene = 6;
            _gameConstStateService.TotalEnemyCountToBorn = 20;
            _gameStateService.remainCountToBorn = _gameConstStateService.TotalEnemyCountToBorn;
            _gameStateService.curEnemyCountInScene = 0;
            _gameStateService.bornTimer = LFloat.zero;
            _gameStateService.bornInterval = new LFloat(3);
            //
        }
    }
}