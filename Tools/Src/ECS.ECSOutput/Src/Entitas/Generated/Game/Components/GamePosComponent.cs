//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public Lockstep.ECS.Game.PosComponent pos { get { return (Lockstep.ECS.Game.PosComponent)GetComponent(GameComponentsLookup.Pos); } }
    public bool hasPos { get { return HasComponent(GameComponentsLookup.Pos); } }

    public void AddPos(Lockstep.Math.LVector2 newValue) {
        var index = GameComponentsLookup.Pos;
        var component = (Lockstep.ECS.Game.PosComponent)CreateComponent(index, typeof(Lockstep.ECS.Game.PosComponent));
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplacePos(Lockstep.Math.LVector2 newValue) {
        var index = GameComponentsLookup.Pos;
        var component = (Lockstep.ECS.Game.PosComponent)CreateComponent(index, typeof(Lockstep.ECS.Game.PosComponent));
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemovePos() {
        RemoveComponent(GameComponentsLookup.Pos);
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

    static Entitas.IMatcher<GameEntity> _matcherPos;

    public static Entitas.IMatcher<GameEntity> Pos {
        get {
            if (_matcherPos == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Pos);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherPos = matcher;
            }

            return _matcherPos;
        }
    }
}
