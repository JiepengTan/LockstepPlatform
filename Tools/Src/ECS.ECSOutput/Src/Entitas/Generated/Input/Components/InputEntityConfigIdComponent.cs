//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class InputEntity {

    public Lockstep.ECS.Input.EntityConfigIdComponent entityConfigId { get { return (Lockstep.ECS.Input.EntityConfigIdComponent)GetComponent(InputComponentsLookup.EntityConfigId); } }
    public bool hasEntityConfigId { get { return HasComponent(InputComponentsLookup.EntityConfigId); } }

    public void AddEntityConfigId(int newValue) {
        var index = InputComponentsLookup.EntityConfigId;
        var component = (Lockstep.ECS.Input.EntityConfigIdComponent)CreateComponent(index, typeof(Lockstep.ECS.Input.EntityConfigIdComponent));
        component.value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceEntityConfigId(int newValue) {
        var index = InputComponentsLookup.EntityConfigId;
        var component = (Lockstep.ECS.Input.EntityConfigIdComponent)CreateComponent(index, typeof(Lockstep.ECS.Input.EntityConfigIdComponent));
        component.value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveEntityConfigId() {
        RemoveComponent(InputComponentsLookup.EntityConfigId);
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

    static Entitas.IMatcher<InputEntity> _matcherEntityConfigId;

    public static Entitas.IMatcher<InputEntity> EntityConfigId {
        get {
            if (_matcherEntityConfigId == null) {
                var matcher = (Entitas.Matcher<InputEntity>)Entitas.Matcher<InputEntity>.AllOf(InputComponentsLookup.EntityConfigId);
                matcher.componentNames = InputComponentsLookup.componentNames;
                _matcherEntityConfigId = matcher;
            }

            return _matcherEntityConfigId;
        }
    }
}