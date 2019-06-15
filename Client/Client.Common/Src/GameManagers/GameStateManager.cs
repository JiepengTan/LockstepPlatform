using System;
using System.Collections.Generic;
using Lockstep.Math;
using NaughtyAttributes;
using UnityEngine;

namespace Lockstep.Game {

    public partial class GameStateManager :SingletonManager<GameStateManager>, IGameStateService {
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

        public LFloat _deltaTime = new LFloat(true, 16);

        public LFloat DeltaTime {
            get => _deltaTime;
            set => _deltaTime = value;
        }
    }
}