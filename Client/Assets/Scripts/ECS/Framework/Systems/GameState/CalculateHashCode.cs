using Entitas;                                  

namespace Lockstep.Core.Logic.Systems.GameState
{
    public class CalculateHashCode : IInitializeSystem, IExecuteSystem
    {                                                         
        private readonly IGroup<GameEntity> _hashableEntities;

        private readonly GameStateContext _gameStateContext;

        public CalculateHashCode(Contexts contexts)
        {                                                                                 
            _gameStateContext = contexts.gameState;

            _hashableEntities = contexts.game.GetGroup(GameMatcher.AllOf(GameMatcher.LocalId, GameMatcher.Position).NoneOf(GameMatcher.Backup));
        }

        public void Initialize()
        {
            _gameStateContext.ReplaceHashCode(0);
        }

        public void Execute()
        {
            long hashCode = 0;
            hashCode ^= _hashableEntities.count;
            foreach (var entity in _hashableEntities)
            {
                hashCode ^= entity.position.value._x;
                hashCode ^= entity.position.value._y;
                if (entity.hasVelocity)
                {
                    hashCode ^= entity.velocity.value._x;
                    hashCode ^= entity.velocity.value._y;
                }
                if (entity.hasDestination)
                {
                    hashCode ^= entity.destination.value._x;
                    hashCode ^= entity.destination.value._y;
                }
            }                   

            _gameStateContext.ReplaceHashCode(hashCode);
        }       
    }
}
