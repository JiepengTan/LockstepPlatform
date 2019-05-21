using Entitas;
using Entitas.CodeGeneration.Attributes;
using Lockstep.Math;

namespace Lockstep.ECS.GameState
{        
    [GameState]
    [Unique]
    public class EnemyCountStateComponent : IComponent {
        //const in the game
        public int MaxEnemyCountInScene;
        public int TotalEnemyCountToBorn;
        
        //changed in the game
        public int CurEnemyCountInScene;
        public int RemainCountToBorn;
        public LFloat bornTimer;
        public LFloat bornInterval;
    }
    
}