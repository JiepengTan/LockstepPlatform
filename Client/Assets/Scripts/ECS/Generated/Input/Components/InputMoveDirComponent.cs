//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class InputEntity {

    public MoveDirComponent moveDir { get { return (MoveDirComponent)GetComponent(InputComponentsLookup.MoveDir); } }
    public bool hasMoveDir { get { return HasComponent(InputComponentsLookup.MoveDir); } }

    public void AddMoveDir(Lockstep.Math.LVector2 newValue) {
        var index = InputComponentsLookup.MoveDir;
        var component = CreateComponent<MoveDirComponent>(index);
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceMoveDir(Lockstep.Math.LVector2 newValue) {
        var index = InputComponentsLookup.MoveDir;
        var component = CreateComponent<MoveDirComponent>(index);
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveMoveDir() {
        RemoveComponent(InputComponentsLookup.MoveDir);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class InputMatcher {

    static Entitas.IMatcher<InputEntity> _matcherMoveDir;

    public static Entitas.IMatcher<InputEntity> MoveDir {
        get {
            if (_matcherMoveDir == null) {
                var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.MoveDir);
                matcher.componentNames = InputComponentsLookup.componentNames;
                _matcherMoveDir = matcher;
            }

            return _matcherMoveDir;
        }
    }
}