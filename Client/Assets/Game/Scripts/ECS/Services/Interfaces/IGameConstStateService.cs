using System.Collections.Generic;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    
    public interface IConstGameStateService : IService {
        //room info
        byte[] allActorIds { get; set; }
        int roomId { get; set; }
        int actorCount { get; set; }
        int playerInitLifeCount { get; }


        Vector2Int mapMin { get; set; }
        Vector2Int mapMax { get; set; }

        List<LVector2> enemyBornPoints { get; set; }
        List<LVector2> playerBornPoss { get; set; }
        LVector2 campPos { get; set; }

        int curLevel { get; set; }
        
        
        int MaxEnemyCountInScene{ get; set; }
        int TotalEnemyCountToBorn{ get; set; }
    }
    
    
    public interface IGameStateService : IService {
        
        //changed in the game
        int curEnemyCountInScene{ get; set; }
        int remainCountToBorn{ get; set; }
        LFloat bornTimer{ get; set; }
        LFloat bornInterval{ get; set; }
    }
}