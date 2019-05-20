using UnityEngine;

namespace Lockstep.Game {


    [System.Serializable]
    public class AudioManager : SingletonManager<AudioManager>,IAudioService {
        public void PlayClipDestroyGrass(){ PlayClip(destroyGrass); }
        public void PlayClipBorn(){ PlayClip(born); }
        public void PlayClipDied(){ PlayClip(died); }
        public void PlayClipHitTank(){ PlayClip(hitTank); }
        public void PlayClipHitIron(){ PlayClip(hitIron); }
        public void PlayClipHitBrick(){ PlayClip(hitBrick); }
        public void PlayClipDestroyIron(){ PlayClip(destroyIron); }
        public void PlayMusicBG(){ PlayClip(bgMusic); }
        public void PlayMusicStart(){ PlayClip(startMusic); }
        public void PlayMusicGetItem(){ PlayClip(addItem); }
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