using Lockstep.ECS.GameState;
using Lockstep.Logging;
using Lockstep.Math;

namespace Lockstep.Game.Systems.GameState {
    public class SystemInitState : BaseSystem, Entitas.IInitializeSystem {
        public SystemInitState(Contexts contexts, IServiceContainer serviceContainer) :
            base(contexts, serviceContainer){ }

        public void Initialize(){
            //create camps 
            var campPos = _constGameStateService.campPos;
            _unitService.CreateCamp(campPos,0);
            //create actors
            Debug.Assert(_constGameStateService.actorCount <= _constGameStateService.playerBornPoss.Count,"");
            var allActorIds = _constGameStateService.allActorIds;
            for (int i = 0; i < _constGameStateService.actorCount; i++) {
                var entity = _actorContext.CreateEntity();
                entity.AddId(_constGameStateService.allActorIds[i]);
                entity.AddScore(0);
                entity.AddLife(_constGameStateService.playerInitLifeCount);
            }
            //born Player
            for (int i = 0; i < _constGameStateService.actorCount; i++) {
                var actorId = _constGameStateService.allActorIds[i];
                _unitService.CreatePlayer(actorId,0);
            }

            //reset status
            _constGameStateService.MaxEnemyCountInScene = 6;
            _constGameStateService.TotalEnemyCountToBorn = 20;
            _gameStateService.remainCountToBorn = _constGameStateService.TotalEnemyCountToBorn;
            _gameStateService.curEnemyCountInScene = 0;
            _gameStateService.bornTimer = LFloat.zero;
            _gameStateService.bornInterval = new LFloat(3);
            //
        }
    }
}