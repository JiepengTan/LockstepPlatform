using System.Collections.Generic;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    public class IConfigService : SingletonManager<IConfigService>, IService {
        //room info
        [Header("room infos")] public byte[] allActorIds;
        public int roomId;
        public int actorCount;
        public int playerInitLifeCount = 3;

        [Header("map infos")]
        //map info
        public Vector2Int mapMin;
        public Vector2Int mapMax;
        
        public List<Vector2Int> enemyBornPoints = new List<Vector2Int>();
        public List<Vector2Int> playerBornPoss = new List<Vector2Int>();

        //game info 
        public int curLevel;

    }
}