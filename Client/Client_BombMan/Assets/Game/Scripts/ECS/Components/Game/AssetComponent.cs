﻿using Lockstep.Math;
using Entitas;
using Lockstep.Game;

namespace Lockstep.ECS.Game
{
    [Game]
    [System.Serializable]
    public partial class AssetComponent : IComponent {
        public EAssetID assetId = EAssetID.Bullet0;
    }
}