using UnityEngine;

namespace Lockstep.Game {
    public interface IMapManager {
        TileInfos GetMapInfo(string name);
        void LoadLevel(int level);
        Vector2Int mapMin { get; }
        Vector2Int mapMax { get; }
    }
}