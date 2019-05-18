using System;
using System.Collections.Generic;
using Lockstep.Game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lockstep.Game {

    [Serializable]
    public class Effect {
        public GameObject prefab;
        public Vector2 offset;
        [HideInInspector] private GameObject prefabInstance;
        public bool isNeedTrackInstaceGo = false;

        public void Play(Vector2 pos){
            if (prefab != null) {
                var go = GameObject.Instantiate(prefab, pos + offset, Quaternion.identity);
                if (isNeedTrackInstaceGo) {
                    prefabInstance = go;
                }
            }
        }

        public void Stop(){
            if (prefabInstance != null) {
                GameObject.Destroy(prefabInstance);
            }
        }
    }

    [Serializable]
    public class EffectProxy {
        public Effect bornEffect;
        public Effect destoryEffect;
        public Effect useEffect;

        public void PlayEffectBorn(Vector2 pos){
            Play(bornEffect, pos);
        }

        public void PlayEffectDestory(Vector2 pos){
            Play(destoryEffect, pos);
        }

        public void PlayEffectUse(Vector2 pos){
            Play(useEffect, pos);
        }

        public void StopAll(){
            bornEffect?.Stop();
            destoryEffect?.Stop();
            useEffect?.Stop();
        }

        private void Play(Effect effect, Vector2 pos){
            effect?.Play(pos);
        }
    }


    public class Player : Tank { }

    public class Enemy : Tank {
        public AIProxy aiProxy;
    }


    public class Item : Unit {
        public int Type;
        public float lifeTime;
        public bool isEnable = true;

        public void TriggelEffect(Tank unit){
            if (!isEnable) return;
            isEnable = false;
            AudioManager.PlayMusicGetItem();
            OnTriggerEffect(unit);
        }

        protected virtual void OnTriggerEffect(Tank trigger){ }
    }
}