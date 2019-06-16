//#define USE_UNITY_MONO

using System;
using System.Net.Mime;
using UnityEngine;

namespace Lockstep.Game {
    public class UnityBaseGameService : BaseGameService {
        public Transform transform { get; protected set; }
        public GameObject gameObject { get; protected set; }
        public override void DoUnityAwake(object objParent){
            var parent = objParent as Transform;
            base.DoUnityAwake(parent);
            InitGameObject(parent);
        }
        
        
        private void InitGameObject(Transform parent){
            var go = new GameObject(GetType().Name);
            gameObject = go;
            transform = go.transform;
            transform.SetParent(parent, false);
        }

        public override void DoDestroy(){
            if (gameObject != null) {
                if (Application.isPlaying) {
                    GameObject.Destroy(gameObject);
                }
                else {
                    GameObject.DestroyImmediate(gameObject);
                }
            }
        }
    }
}