using Lockstep.Math;
using Entitas;
using Lockstep.Game;

namespace Lockstep.ECS.Game
{
    [Game]
    [System.Serializable]
    public class AssetComponent : IComponent {
        public EAssetID assetId = EAssetID.Bullet0;
    }
}