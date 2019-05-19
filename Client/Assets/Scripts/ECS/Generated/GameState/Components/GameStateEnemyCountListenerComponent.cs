//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameStateEntity {

    public EnemyCountListenerComponent enemyCountListener { get { return (EnemyCountListenerComponent)GetComponent(GameStateComponentsLookup.EnemyCountListener); } }
    public bool hasEnemyCountListener { get { return HasComponent(GameStateComponentsLookup.EnemyCountListener); } }

    public void AddEnemyCountListener(System.Collections.Generic.List<IEnemyCountListener> newValue) {
        var index = GameStateComponentsLookup.EnemyCountListener;
        var component = CreateComponent<EnemyCountListenerComponent>(index);
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceEnemyCountListener(System.Collections.Generic.List<IEnemyCountListener> newValue) {
        var index = GameStateComponentsLookup.EnemyCountListener;
        var component = CreateComponent<EnemyCountListenerComponent>(index);
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveEnemyCountListener() {
        RemoveComponent(GameStateComponentsLookup.EnemyCountListener);
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
public sealed partial class GameStateMatcher {

    static Entitas.IMatcher<GameStateEntity> _matcherEnemyCountListener;

    public static Entitas.IMatcher<GameStateEntity> EnemyCountListener {
        get {
            if (_matcherEnemyCountListener == null) {
                var matcher = (Entitas.Matcher<GameStateEntity>)Entitas.Matcher<GameStateEntity>.AllOf(GameStateComponentsLookup.EnemyCountListener);
                matcher.componentNames = GameStateComponentsLookup.componentNames;
                _matcherEnemyCountListener = matcher;
            }

            return _matcherEnemyCountListener;
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.EventEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameStateEntity {

    public void AddEnemyCountListener(IEnemyCountListener value) {
        var listeners = hasEnemyCountListener
            ? enemyCountListener.value
            : new System.Collections.Generic.List<IEnemyCountListener>();
        listeners.Add(value);
        ReplaceEnemyCountListener(listeners);
    }

    public void RemoveEnemyCountListener(IEnemyCountListener value, bool removeComponentWhenEmpty = true) {
        var listeners = enemyCountListener.value;
        listeners.Remove(value);
        if (removeComponentWhenEmpty && listeners.Count == 0) {
            RemoveEnemyCountListener();
        } else {
            ReplaceEnemyCountListener(listeners);
        }
    }
}