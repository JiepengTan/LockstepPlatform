using System.Collections.Generic;
using Lockstep.Math;
using NetMsg.Common;

namespace Lockstep.Game
{
    public interface IInputService : IService {
        void Execute(InputCmd cmd, InputEntity entity);
        List<InputCmd> GetInputCmds();
    }
}