using Entitas;
using Lockstep.Math;

namespace Lockstep.Game.Systems.Game {
    public class SystemEnemyBorn : BaseSystem, IExecuteSystem {

        public SystemEnemyBorn(Contexts contexts, IServiceContainer serviceContainer) :
            base(contexts, serviceContainer){
        }

        public void Execute(){
            var state = _gameStateContext.enemyCountState;
            if (state.CurEnemyCountInScene < state.MaxEnemyCountInScene && state.RemainCountToBorn > 0) {
                state.bornTimer -= Define.DeltaTime;
                if (state.bornTimer < 0) {
                    state.bornTimer = state.bornInterval;
                    state.RemainCountToBorn--;
                    state.CurEnemyCountInScene++;
                    //born enemy
                    var allPoints = _globalStateService.enemyBornPoints;
                    var bornPointCount = allPoints.Count;
                    var idx = LRandom.Range(0, bornPointCount);
                    var bornPoint = allPoints[idx];
                    _unitService.CreateEnemy(bornPoint);
                }
            }
        }
    }
}