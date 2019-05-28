//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ContextsGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class Contexts : Entitas.IContexts {

    public static Contexts sharedInstance {
        get {
            if (_sharedInstance == null) {
                _sharedInstance = new Contexts();
            }

            return _sharedInstance;
        }
        set { _sharedInstance = value; }
    }

    static Contexts _sharedInstance;

    public ActorContext actor { get; set; }
    public ConfigContext config { get; set; }
    public DebugContext debug { get; set; }
    public GameContext game { get; set; }
    public GameStateContext gameState { get; set; }
    public InputContext input { get; set; }
    public SnapshotContext snapshot { get; set; }

    public Entitas.IContext[] allContexts { get { return new Entitas.IContext [] { actor, config, debug, game, gameState, input, snapshot }; } }

    public Contexts() {
        actor = new ActorContext();
        config = new ConfigContext();
        debug = new DebugContext();
        game = new GameContext();
        gameState = new GameStateContext();
        input = new InputContext();
        snapshot = new SnapshotContext();

        var postConstructors = System.Linq.Enumerable.Where(
            GetType().GetMethods(),
            method => System.Attribute.IsDefined(method, typeof(Entitas.CodeGeneration.Attributes.PostConstructorAttribute))
        );

        foreach (var postConstructor in postConstructors) {
            postConstructor.Invoke(this, null);
        }
    }

    public void Reset() {
        var contexts = allContexts;
        for (int i = 0; i < contexts.Length; i++) {
            contexts[i].Reset();
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.EntityIndexGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class Contexts {

    public const string Id = "Id";
    public const string LocalId = "LocalId";
    public const string Tick = "Tick";

    [Entitas.CodeGeneration.Attributes.PostConstructor]
    public void InitializeEntityIndices() {
        actor.AddEntityIndex(new Entitas.PrimaryEntityIndex<ActorEntity, byte>(
            Id,
            actor.GetGroup(ActorMatcher.Id),
            (e, c) => ((Lockstep.ECS.Actor.IdComponent)c).value));

        game.AddEntityIndex(new Entitas.PrimaryEntityIndex<GameEntity, uint>(
            LocalId,
            game.GetGroup(GameMatcher.LocalId),
            (e, c) => ((Lockstep.ECS.Game.LocalIdComponent)c).value));

        snapshot.AddEntityIndex(new Entitas.PrimaryEntityIndex<SnapshotEntity, int>(
            Tick,
            snapshot.GetGroup(SnapshotMatcher.Tick),
            (e, c) => ((Lockstep.ECS.Snapshot.TickComponent)c).value));
    }
}

public static class ContextsExtensions {

    public static ActorEntity GetEntityWithId(this ActorContext context, byte value) {
        return ((Entitas.PrimaryEntityIndex<ActorEntity, byte>)context.GetEntityIndex(Contexts.Id)).GetEntity(value);
    }

    public static GameEntity GetEntityWithLocalId(this GameContext context, uint value) {
        return ((Entitas.PrimaryEntityIndex<GameEntity, uint>)context.GetEntityIndex(Contexts.LocalId)).GetEntity(value);
    }

    public static SnapshotEntity GetEntityWithTick(this SnapshotContext context, int value) {
        return ((Entitas.PrimaryEntityIndex<SnapshotEntity, int>)context.GetEntityIndex(Contexts.Tick)).GetEntity(value);
    }
}
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.VisualDebugging.CodeGeneration.Plugins.ContextObserverGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class Contexts {

#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)

    [Entitas.CodeGeneration.Attributes.PostConstructor]
    public void InitializeContextObservers() {
        try {
            CreateContextObserver(actor);
            CreateContextObserver(config);
            CreateContextObserver(debug);
            CreateContextObserver(game);
            CreateContextObserver(gameState);
            CreateContextObserver(input);
            CreateContextObserver(snapshot);
        } catch(System.Exception) {
        }
    }

    public void CreateContextObserver(Entitas.IContext context) {
        if (UnityEngine.Application.isPlaying) {
            var observer = new Entitas.VisualDebugging.Unity.ContextObserver(context);
            UnityEngine.Object.DontDestroyOnLoad(observer.gameObject);
        }
    }

#endif
}
