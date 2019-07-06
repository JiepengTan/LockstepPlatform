//The phase that the game can be at

using Lockstep.ECS.ECDefine;

public enum GamePhase {
    Gameplay,
    GameOver
}

//A struct to be declared on the global variables, so there is a global GamePhase 
public class GameState : IComponent {
    GamePhase Phase;
}

public partial class Global : IGlobal {
    public GameState State;
    public float TimeToEndGame; //when the boss is killed, the game must end. But it only end after a certain time
}

//So the Unity-side can react to the game ending and show some UI, etc
public class GameOver : IEvent { }

//Activated when the boss dies, so things needed for the game to be over can be done
public partial class SignalDefine {
    [Signal]
    void OnBossDeath(){ }
}