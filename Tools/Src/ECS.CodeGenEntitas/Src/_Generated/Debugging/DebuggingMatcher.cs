//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ContextMatcherGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class DebuggingMatcher {

    public static Entitas.IAllOfMatcher<DebuggingEntity> AllOf(params int[] indices) {
        return Entitas.Matcher<DebuggingEntity>.AllOf(indices);
    }

    public static Entitas.IAllOfMatcher<DebuggingEntity> AllOf(params Entitas.IMatcher<DebuggingEntity>[] matchers) {
          return Entitas.Matcher<DebuggingEntity>.AllOf(matchers);
    }

    public static Entitas.IAnyOfMatcher<DebuggingEntity> AnyOf(params int[] indices) {
          return Entitas.Matcher<DebuggingEntity>.AnyOf(indices);
    }

    public static Entitas.IAnyOfMatcher<DebuggingEntity> AnyOf(params Entitas.IMatcher<DebuggingEntity>[] matchers) {
          return Entitas.Matcher<DebuggingEntity>.AnyOf(matchers);
    }
}
