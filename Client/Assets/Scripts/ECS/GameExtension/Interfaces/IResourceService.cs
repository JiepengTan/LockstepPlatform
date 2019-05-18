using Entitas;

namespace Lockstep.Game.Interfaces
{
    public interface IResourceService : IService
    {                                                        
        void LoadView(GameEntity entity, int configId, IContext ctx);

        void DeleteView(uint entityId);
    }
}