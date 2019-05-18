namespace Lockstep.Game {
    public interface IEventListener {
        void RegisterListeners(GameEntity entity);
        void UnregisterListeners();
    }
}
