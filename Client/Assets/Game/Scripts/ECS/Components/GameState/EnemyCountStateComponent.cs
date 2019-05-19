using Entitas;
using Entitas.CodeGeneration.Attributes;
using Lockstep.Math;

namespace Lockstep.ECS.GameState
{        
    [GameState]
    [Unique]
    public class EnemyCountStateComponent : IComponent {
        public int CurEnemyCountInScene;
        public int MaxEnemyCountInScene;
        public int RemainCountToBorn;
        public int TotalEnemyCountToBorn;
        public LFloat bornTimer;
        public LFloat bornInterval;
    }
    
}