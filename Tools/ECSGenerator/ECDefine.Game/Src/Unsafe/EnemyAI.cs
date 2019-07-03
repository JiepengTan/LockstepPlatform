using Lockstep.ECS.ECDefine;

public class State : IAsset { }

public class Action : IAsset { }

public class Decision : IAsset { }

//Each single enemy will have its own FSM component
//This way, every enemy can have its own InitialState informing "how this enemy begins to act"
//and can have its CurrentState informing "how is this enemy acting right now"
public class FSM : IComponent {
    AssetRef<State> InitialState;
    AssetRef<State> CurrentState;
}