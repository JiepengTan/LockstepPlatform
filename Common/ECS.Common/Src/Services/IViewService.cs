using Entitas;

namespace Lockstep.Game {
    public interface IViewService : IService {
        void BindView(IEntity entity,  object viewObj);
        void DeleteView(uint entityId);
        void RebindView(IEntity entity);
        void RebindAllEntities();
    }
}