using Entitas;

namespace Lockstep.ECS.Systems.GameState {
    public class CalculateHashCode : IInitializeSystem, IExecuteSystem {
        private readonly IGroup<GameEntity> _hashableEntities;

        private readonly GameStateContext _gameStateContext;

        public CalculateHashCode(Contexts contexts){
            _gameStateContext = contexts.gameState;
            _hashableEntities = contexts.game.GetGroup(GameMatcher.AllOf(
                    GameMatcher.LocalId,
                    GameMatcher.Pos
                )
                .NoneOf(GameMatcher.Backup));
        }

        public void Initialize(){
            _gameStateContext.ReplaceHashCode(0);
        }

        public void Execute(){
            long hashCode = 0;
            hashCode ^= _hashableEntities.count;
            foreach (var entity in _hashableEntities) {
                hashCode ^= entity.pos.value._x;
                hashCode ^= entity.pos.value._y;
                if (entity.hasDir) {
                    hashCode ^= (int) entity.dir.value;
                }
            }

            _gameStateContext.ReplaceHashCode(hashCode);
        }
    }
}