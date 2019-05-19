using Lockstep.Math;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using Lockstep.Game;

namespace Lockstep.ECS.Game
{
    [Game]
    [Unique]
    [System.Serializable]
    public class TilemapComponent : IComponent {
        public byte[,] tileIds;
    }
}