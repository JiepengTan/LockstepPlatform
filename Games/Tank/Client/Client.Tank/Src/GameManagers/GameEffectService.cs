using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    public partial class GameEffectService : UnityBaseGameService, IGameEffectService {
        private GameConfig _config;
        private EffectService _effectMgr;

        public override void DoStart(){
            base.DoStart();
            _config = Resources.Load<GameConfig>(_gameConfigService.ConfigPath);
            _effectMgr = _effectService as EffectService;
        }

        public void ShowDiedEffect(LVector2 pos){
            _effectMgr.CreateEffect(_config.DiedPrefab, pos);
        }

        public void ShowBornEffect(LVector2 pos){
            _effectMgr.CreateEffect(_config.BornPrefab, pos);
        }
    }
}