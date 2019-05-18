
    using UnityEngine;

    public class DestroySelf : UnityEngine.MonoBehaviour {
        public float delay =4;
        private float timer;

        public void Update(){
            timer += Time.deltaTime;
            if (timer > delay) {
                GameObject.Destroy(gameObject);
            }
        }
    }