using System;
using System.Collections.Generic;
using Entitas;
using Entitas.Unity;
using Entitas.VisualDebugging.Unity;
using UnityEngine;
using Lockstep.Math;
using Debug = Lockstep.Logging.Debug;
using Object = UnityEngine.Object;

namespace Lockstep.Game {
    public partial class UnityViewService : UnityBaseGameService, IViewService {
        private Dictionary<uint, GameObject> _linkedEntities = new Dictionary<uint, GameObject>();
        private IGroup<GameEntity> _assetBindGroup;

        public override void DoStart(){
            base.DoStart();
            _assetBindGroup = ((Contexts) (_constStateService.Contexts)).game.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.Asset,
                GameMatcher.Pos,
                GameMatcher.Dir));
        }

        public void BindView(IEntity entity, ushort assetId, LVector2 createPos, int deg = 0){
            var path = _resService.GetAssetPath(assetId);
            if (string.IsNullOrEmpty(path)) return;
            var prefab = Resources.Load<GameObject>(path);
            var go = GameObject.Instantiate(prefab, transform.position + createPos.ToVector3(),
                Quaternion.Euler(0, 0, deg), transform);
            go.AddComponent<PosListener>();
            go.AddComponent<DirListener>();
            BindView(entity as GameEntity, go);
        }

        public void BindView(GameEntity entity, object viewObj){
            var viewGo = viewObj as GameObject;
            if (viewGo != null) {
                if (!viewGo.activeSelf) {
                    viewGo.SetActive(true);
                }

                viewGo.Link(entity);
                var componentSetters = viewGo.GetComponents<IComponentSetter>();
                foreach (var componentSetter in componentSetters) {
                    componentSetter.SetComponent(entity);
                    Object.Destroy((MonoBehaviour) componentSetter);
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
            foreach (var entity in entities) {
                RebindView(entity);
            }
        }

        public void RebindView(IEntity entity){
            RebindView(entity as GameEntity);
        }

        public void RebindView(GameEntity entity){
            var id = entity.localId.value;
            if (_linkedEntities.ContainsKey(id)) {
                return;
            }

            var assetId = entity.asset.assetId;
            var path = _resService.GetAssetPath((ushort) assetId);
            if (string.IsNullOrEmpty(path)) return;
            var prefab = Resources.Load<GameObject>(path);
            var go = Object.Instantiate(prefab,
                transform.position + entity.pos.value.ToVector3(),
                Quaternion.Euler(0, 0, DirUtil.GetDirDeg(entity.dir.value)), transform);
            go.AddComponent<PosListener>();
            go.AddComponent<DirListener>();
            BindView(entity, go);
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