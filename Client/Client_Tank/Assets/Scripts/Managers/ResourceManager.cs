using System.Collections;
using System.Collections.Generic;
using Lockstep.Math;
using NetMsg.Common;
using UnityEngine;

namespace Lockstep.Game {

    public partial class ResourceManager : SingletonManager<ResourceManager>, IResourceService {
        public override void DoStart(){
            base.DoStart();
            _tranParent = transform;
            _config = Resources.Load<GameConfig>(GameConfig.ConfigPath);
            _assetBindGroup = _gameContext.GetGroup(GameMatcher.AllOf(
                GameMatcher.LocalId,
                GameMatcher.Asset,
                GameMatcher.Pos,
                GameMatcher.Dir));
        }

        private GameConfig _config;
        private ResProxy head;
        private ResProxy tail;


        public void ShowDiedEffect(LVector2 pos){
            CreateEffect(_config.DiedPrefab, pos);
        }

        public void ShowBornEffect(LVector2 pos){
            CreateEffect(_config.BornPrefab, pos);
        }

        void CreateEffect(GameObject prefab, LVector2 pos){
            var liveTime = prefab.GetComponent<RollbackableRes>().liveTime;
            GameObject go = null;
            if (!_constStateService.IsVideoLoading) {
                go = GameObject.Instantiate(prefab, transform.position + pos.ToVector3(), Quaternion.identity);
            }

            var comp = new ResProxy();
            if (comp != null) {
                if (tail == null) {
                    head = tail = comp;
                }
                else {
                    comp.pre = tail;
                    tail.next = comp;
                    tail = comp;
                }

                comp.DoStart(CurTick, go?.GetComponent<RollbackableRes>(), liveTime);
            }
        }

        void Destroy(ResProxy node){
            if (node.res != null) {
                GameObject.Destroy(node.res.gameObject);
            }
        }

        public override void Backup(int tick){
            var node = head;
            var temp = node;
            while (node != null) {
                temp = node;
                node = node.next;
                if (temp.IsLive(tick)) {
                    temp.DoUpdate(tick);
                }
                else {
                    if (head == temp) {
                        head = temp.next;
                    }

                    if (tail == temp) {
                        tail = temp.pre;
                    }

                    if (temp.pre != null) {
                        temp.pre.next = temp.next;
                    }

                    if (temp.next != null) {
                        temp.next.pre = temp.pre;
                    }

                    Destroy(temp);
                }
            }
        }
    }
}