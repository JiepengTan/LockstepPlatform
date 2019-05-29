using Lockstep.ECS.GameState;
using Lockstep.Logging;
using Lockstep.Math;

namespace Lockstep.Game.Systems.GameState {
    public class SystemInitState : BaseSystem, Entitas.IInitializeSystem {
        public SystemInitState(Contexts contexts, IServiceContainer serviceContainer) :
            base(contexts, serviceContainer){ }

        public void Initialize(){
            //create camps 
            var campPos = _constStateService.campPos;
            _unitService.CreateCamp(campPos,0);
            //create actors
            Debug.Assert(_constStateService.actorCount <= _constStateService.playerBornPoss.Count,"");
            var allActorIds = _constStateService.allActorIds;
            for (int i = 0; i < _constStateService.actorCount; i++) {
                var entity = _actorContext.CreateEntity();
                entity.AddId(_constStateService.allActorIds[i]);
                entity.AddScore(0);
                entity.AddLife(_constStateService.playerInitLifeCount);
            }
            //born Player
            for (int i = 0; i < _constStateService.actorCount; i++) {
                var actorId = _constStateService.allActorIds[i];
                _unitService.CreatePlayer(actorId,0);
            }
            //born enemy
            var enemyCount = _constStateService.enemyBornPoints.Count;
            for (int i = 0; i <enemyCount ; i++) {
                _unitService.CreateEnemy(_constStateService.enemyBornPoints[i]);
            }
        }
    }
}