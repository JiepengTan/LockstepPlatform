using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    public partial class GameEffectManager : SingletonManager<GameEffectManager>, IGameEffectService {
        private GameConfig _config;
        private EffectManager _effectMgr;

        public override void DoStart(){
            base.DoStart();
            _config = Resources.Load<GameConfig>(GameConfig.ConfigPath);
            _effectMgr = _effectService as EffectManager;
        }

        public void ShowDiedEffect(LVector2 pos){
            _effectMgr.CreateEffect(_config.DiedPrefab, pos);
        }

        public void ShowBornEffect(LVector2 pos){
            _effectMgr.CreateEffect(_config.BornPrefab, pos);
        }
    }
}