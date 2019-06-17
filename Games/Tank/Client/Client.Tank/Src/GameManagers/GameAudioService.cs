using UnityEngine;

namespace Lockstep.Game {

    
    [System.Serializable]
    public class GameAudioService : UnityBaseGameService,IGameAudioService {
        private AudioService _audioSvc;
        private static string _audioConfigPath = "AudioConfig";
        private AudioConfig _config;
        public override void DoStart(){
            base.DoStart();
            _audioSvc = ( _audioService) as AudioService;
            _config = Resources.Load<AudioConfig>(_audioConfigPath);
            _config?.DoStart();
        }
        
        void OnEvent_OnAllPlayerFinishedLoad(object param){
            PlayMusicStart();
        }
        
        public void PlayClipDestroyGrass(){ _audioSvc.PlayClip(_config.destroyGrass); }
        public void PlayClipBorn(){ _audioSvc.PlayClip(_config.born); }
        public void PlayClipDied(){ _audioSvc.PlayClip(_config.died); }
        public void PlayClipHitTank(){ _audioSvc.PlayClip(_config.hitTank); }
        public void PlayClipHitIron(){ _audioSvc.PlayClip(_config.hitIron); }
        public void PlayClipHitBrick(){ _audioSvc.PlayClip(_config.hitBrick); }
        public void PlayClipDestroyIron(){ _audioSvc.PlayClip(_config.destroyIron); }
        public void PlayMusicBG(){ _audioSvc.PlayClip(_config.bgMusic); }
        public void PlayMusicStart(){ _audioSvc.PlayClip(_config.startMusic); }
        public void PlayMusicGetItem(){ _audioSvc.PlayClip(_config.addItem); }
        
    }
}