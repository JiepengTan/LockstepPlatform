#define DEBUG_FRAME_DELAY
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using NetMsg.Game.Tank;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Game {
    /// <summary>
    /// 滑动窗口 msg buffer
    /// </summary>
    public class CommandBuffer {
        public const int SERVER_FRAME_RATE = 60;
        public const int MAX_FRAME_BUFFER_COUNT = SERVER_FRAME_RATE;

        /// 进行备份的帧间隔
        public const int SNAPSHORT_FRAME_INTERVAL = 10;
        
        ///最大的可以超前的Frame数量
        public const int MAX_OVERRIDE_COUNT = MAX_FRAME_BUFFER_COUNT - SNAPSHORT_FRAME_INTERVAL;

        public ServerFrame[] serverFrames = new ServerFrame[MAX_FRAME_BUFFER_COUNT];
        public ServerFrame[] clientFrames = new ServerFrame[MAX_FRAME_BUFFER_COUNT];

        /// <summary>
        /// 下一个需要验证的tick
        /// </summary>
        public uint waitCheckTick; //本地服务器 tick 比真正的服务器慢，表示在这个tick 之前的所有的Frame 都是验证过的

        /// <summary>
        /// 下一步需要执行的客户端tick
        /// </summary>
        public uint nextClientTic; //本地客户端Tick 比服务器超前，很多输入是客户端的预判 需要验证 

        public uint maxServerTick; //当前最大接收到的frame tick


        /// 是否可以执行下一帧
        public bool CanExcuteNextFrame(){
            return (nextClientTic - waitCheckTick) < MAX_OVERRIDE_COUNT;
        }

        //is return true need revert to (waitCheckTick -1)
        public bool CheckHistoryCmds(){
            UnityEngine.Debug.Assert(waitCheckTick <= nextClientTic, "localServerTick <= localClientTick ");
            while (waitCheckTick <= maxServerTick) {
                var sIdx = waitCheckTick % MAX_FRAME_BUFFER_COUNT;
                var cFrame = clientFrames[sIdx];
                var sFrame = serverFrames[sIdx];
                if (sFrame == null || sFrame.tick < waitCheckTick) //服务器帧还没到
                    return false;
                
                UnityEngine.Debug.Assert(cFrame.tick == sFrame.tick && cFrame.tick == waitCheckTick,
                    $" Logic Error cs tick is diff s:{sFrame.tick} c:{cFrame.tick} checking:{waitCheckTick}");
                //Check client guess input match the real input
                if (sFrame.IsSame(cFrame)) {
                    //serverFrames[sIdx] = null;
                    //clientFrames[sIdx] = null;
                    waitCheckTick++;
                }
                else {
                    return true;
                }
            }

            return false;
        }

        public ServerFrame GetFrame(uint tick){
            var sFrame = GetServerFrame(tick);
            if (sFrame != null && sFrame.tick == tick) {
                return sFrame;
            }

            return GetLocalFrame(tick);
        }

        public void UpdateCheckedTick(){
            uint tick = waitCheckTick;
            for (; tick <= maxServerTick; tick++) {
                var idx = tick % MAX_FRAME_BUFFER_COUNT;
                if (serverFrames[idx] == null || serverFrames[idx].tick != tick) {
                    break;
                }
            }

            waitCheckTick = tick;
        }

        public void PushLocalFrame(ServerFrame frame){
            var tick = frame.tick;
            UnityEngine.Debug.Assert(tick == nextClientTic);
            UnityEngine.Debug.Assert(nextClientTic - waitCheckTick < MAX_OVERRIDE_COUNT, "ring out of range");
            var sIdx = nextClientTic % MAX_FRAME_BUFFER_COUNT;
            clientFrames[sIdx] = frame;
            nextClientTic++;
#if DEBUG_FRAME_DELAY
            var time = 0;
            foreach (var input in frame.inputs) {
                if (input.ActorId == Simulation.MainActorID) {
                    input.timeSinceStartUp = Time.realtimeSinceStartup;
                }
            }
#endif
        }

        public void PushServerFrames(ServerFrame[] frames){
            lock (this) {
                var count = frames.Length;
                for (int i = 0; i < count; i++) {
                    var data = frames[i];

                    if (data == null || data.tick < waitCheckTick) {
                        //已经验证过的帧 直接抛弃
                        return;
                    }

                    if (data.tick >= waitCheckTick + MAX_OVERRIDE_COUNT - 1) {
                        //本地服务器落后太多  需要本地验证完成后再统一叫服务器下发
                        return;
                    }

                    if (data.tick > maxServerTick) { //记录最大服务帧
                        maxServerTick = data.tick;
                    }

                    var targetIdx = data.tick % MAX_FRAME_BUFFER_COUNT;
                    if (serverFrames[targetIdx] == null) {
                        serverFrames[targetIdx] = data;
#if DEBUG_FRAME_DELAY
                        var time = 0;
                        foreach (var input in data.inputs) {
                            if (input.ActorId == Simulation.MainActorID) {
                                var delay = Time.realtimeSinceStartup - input.timeSinceStartUp;
                                if (delay > 0.2f) {
                                    UnityEngine.Debug.Log($"Tick {data.tick} recv Delay {delay}");
                                }
                            }
                        }
#endif
                    }
                }
            }
        }

        public ServerFrame GetServerFrame(uint tick){
            lock (this) {
                if (tick < waitCheckTick || tick > maxServerTick) {
                    return null;
                }

                var idx = tick % MAX_FRAME_BUFFER_COUNT;
                return serverFrames[idx];
            }
        }

        public ServerFrame GetLocalFrame(uint tick){
            lock (this) {
                if (tick < waitCheckTick || tick >= nextClientTic) {
                    return null;
                }

                var idx = tick % MAX_FRAME_BUFFER_COUNT;
                return clientFrames[idx];
            }
        }

        public uint[] GetMissFrames(){ //check miss frame msg
            lock (this) {
                int missCount = 0;
                for (uint tick = waitCheckTick; tick < maxServerTick; tick++) {
                    var idx = tick % MAX_FRAME_BUFFER_COUNT;
                    if (serverFrames[idx] == null) { //有空窗口
                        ++missCount;
                    }
                }

                if (missCount > 0) {
                    var missFrames = new uint[missCount];
                    int missFrameIdx = 0;
                    for (uint tick = waitCheckTick; tick < maxServerTick; tick++) {
                        var idx = tick % MAX_FRAME_BUFFER_COUNT;
                        if (serverFrames[idx] == null) { //有空窗口
                            missFrames[missFrameIdx++] = tick;
                        }
                    }

                    return missFrames;
                }

                return null;
            }
        }
    }
}