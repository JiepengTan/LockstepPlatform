using System.Collections.Generic;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {

    public class GameStateManager: SingletonManager<GameStateManager>, IGameStateService {
        //room info
        public byte[] allActorIds { get; set; }
        public int roomId { get; set; }
        public int actorCount { get; set; }
        public int playerInitLifeCount { get; set; }

        //map info
        public Vector2Int mapMin{ get; set; }
        public Vector2Int mapMax{ get; set; }
        public List<Vector2Int> enemyBornPoints{ get; set; }
        public List<Vector2Int> playerBornPoss{ get; set; }

        //game info 
        public int curLevel{ get; set; }

        public override void DoStart(){
            base.DoStart();
            playerInitLifeCount = 3;
        }
    }
}