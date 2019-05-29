using UnityEngine;
using UnityEngine.UI;

namespace Lockstep.Game {
    [System.Serializable]
    public class UIManager : SingletonManager<UIManager> {
        public override void DoStart(){
            base.DoStart();
            var cam = Camera.main;
            var rt = new RenderTexture(768,768,16);
            cam.targetTexture = rt;
            var rawImage = GameObject.Find("Canvas/UICanvas/GameBG/RawImage")?.GetComponent<RawImage>();
            if (rawImage != null) {
                rawImage.texture = rt;
            }
        }
    }
}