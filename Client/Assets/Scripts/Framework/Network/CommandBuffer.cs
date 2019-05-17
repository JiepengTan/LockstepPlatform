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
        public const int SNAPSHORT_FRAME_INTERVAL = 2;

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
            var isOverFrame = (nextClientTic - waitCheckTick) >= MAX_OVERRIDE_COUNT;
            var isServerFrameUpdate = IsServerFrameFlush();
            return !isOverFrame || isServerFrameUpdate;
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

                UnityEngine.Debug.Assert(cFrame!=null && cFrame.tick == sFrame.tick && cFrame.tick == waitCheckTick,
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

        public uint GetMissServerFrameTick(){
            uint tick = waitCheckTick;
            for (; tick <= maxServerTick; tick++) {
                var idx = tick % MAX_FRAME_BUFFER_COUNT;
                if (serverFrames[idx] == null || serverFrames[idx].tick != tick) {
                    break;
                }
            }
            waitCheckTick = tick;
            return tick;
        }

        public ServerFrame GetFrame(uint tick){
            var sFrame = GetServerFrame(tick);
            if (sFrame != null && sFrame.tick == tick) {
                return sFrame;
            }

            return GetLocalFrame(tick);
        }

        public bool IsServerFrameFlush(){
            var idx = waitCheckTick % MAX_FRAME_BUFFER_COUNT;
            return serverFrames[idx] != null && serverFrames[idx].tick == waitCheckTick;
        }


        public void PushLocalFrame(ServerFrame frame){
            var tick = frame.tick;
            if (tick != nextClientTic) {
                UnityEngine.Debug.LogError($"PushLocalFrame error tick: {tick} :nextClientTic:{nextClientTic}");
            }

            UnityEngine.Debug.Assert(tick == nextClientTic);
            UnityEngine.Debug.Assert(((int)nextClientTic - (int)waitCheckTick) < MAX_OVERRIDE_COUNT, $"ring out of range cTick:{nextClientTic}  waitCheck:{waitCheckTick} ");
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

        public static int[] Msg_PlayerInputCount = new int[2];
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
                    if (serverFrames[targetIdx] == null || serverFrames[targetIdx].tick != data.tick) {
                        serverFrames[targetIdx] = data;
                        foreach (var input in data.inputs) {
                            if (input.Commands.Length > 0) {
                                //UnityEngine.Debug.Log($"self:{input.ActorId == Simulation.MainActorID} id{input.ActorId} RecvInput actorID:{input.ActorId}  cmd:{(ECmdType) (input.Commands[0].type)}");
                                Msg_PlayerInputCount[input.ActorId]++;
                            }

                            Simulation.allAccumInputCount[input.ActorId, input.Tick] = Msg_PlayerInputCount[input.ActorId];
                        }
#if DEBUG_FRAME_DELAY
                        var time = 0;
                        foreach (var input in data.inputs) {
                            if (input.ActorId == Simulation.MainActorID) {
                                var delay = Time.realtimeSinceStartup - input.timeSinceStartUp;
                                if (delay > 0.2f) {
                                    UnityEngine.Debug.Log($"Tick {data.tick} input.Tick:{input.Tick} recv Delay {delay} rawTime{input.timeSinceStartUp}");
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
                if (tick > maxServerTick) {
                    return null;
                }

                var idx = tick % MAX_FRAME_BUFFER_COUNT;
                return serverFrames[idx];
            }
        }

        public ServerFrame GetLocalFrame(uint tick){
            lock (this) {
                if (tick >= nextClientTic) {
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