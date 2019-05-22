using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lockstep.Core;
using Lockstep.Core.Logic;
using Lockstep.Logging;
using Lockstep.Game.Features;
using Lockstep.Serialization;
using NetMsg.Game;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Lockstep.Game {
    public class SimulationManager : SingletonManager<SimulationManager> {
        public World World => _world;
        private Contexts _context;
        private GameLog GameLog = new GameLog();
        private byte _localActorId;
        private bool Running;
        private IServiceContainer _services;
        private float _tickDt;
        private float _accumulatedTime;
        private World _world;
        private FrameBuffer cmdBuffer = new FrameBuffer();
        private uint _localTick;
        private int _roomId;
        private List<long> allHashCodes = new List<long>();
        private uint firstHashTick = 0;
        
        private byte[] _allActors;
        private int _actorCount;
        private uint CurTick = 0;

        void OnEvent_OnServerFrame(object param){
            var msg = param as Msg_ServerFrames;
            cmdBuffer.PushServerFrames(msg.frames);
        }

        void OnEvent_OnRoomGameStart(object param){
            var msg = param as Msg_StartGame;
            OnGameStart(msg.RoomID, msg.SimulationSpeed, msg.ActorID, msg.AllActors);
            EventHelper.Trigger(EEvent.OnSimulationInited,null);
        }

        void OnEvent_OnAllPlayerFinishedLoad(object param){
            Debug.Log($"OnEvent_OnAllPlayerFinishedLoad");
            Running = true;
            _world.StartSimulate();
            EventHelper.Trigger(EEvent.OnSimulationStart,null);
        }

        public void OnEvent_AllPlayerFinishedLoad(object param){
            OnLoadFinished();
        }

        public override void DoAwake(IServiceContainer services){
            _services = services;
            _context = Main.Instance.contexts;
            _inputService = _services.GetService<IInputService>();
        }


        public override void DoUpdate(float deltaTime){
            if (!Running) {
                return;
            }
            if (!cmdBuffer.CanExcuteNextFrame()) { //因为网络问题 需要等待服务器发送确认包 才能继续往前
                return;
            }

            _accumulatedTime += deltaTime * 1000;
            while (_accumulatedTime >= _tickDt) {
                var tick = _world.Tick;
                var input = new Msg_PlayerInput(tick, _localActorId, _inputService.GetInputCmds());
                var localFrame = new ServerFrame();
                localFrame.tick = tick;
                var inputs = new Msg_PlayerInput[_actorCount];
                inputs[_localActorId] = input;
                localFrame.inputs = inputs;
                FillInputWithLastFrame(localFrame);
                cmdBuffer.PushLocalFrame(localFrame);
                _networkMgr.SendInput(input);

                //校验服务器包  如果有预测失败 则需要进行回滚
                var isNeedRevert = cmdBuffer.CheckHistoryCmds();
                if (isNeedRevert) {
                    UnityEngine.Debug.Log($" Need revert from curTick {_world.Tick} to {cmdBuffer.waitCheckTick}");
                    var curTick = _world.Tick;
                    var revertTargetTick = (cmdBuffer.waitCheckTick <= 1 ? 0u : cmdBuffer.waitCheckTick);
                    _world.RollbackTo(revertTargetTick);
                    //  _world.Tick -> nextMissServerFrame simulation
                    var waitCheckTick = cmdBuffer.GetMissServerFrameTick(); //服务器 可能超前
                    //Debug.Assert(nextMissServerFrame <= curTick,$"curTick {curTick} nextMissServerFrame{nextMissServerFrame}");
                    var snapTick = _world.Tick;
                    while (_world.Tick < waitCheckTick) {
                        var frame = cmdBuffer.GetServerFrame(_world.Tick);
                        if (!(frame != null && frame.tick == _world.Tick)) {
                            Debug.LogError("cmdBuffer Mgr error");
                        }

                        //服务器超前 客户端 应该追上去 将服务器中的输入作为客户端输入
                        if (_world.Tick > curTick) {
                            cmdBuffer.PushLocalFrame(frame);
                        }

                        //UnityEngine.Debug.Assert(frame != null && frame.tick == _world.Tick, "cmdBuffer Mgr error");
                        ProcessInputQueue(frame);
                        _world.Simulate(_world.Tick != snapTick);
                        SetHashCode();
                    }

                    // cmdBuffer.waitCheckTick -> lastTick Predict
                    while (_world.Tick < curTick) {
                        var frame = cmdBuffer.GetLocalFrame(_world.Tick);
                        if (!(frame != null && frame.tick == _world.Tick)) {
                            Debug.LogError("cmdBuffer Mgr error");
                        }

                        //UnityEngine.Debug.Assert(frame != null && frame.tick == _world.Tick, "cmdBuffer Mgr error");
                        FillInputWithLastFrame(frame);
                        ProcessInputQueue(frame);
                        _world.Predict();
                        SetHashCode();
                    }
                }

                {
                    var frame = localFrame;
                    if (_world.Tick <= frame.tick) {
                        FillInputWithLastFrame(frame);
                        ProcessInputQueue(frame);
                        _world.Predict();
                        SetHashCode();
                    }
                }

                _accumulatedTime -= _tickDt;
            }

            //清理无用 snapshot
            _world.CleanUselessSnapshot((cmdBuffer.waitCheckTick <= 1 ? 0u : cmdBuffer.waitCheckTick));
            CheckAndSendHashCodes();
        }

        public override void DoDestroy(){
            Running = false;
        }

        public void OnGameStart(int roomId, int targetFps, byte localActorId, byte[] allActors,
            bool isNeedRender = true){
            Debug.Log($"hehe OnGameStart simulation");
            FrameBuffer.DebugMainActorID = localActorId;
            //初始化全局配置
            _globalStateService.roomId = roomId;
            _globalStateService.allActorIds = allActors;
            _globalStateService.actorCount = allActors.Length;

            _localActorId = localActorId;
            _allActors = allActors;

            _localTick = 0;
            _roomId = roomId;
            GameLog.LocalActorId = localActorId;
            GameLog.AllActorIds = allActors;

            _actorCount = allActors.Length;
            _tickDt = 1000f / targetFps;
            _world = new World(_context,_timeMachineService, allActors, new GameLogicSystems(_context, _services));
            
        }


        public void OnLoadFinished(){
            _world.StartSimulate();
            Running = true;
        }



        private void FillInputWithLastFrame(ServerFrame frame){
            uint tick = frame.tick;
            var inputs = frame.inputs;
            var lastFrameInputs = tick == 0 ? null : cmdBuffer.GetFrame(tick - 1)?.inputs;
            var curFrameInput = inputs[_localActorId];
            //将所有角色 给予默认的输入
            for (int i = 0; i < _actorCount; i++) {
                inputs[i] = new Msg_PlayerInput(tick, _allActors[i], lastFrameInputs?[i]?.Commands?.ToList());
            }

            inputs[_localActorId] = curFrameInput;
        }


        public void CheckAndSendHashCodes(){
            if (cmdBuffer.waitCheckTick > firstHashTick) {
                var count = System.Math.Min(allHashCodes.Count, (int) (cmdBuffer.waitCheckTick - firstHashTick));
                if (count > 0) {
                    Msg_HashCode msg = new Msg_HashCode();
                    msg.startTick = firstHashTick;
                    msg.hashCodes = new long[count];
                    for (int i = 0; i < count; i++) {
                        msg.hashCodes[i] = allHashCodes[i];
                    }

                    _networkMgr.SendMsgRoom(EMsgCS.C2S_HashCode, msg);
                    firstHashTick = firstHashTick + (uint) count;
                    allHashCodes.RemoveRange(0, count);
                }
            }
        }

        public void SetHash(uint tick, long hash){
            if (tick < firstHashTick) {
                return;
            }

            var idx = (int) (tick - firstHashTick);
            if (allHashCodes.Count <= idx) {
                for (int i = 0; i < idx + 1; i++) {
                    allHashCodes.Add(0);
                }
            }

            allHashCodes[idx] = hash;
        }

        public void SetHashCode(){
            var nextTick = _world.Tick;
            var iTick = (int) nextTick - 1;
            for (int i = allHashCodes.Count; i <= iTick; i++) {
                allHashCodes.Add(0);
            }

            var hash = _world.Contexts.gameState.hashCode.value;
            allHashCodes[iTick] = _world.Contexts.gameState.hashCode.value;
            SetHash(nextTick - 1, hash);
        }


        public void DumpGameLog(Stream outputStream, bool closeStream = true){
            var serializer = new Serializer();
            serializer.Put(_context.gameState.hashCode.value);
            serializer.Put(_context.gameState.tick.value);
            outputStream.Write(serializer.Data, 0, serializer.Length);

            GameLog.WriteTo(outputStream);

            if (closeStream) {
                outputStream.Close();
            }
        }

        private void ProcessInputQueue(ServerFrame frame){
            var inputs = frame.inputs;
            foreach (var input in inputs) {
                GameLog.Add(frame.tick, input);

                foreach (var command in input.Commands) {
                    Log.Trace(this, input.ActorId + " >> " + input.Tick + ": " + input.Commands.Count());
                    var inputEntity = _context.input.CreateEntity();
                    _inputService.Execute(command, inputEntity);
                    inputEntity.AddTick(input.Tick);
                    inputEntity.AddActorId(input.ActorId);
                    inputEntity.isDestroyed = true;
                }
            }
        }
    }
}