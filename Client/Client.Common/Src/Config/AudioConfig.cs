using System;
using Entitas;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Lockstep.Game {
   
    [CreateAssetMenu]
    public partial class AudioConfig : UnityEngine.ScriptableObject {
        public AudioClip born; 
        public AudioClip died;
        public AudioClip hitTank;
        public AudioClip hitBrick;
        public AudioClip hitIron;
        public AudioClip destroyIron;
        public AudioClip destroyGrass;
        public AudioClip addItem;
        public AudioClip bgMusic;
        public AudioClip startMusic;

        public void DoStart(){
            
        }

        public AudioClip GetAudio(string relPath){
            return null;
        }
    }
}