using UnityEngine;

namespace Lockstep.Game {
    [System.Serializable]
    public class AudioManager : SingletonManager<AudioManager> {
        public static void PlayClipDestroyGrass(){
            Instance.PlayClip(Instance.destroyGrass);
        }

        public static void PlayClipBorn(){
            Instance.PlayClip(Instance.born);
        }

        public static void PlayClipDied(){
            Instance.PlayClip(Instance.died);
        }

        public static void PlayClipHitTank(){
            Instance.PlayClip(Instance.hitTank);
        }

        public static void PlayClipHitIron(){
            Instance.PlayClip(Instance.hitIron);
        }

        public static void PlayClipHitBrick(){
            Instance.PlayClip(Instance.hitBrick);
        }

        public static void PlayClipDestroyIron(){
            Instance.PlayClip(Instance.destroyIron);
        }

        public static void PlayMusicBG(){
            Instance.PlayClip(Instance.bgMusic);
        }

        public static void PlayMusicStart(){
            Instance.PlayClip(Instance.startMusic);
        }

        public static void PlayMusicGetItem(){
            Instance.PlayClip(Instance.addItem);
        }

        public AudioClip born;
        public AudioClip died;
        public AudioClip hitTank;
        public AudioClip hitBrick;
        public AudioClip hitIron;
        public AudioClip destroyIron;
        public AudioClip destroyGrass;
        public AudioClip addItem;
        public AudioClip bgMusic;
        public AudioClip startMusic;
        public AudioSource Source;

        public void PlayClip(AudioClip clip){
            if (clip != null) {
                Source.PlayOneShot(clip);
            }
        }
    }
}