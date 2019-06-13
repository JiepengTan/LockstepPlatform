using Entitas;
using Lockstep.ECS.GameState;
using Lockstep.Math;

namespace Lockstep.Game.Systems.Game {
    public class SystemApplyCampDestroyEffect : BaseSystem, IExecuteSystem {
        readonly IGroup<GameEntity> _destroyedGroup;

        public SystemApplyCampDestroyEffect(Contexts contexts, IServiceContainer serviceContainer) : base(contexts,
            serviceContainer){
            _destroyedGroup = contexts.game.GetGroup(GameMatcher.AllOf(
                GameMatcher.Destroyed,
                GameMatcher.LocalId,
                GameMatcher.TagCamp));
        }


        public void Execute(){
            
        }
    }
}