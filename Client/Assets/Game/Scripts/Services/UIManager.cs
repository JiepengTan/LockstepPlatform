using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lockstep.Game {
    [System.Serializable]
    public class UIManager : SingletonManager<UIManager> {
        public Text txtCurLevel;
        public Text txtRemainEnemy;
        public Text txtMessage;

        public Text txtRemainLife;
        public Text txtScore;

        public Text txtRemainLife2;
        public Text txtScore2;

        public override void DoStart(){
            base.DoStart();
            //var gMgr = Game.Instance;
            //gMgr.OnLevelChanged += OnLevelChanged;
            //gMgr.OnScoreChanged += OnScoreChanged;
            //gMgr.OnLifeCountChanged += OnLifeChanged;
            //gMgr.OnEnmeyCountChanged += OnEnmeyCountChanged;
            //gMgr.OnMessage += OnMessageStr;
        }

        void OnMessageStr(string msg){
            txtMessage.text = msg;
        }

        void OnLevelChanged(int val){
            txtCurLevel.text = val.ToString();
        }

        void OnEnmeyCountChanged(int val){
            txtRemainEnemy.text = val.ToString();
        }

        void OnScoreChanged(PlayerInfo info){
            Text txt = info.isMainPlayer ? txtScore : txtScore2;
            txt.text = info.score.ToString();
        }

        void OnLifeChanged(PlayerInfo info){
            Text txt = info.isMainPlayer ? txtRemainLife : txtRemainLife2;
            txt.text = info.remainPlayerLife.ToString();
        }
    }
}