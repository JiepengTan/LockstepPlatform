using Entitas;
using Lockstep.Math;

namespace Lockstep.Game.Systems.Game {
    public class SystemEnemyBorn : BaseSystem, IExecuteSystem {
        private IGroup<GameEntity> _spawnPoints;

        public SystemEnemyBorn(Contexts contexts, IServiceContainer serviceContainer) :
            base(contexts, serviceContainer){
            _spawnPoints = contexts.game.GetGroup(
                GameMatcher.AllOf(GameMatcher.LocalId, GameMatcher.BornPoint));
        }

        public void Execute(){
            var state = _gameStateContext.enemyCountState;
            if (state.CurEnemyCountInScene < state.MaxEnemyCountInScene && state.RemainCountToBorn > 0) {
                state.bornTimer += Define.DeltaTime;
                var allPoints = _spawnPoints.GetEntities();
                var bornPointCount = allPoints.Length;
                if (state.bornTimer > state.bornInterval && bornPointCount > 0) {
                    state.bornTimer = LFloat.zero;
                    //born enemy
                    var idx = LRandom.Range(0, bornPointCount);
                    var bornPoint = allPoints[idx].bornPoint.coord;
                    _unitService.CreateEnemy(bornPoint);
                }
            }
        }
    }
}