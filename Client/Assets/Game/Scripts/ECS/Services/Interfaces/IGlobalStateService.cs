using System.Collections.Generic;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {

    public interface IGlobalStateService : IService {
        //room info
        byte[] allActorIds { get; set; }
        int roomId { get; set; }
        int actorCount { get; set; }
        int playerInitLifeCount { get; }


        Vector2Int mapMin { get; set; }
        Vector2Int mapMax { get; set; }

        List<LVector2> enemyBornPoints { get; set; }
        List<LVector2> playerBornPoss { get; set; }

        int curLevel { get; set; }
    }
}