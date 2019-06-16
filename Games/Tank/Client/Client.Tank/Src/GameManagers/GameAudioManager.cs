using UnityEngine;

namespace Lockstep.Game {
    
    [System.Serializable]
    public class GameAudioManager : BaseGameManager,IGameAudioService {
        private AudioManager _audioManager;
        private static string _audioConfigPath = "AudioConfig";
        private AudioConfig _config;
        public override void DoStart(){
            base.DoStart();
            _audioManager = _audioService as AudioManager;
            _config = Resources.Load<AudioConfig>(_audioConfigPath);
            _config?.DoStart();
        }
        
        void OnEvent_OnAllPlayerFinishedLoad(object param){
            PlayMusicStart();
        }
        
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
        
    }
}