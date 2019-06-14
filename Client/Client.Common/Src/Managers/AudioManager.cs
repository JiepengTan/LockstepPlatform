using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Lockstep.Game {


    [System.Serializable]
    public class GameAudioManager : SingletonManager<GameAudioManager>,IGameAudioService {
        private AudioManager _audioManager;
        private AudioConfig _config;
        public void PlayClipDestroyGrass(){ _audioManager.PlayClip(_config.destroyGrass); }
        public void PlayClipBorn(){ _audioManager.PlayClip(_config.born); }
        public void PlayClipDied(){ _audioManager.PlayClip(_config.died); }
        public void PlayClipHitTank(){ _audioManager.PlayClip(_config.hitTank); }
        public void PlayClipHitIron(){ _audioManager.PlayClip(_config.hitIron); }
        public void PlayClipHitBrick(){ _audioManager.PlayClip(_config.hitBrick); }
        public void PlayClipDestroyIron(){ _audioManager.PlayClip(_config.destroyIron); }
        public void PlayMusicBG(){ _audioManager.PlayClip(_config.bgMusic); }
        public void PlayMusicStart(){ _audioManager.PlayClip(_config.startMusic); }
        public void PlayMusicGetItem(){ _audioManager.PlayClip(_config.addItem); }
        
        void OnEvent_OnAllPlayerFinishedLoad(object param){
            PlayMusicStart();
        }
    }

    [System.Serializable]
    public class AudioManager : SingletonManager<AudioManager>, IAudioService {
        
        public static string AudioConfigPath = "AudioConfig";
        private AudioConfig _config;
        public AudioSource _source;
        public override void DoStart(){
            base.DoStart();
            _config =   Resources.Load<AudioConfig>(AudioConfigPath);
            _config?.DoStart();
            _source = gameObject.GetComponent<AudioSource>();
            if ( _source== null) {
                _source = gameObject.AddComponent<AudioSource>();
            }
        }

        public void PlayClip(string clip){
            var audio = _config.GetAudio(clip);
            PlayClip(audio);
        }
        
        public void PlayClip(AudioClip clip){
            if (clip != null) {
                //追帧 不播放音效
                if (_constStateService.IsPursueFrame) {  return;}
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
        
    }

}