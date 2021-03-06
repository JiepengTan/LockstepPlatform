//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public DirListenerComponent dirListener { get { return (DirListenerComponent)GetComponent(GameComponentsLookup.DirListener); } }
    public bool hasDirListener { get { return HasComponent(GameComponentsLookup.DirListener); } }

    public void AddDirListener(System.Collections.Generic.List<IDirListener> newValue) {
        var index = GameComponentsLookup.DirListener;
        var component = (DirListenerComponent)CreateComponent(index, typeof(DirListenerComponent));
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceDirListener(System.Collections.Generic.List<IDirListener> newValue) {
        var index = GameComponentsLookup.DirListener;
        var component = (DirListenerComponent)CreateComponent(index, typeof(DirListenerComponent));
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveDirListener() {
        RemoveComponent(GameComponentsLookup.DirListener);
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

    static Entitas.IMatcher<GameEntity> _matcherDirListener;

    public static Entitas.IMatcher<GameEntity> DirListener {
        get {
            if (_matcherDirListener == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.DirListener);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherDirListener = matcher;
            }

            return _matcherDirListener;
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
public partial class GameEntity {

    public void AddDirListener(IDirListener value) {
        var listeners = hasDirListener
            ? dirListener.value
            : new System.Collections.Generic.List<IDirListener>();
        listeners.Add(value);
        ReplaceDirListener(listeners);
    }

    public void RemoveDirListener(IDirListener value, bool removeComponentWhenEmpty = true) {
        var listeners = dirListener.value;
        listeners.Remove(value);
        if (removeComponentWhenEmpty && listeners.Count == 0) {
            RemoveDirListener();
        } else {
            ReplaceDirListener(listeners);
        }
    }
}
