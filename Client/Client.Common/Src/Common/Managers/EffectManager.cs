using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    public partial class EffectManager : SingletonManager<EffectManager>, IEffectService {
        private EffectProxy head;
        private EffectProxy tail;

        public void CreateEffect(int assetId, LVector2 pos){ }

        public void CreateEffect(GameObject prefab, LVector2 pos){
            var liveTime = prefab.GetComponent<RollbackEffect>().liveTime;
            GameObject go = null;
            if (!_constStateService.IsVideoLoading) {
                go = GameObject.Instantiate(prefab, transform.position + pos.ToVector3(), Quaternion.identity);
            }

            var comp = new EffectProxy();
            if (comp != null) {
                if (tail == null) {
                    head = tail = comp;
                }
                else {
                    comp.pre = tail;
                    tail.next = comp;
                    tail = comp;
                }

                comp.DoStart(CurTick, go?.GetComponent<RollbackEffect>(), liveTime);
            }
        }

        public void DestroyEffect(EffectProxy node){
            if (node.Effect != null) {
                var comp = node.Effect as Component;
                GameObject.Destroy(comp.gameObject);
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

                    DestroyEffect(temp);
                }
            }
        }
    }
}