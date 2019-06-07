using System.Collections;
using System.Collections.Generic;
using Lockstep.Math;
using NetMsg.Common;
using UnityEngine;

namespace Lockstep.Game {
    public class RollbackableRes : UnityEngine.MonoBehaviour {
        [HideInInspector] public RollbackableRes pre;
        [HideInInspector] public RollbackableRes next;

        [HideInInspector] public int createTick;
        [HideInInspector] public int diedTick;
        public LFloat liveTime;

        public virtual void DoStart(int curTick){
            createTick = curTick;
            diedTick = curTick + (liveTime * NetworkDefine.FRAME_RATE).ToInt();
        }

        public bool IsLive(int curTick){
            return curTick >= createTick && curTick <= diedTick;
        }

        public virtual void DoUpdate(int tick){ }
    }

    public partial class ResourceManager : IResourceService {
        private GameConfig _config;
        private RollbackableRes head;
        private RollbackableRes tail;


        public void ShowDiedEffect(LVector2 pos){
            CreateEffect(_config.DiedPrefab, pos);
        }

        public void ShowBornEffect(LVector2 pos){
            CreateEffect(_config.BornPrefab, pos);
        }

        void CreateEffect(GameObject prefab, LVector2 pos){
            var go = GameObject.Instantiate(prefab, transform.position + pos.ToVector3(),
                Quaternion.identity);
            var comp = go.GetComponent<RollbackableRes>();
            if (comp != null) {
                if (tail == null) {
                    head = tail = comp;
                }
                else {
                    comp.pre = tail;
                    tail.next = comp;
                    tail = comp;
                }

                comp.DoStart(CurTick);
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

        void Destroy(RollbackableRes node){
            GameObject.Destroy(node.gameObject);
        }
    }
}