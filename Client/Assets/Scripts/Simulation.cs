using Lockstep.Math;
using NetMsg.Game.Tank;
using UnityEngine;

namespace Lockstep.Game {
    public class Simulation {
        public static float curFrameExcuteTimeStamp;
        public static float lastFrameExcuteTimeStamp;
        public static byte MainActorID;
        public static LFloat FrameIntervalMs;

        public ServerFrameBuffer _buffer = new ServerFrameBuffer();

        private float _accumTme;
        private World _world;
        private WorldView _worldRender;
        public byte LocalActorId { get; private set; }
        public bool Running { get; private set; }
        public uint localTick;
        private NetHelper _network;

        public Simulation(NetHelper network){
            this._network = network;
        }

        public void OnNetFrame(ServerFrame[] frames){
            foreach (var frame in frames) {
                _buffer.EnqueueFrame(frame);
            }

            CheckMissFrames();
        }

        public void CheckMissFrames(){
            uint[] missFrames = null;
            missFrames = _buffer.GetMissFrames();
            if (missFrames != null) {
                _network.SendPackRequire(missFrames);
            }
        }

        private int roomId;

        public void StartGame(int roomId,int targetFps, byte localActorId, byte[] allActors, bool isNeedRender = true){
            LocalActorId = localActorId;
            localTick = 0;
            FrameIntervalMs = new LFloat(true,(LFloat.Precision / 1000  * (1000 / targetFps)))  ;
            MainActorID = localActorId;
            this.roomId = roomId;
            _world = new World(allActors, true);
            if (isNeedRender) {
                _worldRender = new GameObject("RenderWorld").AddComponent<WorldView>();
                _worldRender.Init(_world);
            }

            this.Running = true;
            this._world.DoStart();
            _worldRender?.DoStart();
        }

        public void Update(float elapsedMs){
            if (!this.Running)
                return;
            while (localTick < _buffer.maxServerTick) {
                var frame = _buffer.Dequeue(localTick);
                if (frame == null)
                    return;
                _world.DoUpdate(FrameIntervalMs, frame);
                localTick++;
                lastFrameExcuteTimeStamp = curFrameExcuteTimeStamp;
                curFrameExcuteTimeStamp = Time.timeSinceLevelLoad;
            }
        }

        public void DoDestroy(){
            _world.DoDestroy();
            _worldRender?.DoDestroy();
        }
    }
}