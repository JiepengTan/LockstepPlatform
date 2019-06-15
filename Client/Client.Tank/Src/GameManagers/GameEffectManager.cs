using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    public partial class GameEffectManager : BaseGameManager, IGameEffectService {
        private GameConfig _config;
        private EffectManager _effectMgr;

        public override void DoStart(){
            base.DoStart();
            _config = Resources.Load<GameConfig>(ConstGameConfig.ConfigPath);
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