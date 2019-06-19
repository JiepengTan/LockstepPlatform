using Entitas;
using Lockstep.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Lockstep.Game.UI {
    public class UIGameStatus : UIBaseWindow {
        private Transform TextMsg => GetRef<Transform>("TextMsg");
        private Transform TextLevel => GetRef<Transform>("TextLevel");
        private Transform TextEnemyCount => GetRef<Transform>("TextEnemyCount");
        private Transform TextScore1 => GetRef<Transform>("TextScore1");
        private Transform TextLife1 => GetRef<Transform>("TextLife1");
        private Transform TextScore2 => GetRef<Transform>("TextScore2");
        private Transform TextLife2 => GetRef<Transform>("TextLife2");
        private RawImage RawImg => GetRef<RawImage>("RawImg");

        void ShowPlayerInfo(ActorEntity entity, Transform textScore, Transform textLife){
            if (entity == null) return;
            var score = entity.score.value;
            var life = entity.life.value;
            ShowText(textLife, life.ToString());
            ShowText(textScore, score.ToString());
        }

        void ShowText(Transform parent, string txt){
            if (parent == null) return;
            var text = parent.GetComponent<Text>();
            if (text != null) {
                text.text = txt;
            }
        }

        public  override void DoStart(){
            RawImg.texture = ((UnityUIService) (_uiService)).rt;
#if UNITY_EDITOR
            //OpenWindow(UIDefine.UIDebugInfo);
#endif
        }

        void Update(){
            //UpdateStatus();
        }

        void UpdateStatus(){
            if (!GameConstStateService.Instance.IsPlaying) {
                return;
            }

            var actor = Contexts.sharedInstance.actor;
            var player1 = actor.GetEntityWithId(0);
            var player2 = actor.GetEntityWithId(1);
            ShowPlayerInfo(player1, TextScore1, TextLife1);
            ShowPlayerInfo(player2, TextScore2, TextLife2);
            var gameState = Contexts.sharedInstance.gameState;
            var game = Contexts.sharedInstance.game;
            ShowText(TextEnemyCount, (GameStateService.Instance.remainCountToBorn).ToString());
            ShowText(TextLevel, (ConstStateService.Instance.CurLevel).ToString());
        }
    }
}