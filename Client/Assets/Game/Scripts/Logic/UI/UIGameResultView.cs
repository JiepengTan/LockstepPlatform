using Lockstep.Game;
using UnityEngine;
using UnityEngine.UI;
using Debug = Lockstep.Logging.Debug;

public class UIGameResultView : MonoBehaviour, IGameResultListener,IGameResultRemovedListener
{
    [SerializeField] private Animator _animator;
    [SerializeField] private string _boolName;

    private int _boolHash;

    private void Start()
    {
        Contexts.sharedInstance.gameState.CreateEntity().AddGameResultListener(this);
        Contexts.sharedInstance.gameState.CreateEntity().AddGameResultRemovedListener(this);
        _boolHash = Animator.StringToHash(_boolName);

        SetGameOver(Contexts.sharedInstance.gameState.gameResultEntity, Contexts.sharedInstance.gameState.hasGameResult);
    }

    public void OnGameResult(GameStateEntity entity, Lockstep.Game.EGameResult value)
    {
        if (value != EGameResult.Playing) { 
            UnityEngine.Debug.LogError("SetGameOver " + value);
        }
    }
    public void OnGameResultRemoved(GameStateEntity entity)
    {
        SetGameOver(entity, false);
    }

    private void SetGameOver(GameStateEntity entity, bool value)
    {
        //_animator.SetBool(_boolHash, value);
        UnityEngine.Debug.Log("SetGameOver " + value);
    }
}