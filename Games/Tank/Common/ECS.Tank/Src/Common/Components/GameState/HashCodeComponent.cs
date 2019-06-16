using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Lockstep.ECS.GameState
{
    /// <summary>
    /// 执行前的hash
    /// </summary>
    [GameState, Unique]
    public partial  class BeforeExecuteHashCodeComponent : IComponent
    {
        public long value;
    }
    /// <summary>
    /// 执行后的hash
    /// </summary>
    [GameState, Unique]
    public partial class HashCodeComponent : IComponent
    {
        public long value;
    }
}