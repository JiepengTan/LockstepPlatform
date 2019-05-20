namespace Lockstep.Game {
    public interface IAudioService :IService {
        void PlayClipDestroyGrass();
        void PlayClipBorn();
        void PlayClipDied();
        void PlayClipHitTank();
        void PlayClipHitIron();
        void PlayClipHitBrick();
        void PlayClipDestroyIron();
        void PlayMusicBG();
        void PlayMusicStart();
        void PlayMusicGetItem();   
    }
}