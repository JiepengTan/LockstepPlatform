#define DEBUG_FRAME_DELAY
using System;
using Lockstep.Math;
using NetMsg.Game.Tank;
using Debug  = Lockstep.Serialization;

namespace Lockstep.Game {
    [System.Serializable]
    public class BasePlayer {
        public Action<BasePlayer> OnSpawn;
        public readonly string Name;

        private LFloat _speed;

        //上一帧的数据
        protected LVector2 _lastFramePos;
        protected int _lastFrameDeg;

        protected LVector2 _curFramePos;
        protected int _curFrameDeg;

        public LVector2 Position {
            get { return _curFramePos; }
            set { _curFramePos = value; }
        }

        public int Rotation {
            get { return _curFrameDeg; }
            set { _curFrameDeg = value; }
        }

        public LVector2 LastFramePosition => _curFramePos;
        public int LastFrameRotation => _curFrameDeg;
        public readonly byte Id;

        public int Ping;

        public BasePlayer(byte id, string name){
            Id = id;
            Name = name;
        }

        public virtual void Spawn(LVector2 position){
            _lastFramePos = _curFramePos = position;
            _curFrameDeg = _curFrameDeg = 0;
            if (OnSpawn != null) {
                OnSpawn(this);
            }
        }


        public virtual void UpdatePosDeg(LVector2 position, int deg){
            _lastFramePos = _curFramePos;
            _lastFrameDeg = _curFrameDeg;
            _curFramePos = position;
            _curFrameDeg = deg;
        }

    }
}