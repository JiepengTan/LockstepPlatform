//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class SnapshotEntity {

    public Lockstep.Core.State.Snapshot.HashCodeComponent hashCode { get { return (Lockstep.Core.State.Snapshot.HashCodeComponent)GetComponent(SnapshotComponentsLookup.HashCode); } }
    public bool hasHashCode { get { return HasComponent(SnapshotComponentsLookup.HashCode); } }

    public void AddHashCode(long newValue) {
        var index = SnapshotComponentsLookup.HashCode;
        var component = CreateComponent<Lockstep.Core.State.Snapshot.HashCodeComponent>(index);
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceHashCode(long newValue) {
        var index = SnapshotComponentsLookup.HashCode;
        var component = CreateComponent<Lockstep.Core.State.Snapshot.HashCodeComponent>(index);
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveHashCode() {
        RemoveComponent(SnapshotComponentsLookup.HashCode);
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
public sealed partial class SnapshotMatcher {

    static Entitas.IMatcher<SnapshotEntity> _matcherHashCode;

    public static Entitas.IMatcher<SnapshotEntity> HashCode {
        get {
            if (_matcherHashCode == null) {
                var matcher = (Entitas.Matcher<SnapshotEntity>)Entitas.Matcher<SnapshotEntity>.AllOf(SnapshotComponentsLookup.HashCode);
                matcher.componentNames = SnapshotComponentsLookup.componentNames;
                _matcherHashCode = matcher;
            }

            return _matcherHashCode;
        }
    }
}