using Lockstep.Math;

namespace Lockstep.Game.Interfaces
{
    public interface IGridService : IService
    {
        LVector2 GetWorldSize();
        LVector2 GetCellSize();        
    }
}