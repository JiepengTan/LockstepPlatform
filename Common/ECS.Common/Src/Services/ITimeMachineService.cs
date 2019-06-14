namespace Lockstep.Game {
    public interface ITimeMachineService : ITimeMachine, IService {
        void RegisterTimeMachine(ITimeMachine roll);
    }
}