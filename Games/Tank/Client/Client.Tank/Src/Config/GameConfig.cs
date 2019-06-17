using System;
using Lockstep.ECS.Game;
using UnityEngine;

namespace Lockstep.Game {
    [CreateAssetMenu]
    public partial class GameConfig : UnityEngine.ScriptableObject {

        public GameObject BornPrefab;
        public GameObject DiedPrefab;
        
        public GameConfigService config = new GameConfigService();
        public void Read(string path){
            config.Read(path);
        }

        public void Write(string path){
            config.Write(path);
        }
    }
}