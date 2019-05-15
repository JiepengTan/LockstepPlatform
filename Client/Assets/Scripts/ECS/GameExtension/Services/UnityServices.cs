using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;
using Entitas.Unity;
using Entitas.VisualDebugging.Unity;
using Lockstep.Game.Interfaces;

public interface IEventListener {
    void RegisterListeners(GameEntity entity);
    void UnregisterListeners();
}

public interface IComponentSetter {
    void SetComponent(GameEntity entity);
}

public class UnityGameService : IViewService {
    private Dictionary<uint, GameObject> linkedEntities = new Dictionary<uint, GameObject>();
    private GameObject prefab;
    private Transform transParent;

    public UnityGameService(){
        transParent = new GameObject("GoParents").transform;
        prefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
        prefab.SetActive(false);
        prefab.transform.SetParent(transParent, false);
        prefab.AddComponent<HashCodeSetter>();
        prefab.AddComponent<PositionListener>();
    }

    public void LoadView(GameEntity entity, int configId, IContext ctx){
        //TODO: pooling    
        var viewGo = UnityEngine.Object.Instantiate(prefab, transParent).gameObject;
        if (viewGo != null) {
            viewGo.SetActive(true);
            viewGo.Link(entity, ctx);

            var componentSetters = viewGo.GetComponents<IComponentSetter>();
            foreach (var componentSetter in componentSetters) {
                componentSetter.SetComponent(entity);
                UnityEngine.Object.Destroy((MonoBehaviour) componentSetter);
            }

            var eventListeners = viewGo.GetComponents<IEventListener>();
            foreach (var listener in eventListeners) {
                listener.RegisterListeners(entity);
            }

            linkedEntities.Add(entity.localId.value, viewGo);
        }
    }

    public void DeleteView(uint entityId){
        var viewGo = linkedEntities[entityId];
        var eventListeners = viewGo.GetComponents<IEventListener>();
        foreach (var listener in eventListeners) {
            listener.UnregisterListeners();
        }

        linkedEntities[entityId].Unlink();
        linkedEntities[entityId].DestroyGameObject();
        linkedEntities.Remove(entityId);
    }
}