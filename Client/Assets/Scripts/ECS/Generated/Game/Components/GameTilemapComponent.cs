//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentContextApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameContext {

    public GameEntity tilemapEntity { get { return GetGroup(GameMatcher.Tilemap).GetSingleEntity(); } }
    public Lockstep.ECS.Game.TilemapComponent tilemap { get { return tilemapEntity.tilemap; } }
    public bool hasTilemap { get { return tilemapEntity != null; } }

    public GameEntity SetTilemap(byte[,] newTileIds) {
        if (hasTilemap) {
            throw new Entitas.EntitasException("Could not set Tilemap!\n" + this + " already has an entity with Lockstep.ECS.Game.TilemapComponent!",
                "You should check if the context already has a tilemapEntity before setting it or use context.ReplaceTilemap().");
        }
        var entity = CreateEntity();
        entity.AddTilemap(newTileIds);
        return entity;
    }

    public void ReplaceTilemap(byte[,] newTileIds) {
        var entity = tilemapEntity;
        if (entity == null) {
            entity = SetTilemap(newTileIds);
        } else {
            entity.ReplaceTilemap(newTileIds);
        }
    }

    public void RemoveTilemap() {
        tilemapEntity.Destroy();
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public Lockstep.ECS.Game.TilemapComponent tilemap { get { return (Lockstep.ECS.Game.TilemapComponent)GetComponent(GameComponentsLookup.Tilemap); } }
    public bool hasTilemap { get { return HasComponent(GameComponentsLookup.Tilemap); } }

    public void AddTilemap(byte[,] newTileIds) {
        var index = GameComponentsLookup.Tilemap;
        var component = CreateComponent<Lockstep.ECS.Game.TilemapComponent>(index);
        component.tileIds = newTileIds;
        AddComponent(index, component);
    }

    public void ReplaceTilemap(byte[,] newTileIds) {
        var index = GameComponentsLookup.Tilemap;
        var component = CreateComponent<Lockstep.ECS.Game.TilemapComponent>(index);
        component.tileIds = newTileIds;
        ReplaceComponent(index, component);
    }

    public void RemoveTilemap() {
        RemoveComponent(GameComponentsLookup.Tilemap);
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

    static Entitas.IMatcher<GameEntity> _matcherTilemap;

    public static Entitas.IMatcher<GameEntity> Tilemap {
        get {
            if (_matcherTilemap == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.Tilemap);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherTilemap = matcher;
            }

            return _matcherTilemap;
        }
    }
}
