using System.Collections.Generic;
using Entitas;

public sealed class CleanDestroyedInputEntities : ICleanupSystem {

    readonly IGroup<InputEntity> _group;
    readonly List<InputEntity> _buffer = new List<InputEntity>();

    public CleanDestroyedInputEntities(Contexts contexts) {
        _group = contexts.input.GetGroup(InputMatcher.Destroyed);
    }

    public void Cleanup() {
        foreach (var e in _group.GetEntities(_buffer)) {
            e.Destroy();
        }
    }
}