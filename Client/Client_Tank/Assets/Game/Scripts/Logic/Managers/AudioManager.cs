using System.Collections.Generic;
using UnityEngine;

namespace Lockstep.Game {


    [System.Serializable]
    public class AudioManager : SingletonManager<AudioManager>,IAudioService {

        public static string AudioConfigPath = "AudioConfig";
        private AudioConfig _config;
        public AudioSource _source;
        public override void DoStart(){
            base.DoStart();
            _config =   Resources.Load<AudioConfig>(AudioConfigPath);
            _source = gameObject.GetComponent<AudioSource>();
            if ( _source== null) {
                _source = gameObject.AddComponent<AudioSource>();
            }
        }

        public void PlayClipDestroyGrass(){ PlayClip(_config.destroyGrass); }
        public void PlayClipBorn(){ PlayClip(_config.born); }
        public void PlayClipDied(){ PlayClip(_config.died); }
        public void PlayClipHitTank(){ PlayClip(_config.hitTank); }
        public void PlayClipHitIron(){ PlayClip(_config.hitIron); }
        public void PlayClipHitBrick(){ PlayClip(_config.hitBrick); }
        public void PlayClipDestroyIron(){ PlayClip(_config.destroyIron); }
        public void PlayMusicBG(){ PlayClip(_config.bgMusic); }
        public void PlayMusicStart(){ PlayClip(_config.startMusic); }
        public void PlayMusicGetItem(){ PlayClip(_config.addItem); }
        
        public void PlayClip(AudioClip clip){
            if (clip != null) {
                //追帧 不播放音效
                if (_constStateService.isPursueFrame) {  return;}
                //回放 不播放音效
                if (_constStateService.IsVideoMode && !_constStateService.IsRunVideo) {  return;}

                if (curFramePlayeredCount.TryGetValue(clip, out int val)) {
                    curFramePlayeredCount[clip] = val + 1;
                }
                else {
                    curFramePlayeredCount.Add(clip,1);
                }
                if(curFramePlayeredCount[clip] >= 2) return;//不播放大于2个的音效 听不出来
                _source.PlayOneShot(clip);
            }
        }
        Dictionary<AudioClip,int> curFramePlayeredCount = new Dictionary<AudioClip, int>();
        public override void Backup(int tick){
            curFramePlayeredCount.Clear();
        }
        void OnEvent_OnAllPlayerFinishedLoad(object param){
            PlayMusicStart();
        }

    }
}