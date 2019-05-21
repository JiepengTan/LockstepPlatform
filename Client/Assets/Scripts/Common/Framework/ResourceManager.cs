using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;
using Entitas.Unity;
using Entitas.VisualDebugging.Unity;

namespace Lockstep.Game {
    public partial class ResourceManager : SingletonManager<ResourceManager>, IViewService {
        private Dictionary<uint, GameObject> _linkedEntities = new Dictionary<uint, GameObject>();

        public void BindView(GameEntity entity, IContext ctx, object viewObj){
            var viewGo = viewObj as GameObject;
            if (viewGo != null) {
                if (!viewGo.activeSelf) {
                    viewGo.SetActive(true);
                }

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

                _linkedEntities.Add(entity.localId.value, viewGo);
            }
        }

        public void DeleteView(uint entityId){
            var viewGo = _linkedEntities[entityId];
            var eventListeners = viewGo.GetComponents<IEventListener>();
            foreach (var listener in eventListeners) {
                listener.UnregisterListeners();
            }

            _linkedEntities[entityId].Unlink();
            _linkedEntities[entityId].DestroyGameObject();
            _linkedEntities.Remove(entityId);
        }
    }
}