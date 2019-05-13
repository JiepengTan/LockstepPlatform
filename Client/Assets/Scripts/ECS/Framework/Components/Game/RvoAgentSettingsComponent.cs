using System.Collections.Generic;
using Entitas;
using Lockstep.Math;

namespace Lockstep.Core.State.Game
{
    public class RvoAgentSettingsComponent : IComponent
    {
        public LVector2 preferredVelocity;
        public LFloat timeHorizonObst;
        public IList<KeyValuePair<LFloat, uint>> agentNeighbors;
    }
}