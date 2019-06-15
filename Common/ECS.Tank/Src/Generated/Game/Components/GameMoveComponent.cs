//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public Lockstep.ECS.Game.MoveComponent move { get { return (Lockstep.ECS.Game.MoveComponent)GetComponent(GameComponentsLookup.Move); } }
    public bool hasMove { get { return HasComponent(GameComponentsLookup.Move); } }

    public void AddMove(Lockstep.Math.LFloat newMoveSpd, Lockstep.Math.LFloat newMaxMoveSpd, bool newIsChangedDir) {
        var index = GameComponentsLookup.Move;
        var component = (Lockstep.ECS.Game.MoveComponent)CreateComponent(index, typeof(Lockstep.ECS.Game.MoveComponent));
        component.moveSpd = newMoveSpd;
        component.maxMoveSpd = newMaxMoveSpd;
        component.isChangedDir = newIsChangedDir;
        AddComponent(index, component);
    }

    public void ReplaceMove(Lockstep.Math.LFloat newMoveSpd, Lockstep.Math.LFloat newMaxMoveSpd, bool newIsChangedDir) {
        var index = GameComponentsLookup.Move;
        var component = (Lockstep.ECS.Game.MoveComponent)CreateComponent(index, typeof(Lockstep.ECS.Game.MoveComponent));
        component.moveSpd = newMoveSpd;
        component.maxMoveSpd = newMaxMoveSpd;
        component.isChangedDir = newIsChangedDir;
        ReplaceComponent(index, component);
    }

    public void RemoveMove() {
        RemoveComponent(GameComponentsLookup.Move);
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
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherMove;

    public static Entitas.IMatcher<GameEntity> Move {
        get {
            if (_matcherMove == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Move);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherMove = matcher;
            }

            return _matcherMove;
        }
    }
}