using System.Collections.Generic;
using Lockstep.Math;
using UnityEngine;
namespace Lockstep.Game {
    public class WorldView : MonoBehaviour{
        public World world;

        private GameObject prefabPlayer;
        public Dictionary<byte,PlayerView> player2View = new Dictionary<byte, PlayerView>();
        public void Init(World world){
            world.OnAddPlayer += OnCreatePlayer;
            prefabPlayer = Resources.Load<GameObject>("Prefabs/Player");
        }

        public void OnCreatePlayer(BasePlayer player){
            var go = GameObject.Instantiate(prefabPlayer, player.Position.ToVector2(),
                Quaternion.Euler(0, 0f, player.Rotation),transform);
            var playerView = go.GetComponent<PlayerView>();
            playerView.Init(player);
            player2View.Add(player.Id,playerView);
        }
        
        public void DoStart(){ }
        public void DoDestroy(){ GameObject.Destroy(gameObject);}

        private void Update(){}

        public void DoUpdate(int deltaTime){
            
        }
    }
}