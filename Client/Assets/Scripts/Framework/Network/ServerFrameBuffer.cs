#define DEBUG_FRAME_DELAY
using NetMsg.Game.Tank;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Game {
    /// <summary>
    /// 滑动窗口 msg buffer
    /// </summary>
    public class ServerFrameBuffer {
        public const int SERVER_FRAME_RATE = 20;
        public const int MAX_FRAME_BUFFER_COUNT = SERVER_FRAME_RATE * 3;
        public ServerFrame[] frames = new ServerFrame[MAX_FRAME_BUFFER_COUNT];
        public int startIdx; //locakTick +1 存放的位置
        public uint startTick;
        public uint maxServerTick; //当前最大接收到的frame tick
        public int missFrameIdx;

        public ServerFrame Dequeue(uint tick){
            lock (this) {
                if (tick != startTick) {
                    return null;
                }
                var frame = frames[startIdx];
                if (frame != null) {
                    frames[startIdx] = null;
                    ++startIdx;
                    ++startTick;
                    if (startIdx == MAX_FRAME_BUFFER_COUNT) startIdx = 0;
                    return frame;
                }
                return null;
            }
        }

        public void EnqueueFrame(ServerFrame data){
            lock (this) {
                if (data == null || data.tick < startTick) {
                    //过期的tick
                    return;
                }

                if (data.tick >= startTick + MAX_FRAME_BUFFER_COUNT) {
                    //本地落后服务器太多
                    return;
                }

                if (data.tick > maxServerTick) {
                    maxServerTick = data.tick;
                }

                var targetIdx = GetSlotIdx(data.tick);
                if (frames[targetIdx] == null) {
                    frames[targetIdx] = data;
#if DEBUG_FRAME_DELAY
                    var time = 0;
                    foreach (var input in data.inputs) {
                        if (input.ActorId == Simulation.MainActorID) {
                            Debug.Log($"Tick {data.tick} recv Delay {Time.realtimeSinceStartup - input.timeSinceStartUp}");
                        }
                    }
#endif
                }
            }
        }

        public uint[] GetMissFrames(){ //check miss frame msg
            lock (this) {
                int missCount = 0;
                for (uint i = startTick; i <= maxServerTick; i++) {
                    var idx = GetSlotIdx(i);
                    if (frames[idx] == null) { //有空窗口
                        ++missCount;
                    }
                }

                if (missCount > 0) {
                    var missFrames = new uint[missCount];
                    int missFrameIdx = 0;
                    for (uint i = startTick; i <= maxServerTick; i++) {
                        var idx = GetSlotIdx(i);
                        if (frames[idx] == null) { //有空窗口
                            missFrames[missFrameIdx++] = i;
                        }
                    }
                    return missFrames;
                }
                return null;
            }
        }

        int GetSlotIdx(uint frameIdx){
            return (startIdx + (int) (frameIdx - startTick)) % MAX_FRAME_BUFFER_COUNT;
        }
    }
}