using System;
using System.Collections.Generic;
using Lockstep.Math;
using NaughtyAttributes;
using UnityEngine;

namespace Lockstep.Game {
    public partial class GameStateManager : SingletonManager<GameStateManager>, IConstGameStateService {
        //room info
        [SerializeField] private byte[] _allActorIds;

        public byte[] allActorIds {
            get { return _allActorIds; }
            set { _allActorIds = value; }
        }

        [ShowNativeProperty] public int roomId { get; set; }
        [ShowNativeProperty] public int actorCount { get; set; }
        [ShowNativeProperty] public int playerInitLifeCount { get; set; }


        [ShowNativeProperty] public   bool isPurchaseFrame { get;set; }
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

        public LVector2 campPos { get; set; }

        //game info 
        [ShowNativeProperty] public int curLevel { get; set; }


        public int MaxEnemyCountInScene { get; set; }
        public int TotalEnemyCountToBorn { get; set; }


        public override void DoStart(){
            base.DoStart();
            playerInitLifeCount = 3;
        }
    }
        
    public partial class GameStateManager : IGameStateService {
        private GameState curGameState = new GameState();

        public class CopyStateCmd : BaseCommand<GameStateManager> {
            private GameState state;

            public override void Do(GameStateManager param){
                state = param.curGameState;
            }

            public override void Undo(GameStateManager param){
                param.curGameState = state;
            }
        }

        public override void Backup(int tick){
            cmdBuffer.Execute(tick, new CopyStateCmd());
        }

        protected override Action<CommandBuffer<GameStateManager>.CommandNode,
            CommandBuffer<GameStateManager>.CommandNode, GameStateManager> GetRollbackFunc(){
            return (minTickNode, maxTickNode, param) => { minTickNode.cmd.Undo(param); };
        }
        
        public struct GameState {
            public int CurEnemyCountInScene;
            public int RemainCountToBorn;
            public LFloat bornTimer;
            public LFloat bornInterval;
        }
        
        public int curEnemyCountInScene {
            get => curGameState.CurEnemyCountInScene;
            set => curGameState.CurEnemyCountInScene = value;
        }

        public int remainCountToBorn {
            get => curGameState.RemainCountToBorn;
            set => curGameState.RemainCountToBorn = value;
        }

        public LFloat bornTimer {
            get => curGameState.bornTimer;
            set => curGameState.bornTimer = value;
        }

        public LFloat bornInterval {
            get => curGameState.bornInterval;
            set => curGameState.bornInterval = value;
        }
    }
}