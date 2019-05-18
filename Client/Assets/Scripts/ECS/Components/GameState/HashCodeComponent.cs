using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Lockstep.Core.State.GameState
{
    /// <summary>
    /// 执行前的hash
    /// </summary>
    [GameState, Unique]
    public sealed class BeforeExecuteHashCodeComponent : IComponent
    {
        public long value;
    }
    /// <summary>
    /// 执行后的hash
    /// </summary>
    [GameState, Unique]
    public sealed class HashCodeComponent : IComponent
    {
        public long value;
    }
}