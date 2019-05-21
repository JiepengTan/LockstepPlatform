using System.Collections.Generic;
using Lockstep.Math;
using NaughtyAttributes;
using UnityEngine;

namespace Lockstep.Game {
    public class GlobalStateManager : SingletonManager<GlobalStateManager>, IGlobalStateService {
        //room info
        [SerializeField] private byte[] _allActorIds;

        public byte[] allActorIds {
            get { return _allActorIds; }
            set { _allActorIds = value; }
        }

        [ShowNativeProperty] public int roomId { get; set; }
        [ShowNativeProperty] public int actorCount { get; set; }
        [ShowNativeProperty] public int playerInitLifeCount { get; set; }


        //map info
        [ShowNativeProperty] public Vector2Int mapMin { get; set; }
        [ShowNativeProperty] public Vector2Int mapMax { get; set; }
        [SerializeField] private List<LVector2> _enemyBornPoints;

        public List<LVector2> enemyBornPoints {
            get { return _enemyBornPoints; }
            set { _enemyBornPoints = value; }
        }

        [SerializeField] private List<LVector2> _playerBornPoss;

        public List<LVector2> playerBornPoss {
            get { return _playerBornPoss; }
            set { _playerBornPoss = value; }
        }
        
        public  LVector2 campPos { get; set; }

        //game info 
        [ShowNativeProperty] public int curLevel { get; set; }

        public override void DoStart(){
            base.DoStart();
            playerInitLifeCount = 3;
        }
    }
}