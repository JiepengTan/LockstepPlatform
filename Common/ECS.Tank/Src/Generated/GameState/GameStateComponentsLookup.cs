//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentLookupGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public static class GameStateComponentsLookup {

    public const int BackupCurFrame = 0;
    public const int BeforeExecuteHashCode = 1;
    public const int HashCode = 2;
    public const int Tick = 3;

    public const int TotalComponents = 4;

    public static readonly string[] componentNames = {
        "BackupCurFrame",
        "BeforeExecuteHashCode",
        "HashCode",
        "Tick"
    };

    public static readonly System.Type[] componentTypes = {
        typeof(Lockstep.ECS.GameState.BackupCurFrameComponent),
        typeof(Lockstep.ECS.GameState.BeforeExecuteHashCodeComponent),
        typeof(Lockstep.ECS.GameState.HashCodeComponent),
        typeof(Lockstep.ECS.GameState.TickComponent)
    };
}