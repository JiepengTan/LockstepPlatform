using System.Collections.Generic;
using Entitas;
using Lockstep.Math;
using NaughtyAttributes;
using UnityEngine;

namespace Lockstep.Game {
    public partial class GameConstStateService : UnityBaseGameService, IGameConstStateService {
        public static GameConstStateService Instance { get; private set; }

        public GameConstStateService(){
            Instance = this;
        }

        public bool IsPlaying { get; set; }

        public int MaxPlayerCount => 2;

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
        [ShowNativeProperty] public LVector2Int mapMin { get; set; }
        [ShowNativeProperty] public LVector2Int mapMax { get; set; }
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

        public LVector2 campPos { get; set; }


        public int MaxEnemyCountInScene { get; set; }
        public int TotalEnemyCountToBorn { get; set; }

        public override void DoStart(){
            base.DoStart();
            playerInitLifeCount = 3;
            IsGameOver = false;
        }


        public bool IsGameOver;

        public void OnEvent_LevelLoadDone(object param){
            var level = (int) param;
            IsGameOver = false;
            _constStateService.CurLevel = level;
        }

        public void OnEvent_SimulationStart(object param){
            IsPlaying = true;
        }

        private void GameFalied(){
            IsGameOver = true;
            ShowMessage("Game Over!!");
        }

        private void GameWin(){
            IsGameOver = true;
            //f (CurLevel >= MAX_LEVEL_COUNT) {
            //   ShowMessage("You Win!!");
            //
            //lse {
            //   //Map2DService.Instance.LoadLevel(CurLevel + 1);
            //
        }

        void ShowMessage(string msg){ }
    }
}