using Entitas;

namespace Lockstep.Game {
    public interface IViewService : IService {
        void BindView(IEntity entity,  int assetId);
        void DeleteView(uint entityId);
        void RebindView(IEntity entity);
        void RebindAllEntities();
    }
}