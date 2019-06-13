using System.Collections;
using System.Collections.Generic;
using Lockstep.Math;
using NetMsg.Common;
using UnityEngine;

namespace Lockstep.Game {
    public class RollbackableRes : UnityEngine.MonoBehaviour {
        [HideInInspector] public ResProxy __proxy;

        [HideInInspector] public int createTick => __proxy.createTick;
        [HideInInspector] public int diedTick => __proxy.diedTick;
        public LFloat liveTime;

        public virtual void DoStart(int curTick){
            
        }
        public virtual void DoUpdate(int tick){ }
    }

    public class ResProxy {
        public RollbackableRes res;
        public ResProxy pre;
        public ResProxy next;

        public int createTick;
        public int diedTick;
        public LFloat liveTime;

        public virtual void DoStart(int curTick, RollbackableRes res, LFloat liveTime){
            this.liveTime = liveTime;
            createTick = curTick;
            diedTick = curTick + (liveTime * NetworkDefine.FRAME_RATE).ToInt();
            this.res = res;
            if (res != null) {
                res.__proxy = this;
                res.DoStart(curTick);
            }
        }

        public bool IsLive(int curTick){
            return curTick >= createTick && curTick <= diedTick;
        }

        public virtual void DoUpdate(int tick){
            this.res?.DoUpdate(tick);
        }
    }

    public partial class ResourceManager : IResourceService {
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