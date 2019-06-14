namespace Lockstep.Game {

    public partial class BaseSystemReferenceHolder : ServiceReferenceHolder {
        protected InputContext _inputContext;
        protected ActorContext _actorContext;
        protected GameContext _gameContext;
        protected GameStateContext _gameStateContext;


        public virtual void InitReference(Contexts contexts){
            _actorContext = contexts.actor;
            _inputContext = contexts.input;
            _gameContext = contexts.game;
            _gameStateContext = contexts.gameState;
        }
    }
}