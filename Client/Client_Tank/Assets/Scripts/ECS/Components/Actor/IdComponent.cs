using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Lockstep.ECS.Actor
{
    /// <summary>
    /// 标志 当前Entity 是当前Tick 中存在  非Backup
    /// </summary>
    [Actor] 
    public partial class IdComponent : IComponent
    {
        [PrimaryEntityIndex]
        public byte value;
    }
}