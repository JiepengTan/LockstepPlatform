using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    public partial class UnityGameEffectService : UnityBaseGameService, IGameEffectService {
        private GameConfig _config;
        private UnityEffectService _unityEffectMgr;

        public override void DoStart(){
            base.DoStart();
            _config = Resources.Load<GameConfig>(_gameConfigService.ConfigPath);
            _unityEffectMgr = _effectService as UnityEffectService;
        }

        public void ShowDiedEffect(LVector2 pos){
            _unityEffectMgr.CreateEffect(_config.DiedPrefab, pos);
        }

        public void ShowBornEffect(LVector2 pos){
            _unityEffectMgr.CreateEffect(_config.BornPrefab, pos);
        }
    }
}