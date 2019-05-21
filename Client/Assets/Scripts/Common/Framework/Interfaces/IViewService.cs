using Entitas;

namespace Lockstep.Game
{
    public interface IViewService : IService
    {                                                        
        void BindView(GameEntity entity, IContext ctx,object viewObj);

        void DeleteView(uint entityId);
    }
}