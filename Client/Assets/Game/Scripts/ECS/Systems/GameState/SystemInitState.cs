using Lockstep.ECS.GameState;
using Lockstep.Math;

namespace Lockstep.Game.Systems.GameState {
    public class SystemInitState :BaseSystem, Entitas.IInitializeSystem {
        private readonly Contexts _contexts;

        public SystemInitState(Contexts contexts, IServiceContainer serviceContainer):base(contexts,serviceContainer){
            _contexts = contexts;
        }

        //Load map Create camp and other Entity
        //Create Players
        public void Initialize(){
            var context = _contexts.gameState;
            //reset status
            context.ReplaceGameResult(EGameResult.Playing);
            context.ReplaceEnemyCountState(
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