using Entitas;
using Lockstep.Math;

namespace Lockstep.Game {
    public interface IViewService : IService {
        void BindView(IEntity entity, short assetId, LVector2 createPos,int deg = 0);
        void DeleteView(uint entityId);
        void RebindView(IEntity entity);
        void RebindAllEntities();
    }
}