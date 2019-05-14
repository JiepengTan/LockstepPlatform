#define DEBUG_FRAME_DELAY
using System;
using System.Collections;
using LiteNetLib;
using Lockstep.Core.Logic.Interfaces;
using Lockstep.Serialization;
using NetMsg.Game.Tank;
using NetMsg.Lobby;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.Game {
    /// 房间消息代理 UDP  
    public class NetProxyRoom : BaseNetProxy { }
}