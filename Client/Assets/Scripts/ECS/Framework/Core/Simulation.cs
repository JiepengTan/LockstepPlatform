using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Lockstep.Core;
using Lockstep.Core.Logic;
using Lockstep.Core.Logic.Interfaces;
using Lockstep.Logging;
using Lockstep.Game.Features;
using Lockstep.Game.Features.Cleanup;
using Lockstep.Game.Features.Input;
using Lockstep.Logging;
using Lockstep.Math;
using Lockstep.Serialization;
using NetMsg.Game.Tank;
using UnityEngine;
using Debug = UnityEngine.Debug;
using ICommand = NetMsg.Game.Tank.ICommand;

namespace Lockstep.Game {
    public class Simulation {
        public static byte MainActorID;

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

        public Simulation(){ }

        public void OnEvent_OnServerFrames(object param){
            var msg = param as ServerFrames;
            cmdBuffer.PushServerFrames(msg.frames);
        }

        public void OnEvent_OnRoomGameStart(object param){
            var msg = param as InitServerFrame;
            StartGame(msg.RoomID, msg.SimulationSpeed, msg.ActorID, msg.AllActors);
        }

        private NetMgr _netMgr;

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
            _localActorId = localActorId;
            MainActorID = localActorId;
            _localTick = 0;
            _roomId = roomId;
            _world = new World(_context, allActors);
            this.Running = true;
        }

        public void DoDestroy(){ }

        public byte[] _allActors;
        private int _actorCount;
        private int _actorIdx;

        public void Start(int targetFps, byte localActorId, byte[] allActors){
            GameLog.LocalActorId = localActorId;
            GameLog.AllActorIds = allActors;
            MainActorID = localActorId;

            _actorCount = allActors.Length;
            _localActorId = localActorId;
            _allActors = allActors;
            _actorIdx = -1;
            for (int i = 0; i < _actorCount; i++) {
                if (allActors[i] == localActorId) {
                    _actorIdx = i;
                    break;
                }
            }

            Debug.Assert(_actorIdx != -1);
            _tickDt = 1000f / targetFps;
            _world = new World(_context, allActors,
                new InputFeature(_context, Services),
                new CleanupFeature(_context, Services));

            Running = true;
        }


        public void DoUpdate(float elapsedMilliseconds){
            if (!Running) {
                return;
            }
            return;
            if (!cmdBuffer.CanExcuteNextFrame()) {//因为网络问题 需要等待服务器发送确认包 才能继续往前
                return;
            }

            List<ICommand> Commands = InputMgr.GetInputCmds();
            _accumulatedTime += elapsedMilliseconds;
            while (_accumulatedTime >= _tickDt) {
                var input = new PlayerInput(_world.Tick, _localActorId, Commands);
                _netMgr.SendInput(input);
                var frame = new ServerFrame();
                var tick = _world.Tick;
                frame.tick = tick;
                var inputs = new PlayerInput[_actorCount];
                for (int i = 0; i < _actorCount; i++) {
                    inputs[i] = new PlayerInput(tick, _allActors[i], null);
                }

                inputs[_actorIdx] = input;
                //校验服务器包  如果有预测失败 则需要进行回滚
                var isNeedRevert = cmdBuffer.CheckHistoryCmds();
                if (isNeedRevert) {
                    var curTick = _world.Tick;
                    if (cmdBuffer.waitCheckTick > 0) {
                        _world.RevertToTick(cmdBuffer.waitCheckTick - 1);
                    }

                    //Restore last local state       
                    while (_world.Tick < curTick) {
                        _world.Simulate();
                    }
                }

                
                //sendInput
                ProcessInputQueue(frame);
                _world.Predict();
                cmdBuffer.PushLocalFrame(frame);

                _accumulatedTime -= _tickDt;
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

        private void ProcessInputQueue(ServerFrame frame){
            var inputs = frame.inputs;
            foreach (var input in inputs) {
                GameLog.Add(_world.Tick, input);

                foreach (var command in input.Commands) {
                    Log.Trace(this, input.ActorId + " >> " + input.Tick + ": " + input.Commands.Count());

                    var inputEntity = _context.input.CreateEntity();
                    command.Execute(inputEntity);

                    inputEntity.AddTick(input.Tick);
                    inputEntity.AddActorId(input.ActorId);
                }
            }
        }
    }
}