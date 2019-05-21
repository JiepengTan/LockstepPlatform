using System.Collections.Generic;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {

    public interface IGameStateService : IService {
        //room info
        byte[] allActorIds { get; set; }
        int roomId { get; set; }
        int actorCount { get; set; }
        int playerInitLifeCount { get; }


        Vector2Int mapMin { get; set; }
        Vector2Int mapMax { get; set; }

        List<Vector2Int> enemyBornPoints { get; set; }
        List<Vector2Int> playerBornPoss { get; set; }

        int curLevel { get; set; }
    }
}