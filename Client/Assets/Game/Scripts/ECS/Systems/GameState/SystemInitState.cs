using Lockstep.ECS.GameState;
using Lockstep.Math;

namespace Lockstep.Game.Systems.GameState {
    public class SystemInitState : BaseSystem, Entitas.IInitializeSystem {
        public SystemInitState(Contexts contexts, IServiceContainer serviceContainer) :
            base(contexts, serviceContainer){ }

        //Load map Create camp and other Entity
        //Create Players
        public void Initialize(){
            //reset status
            _gameStateContext.ReplaceGameResult(EGameResult.Playing);
            _gameStateContext.ReplaceEnemyCountState(
                0,
                6,
                20,
                20,
                LFloat.zero,
                new LFloat(3));

            //
        }
    }
}