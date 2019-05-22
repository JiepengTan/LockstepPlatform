using System.Collections;
using System.Collections.Generic;
using Lockstep.Math;
using UnityEngine;

namespace Lockstep.Game {
 

    public partial class ResourceManager :IResourceService{
        private GameConfig _config;
        public override void DoStart(){
            base.DoStart();
            _config = Resources.Load<GameConfig>(GameConfig.ConfigPath);
        }
        
        public void ShowDiedEffect(LVector2 pos){
            GameObject.Instantiate(_config.DiedPrefab, transform.position + pos.ToVector3(), Quaternion.identity);
        }
        public void ShowBornEffect(LVector2 pos){
            GameObject.Instantiate(_config.BornPrefab, transform.position + pos.ToVector3(), Quaternion.identity);
        }
    }
}