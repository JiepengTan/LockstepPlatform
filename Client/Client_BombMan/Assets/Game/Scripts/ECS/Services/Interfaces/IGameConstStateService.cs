using System.Collections.Generic;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    public interface IConstGameStateService : IService {
        bool IsVideoMode { get; set; }

        bool IsRunVideo { get; set; }

        //本地玩家ID
        byte localActorId { get; set; }
        //room info
        byte[] allActorIds { get; set; }
        int roomId { get; set; }
        int actorCount { get; set; }
        int playerInitLifeCount { get; }


        /// 是否正在追帧
        bool isPursueFrame { get; set; }

        Vector2Int mapMin { get; set; }
        Vector2Int mapMax { get; set; }

        List<LVector2> enemyBornPoints { get; set; }
        List<LVector2> playerBornPoss { get; set; }
        LVector2 campPos { get; set; }

        int curLevel { get; set; }

    }


    public interface IGameStateService : IService {
    }
}