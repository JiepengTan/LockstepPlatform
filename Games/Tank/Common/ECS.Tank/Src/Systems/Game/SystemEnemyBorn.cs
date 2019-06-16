using Entitas;
using Lockstep.Math;

namespace Lockstep.Game.Systems.Game {
    public class SystemEnemyBorn : BaseSystem, IExecuteSystem {

        public SystemEnemyBorn(Contexts contexts, IServiceContainer serviceContainer) :
            base(contexts, serviceContainer){
        }

        public void Execute(){
            if (_gameStateService.curEnemyCountInScene < _gameConstStateService.MaxEnemyCountInScene && _gameStateService.remainCountToBorn > 0) {
                _gameStateService.bornTimer -= _gameStateService.DeltaTime;
                if (_gameStateService.bornTimer < 0) {
                    _gameStateService.bornTimer = _gameStateService.bornInterval;
                    _gameStateService.remainCountToBorn--;
                    _gameStateService.curEnemyCountInScene++;
                    //born enemy
                    var allPoints = _gameConstStateService.enemyBornPoints;
                    var bornPointCount = allPoints.Count;
                    var idx = _randomService.Range(0, bornPointCount);
                    var bornPoint = allPoints[idx];
                    _gameUnitService.CreateEnemy(bornPoint);
                }
            }
        }
    }
}