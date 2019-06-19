using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
    public partial class UnityGameEffectService : UnityBaseGameService, IGameEffectService {

        public void ShowDiedEffect(LVector2 pos){
            _effectService.CreateEffect(_gameConfigService.DiedPrefabAssetId, pos);
        }

        public void ShowBornEffect(LVector2 pos){
            _effectService.CreateEffect(_gameConfigService.BornPrefabAssetId, pos);
        }
    }
}