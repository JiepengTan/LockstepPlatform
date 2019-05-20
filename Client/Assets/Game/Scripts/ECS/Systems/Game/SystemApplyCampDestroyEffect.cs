using Entitas;
using Lockstep.Math;

namespace Lockstep.Game.Systems.Game {

    public class SystemApplyCampDestroyEffect : BaseSystem, IExecuteSystem {
        readonly IGroup<GameEntity> _destroyedGroup;

        public SystemApplyCampDestroyEffect(Contexts contexts, IServiceContainer serviceContainer){
            _destroyedGroup = _gameContext.GetGroup(GameMatcher.AllOf(
                GameMatcher.Destroyed,
                GameMatcher.LocalId,
                GameMatcher.Camp));
        }


        public void Execute(){
           
        }
    }
}