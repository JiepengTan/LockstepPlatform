#define DEBUG_FRAME_DELAY
using System;
using System.Collections.Generic;
using NetMsg.Game;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Game {
    /// <summary>
    /// frame buffer
    /// </summary>
    public class FrameBuffer {
        /// for debug
        public static byte DebugMainActorID;

        /// 进行备份的帧间隔
        public const int SNAPSHORT_FRAME_INTERVAL = 2;

        /// 回滚需要的空间
        public const int ROLLBACK_NEED_SPACE = SNAPSHORT_FRAME_INTERVAL * 2;

        /// 客户度FrameBuffer Size 
        public const int BUFFER_SIZE = NetworkDefine.FRAME_RATE * 30 + MAX_CLIENT_PREDICT_FRAME_COUNT;

        /// 客户端最大可以超前的frame 数量
        public const int MAX_CLIENT_PREDICT_FRAME_COUNT =
            NetworkDefine.MAX_FRAME_DATA_DELAY / NetworkDefine.UPDATE_DELTATIME;

        /// 最大的可以超前的Frame数量
        public const int MAX_SERVER_OVERRIDE_FRAME_COUNT = BUFFER_SIZE - ROLLBACK_NEED_SPACE;

        public ServerFrame[] serverBuffer = new ServerFrame[BUFFER_SIZE];
        public ServerFrame[] clientBuffer = new ServerFrame[BUFFER_SIZE];

        /// 下一个需要验证的tick
        public int nextTickToCheck;

        /// 下一步需要执行的客户端tick
        public int nextClientTick;

        /// 当前服务器的Tick (从接收到的消息中分析出来的)
        public int curServerTick;

        /// 当前Buffer中最大的服务器Tick
        public int maxServerTickInBuffer = -1;


        public bool IsNeedRevert = false;
        private int firstMissFrameTick;

        public void SetClientTick(int tick){
            nextClientTick = tick + 1;
        }

        public void PushLocalFrame(ServerFrame frame){
            var sIdx = frame.tick % BUFFER_SIZE;
            Debug.Assert(clientBuffer[sIdx] == null || clientBuffer[sIdx].tick <= frame.tick,
                "Push local frame error!");
            clientBuffer[sIdx] = frame;
        }

        ///1.push server frames
        public void PushServerFrames(ServerFrame[] frames, bool isNeedDebugCheck = true){
            var count = frames.Length;
            for (int i = 0; i < count; i++) {
                var data = frames[i];

                if (data == null || data.tick < nextTickToCheck) {
                    //已经验证过的帧 直接抛弃
                    return;
                }

                if (data.tick > curServerTick) {
                    curServerTick = data.tick;
                }

                if (data.tick >= nextTickToCheck + MAX_SERVER_OVERRIDE_FRAME_COUNT - 1) {
                    //本地服务器落后太多 需要本地验证完成后再统一叫服务器下发
                    return;
                }

                if (data.tick > maxServerTickInBuffer) { //记录最大服务帧
                    maxServerTickInBuffer = data.tick;
                }

                var targetIdx = data.tick % BUFFER_SIZE;
                if (serverBuffer[targetIdx] == null || serverBuffer[targetIdx].tick != data.tick) {
                    serverBuffer[targetIdx] = data;
#if DEBUG_FRAME_DELAY
                    if (isNeedDebugCheck) {
                        foreach (var input in data.Inputs) {
                            if (input.Commands != null && input.Commands.Length > 0) {
                                //UnityEngine.Debug.Log($"self:{input.ActorId == Simulation.MainActorID} id{input.ActorId} RecvInput actorID:{input.ActorId}  cmd:{(ECmdType) (input.Commands[0].type)}");
                            }
                        }

                        var time = 0;
                        foreach (var input in data.Inputs) {
                            if (input.ActorId == DebugMainActorID) {
                                var delay = Time.realtimeSinceStartup - input.timeSinceStartUp;
                                if (delay > 0.2f && input.timeSinceStartUp > 1) {
                                    UnityEngine.Debug.Log(
                                        $"Tick {data.tick} input.Tick:{input.Tick} recv Delay {delay} rawTime{input.timeSinceStartUp}");
                                }
                            }
                        }
                    }
#endif
                }
            }
        }

        ///2.confirm frames=> (nextTickToCheck,hasMissedFrame,CanClientSimulate)
        public void UpdateFramesInfo(){
            //不考虑追帧
            //UnityEngine.Debug.Assert(nextTickToCheck <= nextClientTick, "localServerTick <= localClientTick ");
            //Confirm frames
            IsNeedRevert = false;
            while (nextTickToCheck <= maxServerTickInBuffer) {
                var sIdx = nextTickToCheck % BUFFER_SIZE;
                var cFrame = clientBuffer[sIdx];
                var sFrame = serverBuffer[sIdx];
                //服务器帧 或者客户端帧 还没到
                if (cFrame == null || cFrame.tick != nextTickToCheck || sFrame == null ||
                    sFrame.tick != nextTickToCheck)
                    break;
                //Check client guess input match the real input
                if (object.ReferenceEquals(sFrame, cFrame) || sFrame.Equals(cFrame)) {
                    nextTickToCheck++;
                }
                else {
                    IsNeedRevert = true;
                    break;
                }
            }
        }


        public int GetMissServerFrameTick(){
            UpdateMissServerFrameTick();
            return firstMissFrameTick;
        }

        private void UpdateMissServerFrameTick(){
            int tick = nextTickToCheck;
            for (; tick <= maxServerTickInBuffer; tick++) {
                var idx = tick % BUFFER_SIZE;
                if (serverBuffer[idx] == null || serverBuffer[idx].tick != tick) {
                    break;
                }
            }

            firstMissFrameTick = tick;
        }

        public bool CanExecuteNextFrame(){
            return (nextClientTick - firstMissFrameTick) < MAX_CLIENT_PREDICT_FRAME_COUNT;
        }

        public bool IsNeedReqMissFrame(){
            return (curServerTick > nextClientTick);
        }

        public int Ping = 50;

        public int GetTargetTick(){
            return curServerTick + (Ping * 2) / NetworkDefine.UPDATE_DELTATIME + 2;
        }

        public ServerFrame GetFrame(int tick){
            var sFrame = GetServerFrame(tick);
            if (sFrame != null) {
                return sFrame;
            }

            return GetLocalFrame(tick);
        }

        public ServerFrame GetServerFrame(int tick){
            if (tick > maxServerTickInBuffer) {
                return null;
            }

            var idx = tick % BUFFER_SIZE;
            var frame = serverBuffer[idx];
            if (frame == null) return null;
            if (frame.tick != tick) return null;
            return frame;
        }

        public ServerFrame GetLocalFrame(int tick){
            lock (this) {
                if (tick >= nextClientTick) {
                    return null;
                }

                var idx = tick % BUFFER_SIZE;
                return clientBuffer[idx];
            }
        }

        public int[] GetMissFrames(){ //check miss frame msg
            lock (this) {
                int missCount = 0;
                for (int tick = nextTickToCheck; tick < maxServerTickInBuffer; tick++) {
                    var idx = tick % BUFFER_SIZE;
                    if (serverBuffer[idx] == null) { //有空窗口
                        ++missCount;
                    }
                }

                if (missCount > 0) {
                    var missFrames = new int[missCount];
                    int missFrameIdx = 0;
                    for (int tick = nextTickToCheck; tick < maxServerTickInBuffer; tick++) {
                        var idx = tick % BUFFER_SIZE;
                        if (serverBuffer[idx] == null) { //有空窗口
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