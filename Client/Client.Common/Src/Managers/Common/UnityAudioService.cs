using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Lockstep.Game {
    [System.Serializable]
    public class UnityAudioService : UnityBaseService, IAudioService {
        private AudioSource _source;
        private Dictionary<AudioClip, int> _curFramePlayeredCount = new Dictionary<AudioClip, int>();

        public override void Backup(int tick){
            _curFramePlayeredCount.Clear();
        }

        public override void DoStart(){
            _source = gameObject.GetComponent<AudioSource>();
            if (_source == null) {
                _source = gameObject.AddComponent<AudioSource>();
            }
        }

        public void PlayClip(string clip){
            //TODO
            //var audio = _config.GetAudio(clip);
            //PlayClip(audio);
        }

        public void PlayClip(AudioClip clip){
            if (clip != null) {
                //追帧 不播放音效
                if (_constStateService.IsPursueFrame) {
                    return;
                }

                //回放 不播放音效
                if (_constStateService.IsVideoMode && !_constStateService.IsRunVideo) {
                    return;
                }

                if (_curFramePlayeredCount.TryGetValue(clip, out int val)) {
                    _curFramePlayeredCount[clip] = val + 1;
                }
                else {
                    _curFramePlayeredCount.Add(clip, 1);
                }

                if (_curFramePlayeredCount[clip] >= 2) return; //不播放大于2个的音效 听不出来
                _source.PlayOneShot(clip);
            }
        }
    }
}