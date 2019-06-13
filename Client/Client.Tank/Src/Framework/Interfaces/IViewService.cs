using Entitas;

namespace Lockstep.Game {
    public interface IViewService : IService {
        void BindView(GameEntity entity,  object viewObj);
        void DeleteView(uint entityId);
        void RebindView(GameEntity entity);
        void RebindAllEntities();
    }
}