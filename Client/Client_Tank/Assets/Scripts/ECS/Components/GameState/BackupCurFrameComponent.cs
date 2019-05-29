using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Lockstep.ECS.GameState
{
    [GameState, Unique]
    public partial class BackupCurFrameComponent : IComponent
    {
    }
}
