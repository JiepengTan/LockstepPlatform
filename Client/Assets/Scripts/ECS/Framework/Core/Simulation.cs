using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lockstep.Core;
using Lockstep.Core.Logic;
using Lockstep.Logging;
using Lockstep.Game.Features;
using Lockstep.Serialization;
using NetMsg.Game.Tank;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Lockstep.Game {
    public class Simulation {
        public static byte MainActorID;
        public World World => _world;
        public Contexts _context { get; }

        public GameLog GameLog { get; } = new GameLog();

        public byte _localActorId { get; private set; }

        public bool Running { get; private set; }

        public ServiceContainer Services { get; }

        private float _tickDt;
        private float _accumulatedTime;

        private World _world;

        private CommandBuffer cmdBuffer = new CommandBuffer();

        public void OnNetFrame(ServerFrame[] frames){ }

        public uint _localTick;
        public int _roomId;
        private NetMgr _netMgr;

        public Simulation(){ }

        public void OnEvent_OnServerFrames(object param){
            var msg = param as Msg_ServerFrames;
            cmdBuffer.PushServerFrames(msg.frames);
        }

        public void OnEvent_OnRoomGameStart(object param){
            var msg = param as Msg_StartGame;
            StartGame(msg.RoomID, msg.SimulationSpeed, msg.ActorID, msg.AllActors);
        }


        public Simulation(Contexts context, NetMgr netMgr, params IService[] services){
            EventHelper.AddListener(EEvent.OnServerFrame, OnEvent_OnServerFrames);
            EventHelper.AddListener(EEvent.OnRoomGameStart, OnEvent_OnRoomGameStart);
            _netMgr = netMgr;
            _context = context;
            Services = new ServiceContainer();
            foreach (var service in services) {
                Services.Register(service);
            }
        }

        public void StartGame(int roomId, int targetFps, byte localActorId, byte[] allActors, bool isNeedRender = true){
            MainActorID = localActorId;
            _localActorId = localActorId;
            _allActors = allActors;

            _localTick = 0;
            _roomId = roomId;
            GameLog.LocalActorId = localActorId;
            GameLog.AllActorIds = allActors;

            _actorCount = allActors.Length;
            _tickDt = 1000f / targetFps;
            _world = new World(_context, allActors,
                new InputFeature(_context, Services),
                new CleanupFeature(_context, Services));

            Running = true;
        }

        public void DoDestroy(){
            Running = false;
        }

        public byte[] _allActors;
        private int _actorCount;

        private uint CurTick = 0;

        public void DoUpdate(float elapsedMilliseconds){
            if (!Running) {
                return;
            }

            if (!cmdBuffer.CanExcuteNextFrame()) { //因为网络问题 需要等待服务器发送确认包 才能继续往前
                return;
            }

            _accumulatedTime += elapsedMilliseconds;
            while (_accumulatedTime >= _tickDt) {
                var tick = _world.Tick;
                var input = new Msg_PlayerInput(tick, _localActorId, InputHelper.GetInputCmds());
                var localFrame = new ServerFrame();
                localFrame.tick = tick;
                var inputs = new Msg_PlayerInput[_actorCount];
                //将所有角色 给予默认的输入
                for (int i = 0; i < _actorCount; i++) {
                    inputs[i] = new Msg_PlayerInput(tick, _allActors[i], null);
                }

                inputs[_localActorId] = input;
                localFrame.inputs = inputs;
                cmdBuffer.PushLocalFrame(localFrame);
                _netMgr.SendInput(input);

                //校验服务器包  如果有预测失败 则需要进行回滚
                var isNeedRevert = cmdBuffer.CheckHistoryCmds();
                if (isNeedRevert) {
                    //UnityEngine.Debug.Log($" Need revert from curTick {_world.Tick} to {cmdBuffer.waitCheckTick}");
                    var curTick = _world.Tick;
                    var revertTargetTick = System.Math.Max(cmdBuffer.waitCheckTick - 1, 0u);
                    _world.RevertToTick(revertTargetTick);
                    CheckRevertHashCode();
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
                        ProcessInputQueue(frame);
                        _world.Predict();
                        SetHashCode();
                    }
                }

                {
                    var frame = localFrame;
                    if (_world.Tick <= frame.tick) {
                        ProcessInputQueue(frame);
                        _world.Predict();
                        SetHashCode();
                    }
                }
                _accumulatedTime -= _tickDt;
            }

            CheckAndSendHashCodes();
        }


        public List<long> allBeforeExecuteHashCodes = new List<long>();
        public List<long> allHashCodes = new List<long>();
        public static int[,] allAccumInputCount = new int[2, 10000];
        private List<long> allHashCodess = new List<long>();
        private uint firstHashTick = 0;

        public void CheckAndSendHashCodes(){
            if (cmdBuffer.waitCheckTick > firstHashTick) {
                var count = System.Math.Min(allHashCodes.Count, (int) (cmdBuffer.waitCheckTick - firstHashTick));
                if (count > 0) {
                    Msg_HashCode msg = new Msg_HashCode();
                    msg.startTick = firstHashTick;
                    msg.hashCodes = new long[count];
                    for (int i = 0; i < count; i++) {
                        msg.hashCodes[i] = allHashCodess[i];
                    }

                    _netMgr.SendMsgRoom(EMsgCS.C2S_HashCode, msg);
                    firstHashTick = firstHashTick + (uint) count;
                    allHashCodess.RemoveRange(0, count);
                }
            }
        }

        public void SetHash(uint tick, long hash){
            if (tick < firstHashTick) {
                return;
            }

            var idx = (int) (tick - firstHashTick);
            if (allHashCodess.Count <= idx) {
                for (int i = 0; i < idx + 1; i++) {
                    allHashCodess.Add(0);
                }
            }

            allHashCodess[idx] = hash;
        }

        public void SetHashCode(){
            var nextTick = _world.Tick;
            var iTick = (int) nextTick - 1;
            for (int i = allHashCodes.Count; i <= iTick; i++) {
                allHashCodes.Add(0);
                allBeforeExecuteHashCodes.Add(0);
            }

            var hash = _world.Contexts.gameState.hashCode.value;
            allHashCodes[iTick] = _world.Contexts.gameState.hashCode.value;
            allBeforeExecuteHashCodes[iTick] = _world.Contexts.gameState.beforeExecuteHashCode.value;
            SetHash(nextTick - 1, hash);
        }

        public void CheckRevertHashCode(){
            var iTick = (int) _world.Tick;
            var hashCodeAfterRevert = _world.Contexts.gameState.beforeExecuteHashCode.value;
            var hashCodeBefore = allBeforeExecuteHashCodes[iTick];
            if (hashCodeAfterRevert != hashCodeBefore) {
                Debug.LogError($"Revert Error: 前后状态不一致 before{hashCodeBefore} afterRevert{hashCodeAfterRevert}");
            }
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

        public static int[] ExcutedMsg_PlayerInputCount = new int[2];

        private void ProcessInputQueue(ServerFrame frame){
            var inputs = frame.inputs;
            foreach (var input in inputs) {
                GameLog.Add(frame.tick, input);

                foreach (var command in input.Commands) {
                    Log.Trace(this, input.ActorId + " >> " + input.Tick + ": " + input.Commands.Count());
                    ExcutedMsg_PlayerInputCount[input.ActorId]++;
                    var inputEntity = _context.input.CreateEntity();
                    InputHelper.Execute(command, inputEntity);
                    inputEntity.AddTick(input.Tick);
                    inputEntity.AddActorId(input.ActorId);
                    inputEntity.isDestroyed = true;
                }
            }
        }
    }
}