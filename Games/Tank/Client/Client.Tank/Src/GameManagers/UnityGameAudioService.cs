using UnityEngine;

namespace Lockstep.Game {

    
    [System.Serializable]
    public class UnityGameAudioService : UnityBaseGameService,IGameAudioService {
        private UnityAudioService _unityAudioSvc;
        private static string _audioConfigPath = "AudioConfig";
        private AudioConfig _config;
        public override void DoStart(){
            base.DoStart();
            _unityAudioSvc = ( _audioService) as UnityAudioService;
            _config = Resources.Load<AudioConfig>(_audioConfigPath);
            _config?.DoStart();
        }
        
        void OnEvent_OnAllPlayerFinishedLoad(object param){
            PlayMusicStart();
        }
        
        public void PlayClipDestroyGrass(){ _unityAudioSvc.PlayClip(_config.destroyGrass); }
        public void PlayClipBorn(){ _unityAudioSvc.PlayClip(_config.born); }
        public void PlayClipDied(){ _unityAudioSvc.PlayClip(_config.died); }
        public void PlayClipHitTank(){ _unityAudioSvc.PlayClip(_config.hitTank); }
        public void PlayClipHitIron(){ _unityAudioSvc.PlayClip(_config.hitIron); }
        public void PlayClipHitBrick(){ _unityAudioSvc.PlayClip(_config.hitBrick); }
        public void PlayClipDestroyIron(){ _unityAudioSvc.PlayClip(_config.destroyIron); }
        public void PlayMusicBG(){ _unityAudioSvc.PlayClip(_config.bgMusic); }
        public void PlayMusicStart(){ _unityAudioSvc.PlayClip(_config.startMusic); }
        public void PlayMusicGetItem(){ _unityAudioSvc.PlayClip(_config.addItem); }
        
    }
}