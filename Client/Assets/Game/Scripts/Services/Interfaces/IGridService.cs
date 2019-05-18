using Lockstep.Math;

namespace Lockstep.Game
{
    public interface IGridService : IService
    {
        LVector2 GetWorldSize();
        LVector2 GetCellSize();        
    }
}