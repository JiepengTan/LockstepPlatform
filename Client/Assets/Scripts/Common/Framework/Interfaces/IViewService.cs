using Entitas;

namespace Lockstep.Game
{
    public interface IViewService : IService
    {                                                        
        void LoadView(GameEntity entity, int configId, IContext ctx);

        void DeleteView(uint entityId);
    }
}