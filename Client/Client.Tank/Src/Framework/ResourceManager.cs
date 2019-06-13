﻿using System;
using System.Collections.Generic;
using Entitas;
using Entitas.Unity;
using Entitas.VisualDebugging.Unity;
using UnityEngine;
using Lockstep.Math;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Game {
    public partial class ResourceManager : IViewService {
        private Dictionary<uint, GameObject> _linkedEntities = new Dictionary<uint, GameObject>();
     
        private IGroup<GameEntity> _assetBindGroup;

        private Transform _tranParent;
        public void BindView(GameEntity entity,object viewObj){
            var viewGo = viewObj as GameObject;
            if (viewGo != null) {
                if (!viewGo.activeSelf) {
                    viewGo.SetActive(true);
                }

                viewGo.Link(entity);
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

        public void RebindAllEntities(){
            var entities = _assetBindGroup.GetEntities();
            foreach (var enttiy in entities) {
                RebindView(enttiy);
            }
        }

        public void RebindView(GameEntity entity){
            var id = entity.localId.value;
            if (_linkedEntities.ContainsKey(id)) {
                return;
            }

            var assetId = entity.asset.assetId;
            var prefab = Resources.Load<GameObject>(GameConfig.GetAssetPath(assetId));
            var go = GameObject.Instantiate(prefab,
                _tranParent.position + entity.pos.value.ToVector3(),
                Quaternion.Euler(0,0,DirUtil.GetDirDeg(entity.dir.value)), _tranParent);
            go.AddComponent<PosListener>();
            go.AddComponent<DirListener>();
            BindView(entity,go);
        }

        public void DeleteView(uint entityId){
            if (!_linkedEntities.ContainsKey(entityId)) return;
            var viewGo = _linkedEntities[entityId];
            var eventListeners = viewGo.GetComponents<IEventListener>();
            foreach (var listener in eventListeners) {
                listener.UnRegisterListeners();
            }

            _linkedEntities[entityId].Unlink();
            _linkedEntities[entityId].DestroyGameObject();
            _linkedEntities.Remove(entityId);
        }
    }
}