using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Lockstep.ECS.Game
{
    /// <summary>
    /// 标志 当前Entity 是当前Tick 中存在 非Backup
    /// </summary>
    [Game] 
    public partial class LocalIdComponent : IComponent
    {    
        [PrimaryEntityIndex]
        public uint value;
    }
}