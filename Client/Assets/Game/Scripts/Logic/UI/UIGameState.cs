using Entitas;
using Lockstep.Game;
using UnityEngine;
using UnityEngine.UI;

public class UIGameState : MonoBehaviour {
    public Transform tranLevel;
    public Transform tranEnemy;
    public Transform tranMsg;
    public Transform tranScore1;
    public Transform tranLife1;
    public Transform tranScore2;
    public Transform tranLife2;

    void ShowPlayerInfo(ActorEntity entity, Transform tranScore, Transform tranLife){
        if (entity == null) return;
        var score = entity.score.value;
        var life = entity.life.value;
        ShowText(tranLife, life.ToString());
        ShowText(tranScore, score.ToString());
    }

    void ShowText(Transform parent, string txt){
        if(parent == null) return;
        var text = parent.Find("txtContent")?.GetComponent<Text>();
        if (text != null) {
            text.text = txt;
        }
    }

    void Update(){
        if (!GameManager.Instance.IsPlaying) {
            return;
        }

        var actor = Contexts.sharedInstance.actor;
        var player1 = actor.GetEntityWithId(0);
        var player2 = actor.GetEntityWithId(1);
        ShowPlayerInfo(player1, tranScore1, tranLife1);
        ShowPlayerInfo(player2, tranScore2, tranLife2);
        var gameState = Contexts.sharedInstance.gameState;
        var game = Contexts.sharedInstance.game;
        var gameStateMgr = GameStateManager.Instance;
        ShowText(tranEnemy, (gameStateMgr.remainCountToBorn).ToString());
        ShowText(tranLevel, (gameState.curLevelEntity?.curLevel?.value ?? 0).ToString());
    }
}