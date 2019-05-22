using UnityEngine;

namespace Lockstep.Game {
    public interface IMapService :IService{
        ushort Pos2TileId(Vector2Int pos, bool isCollider);
        void ReplaceTile(Vector2Int pos, ushort srcId, ushort dstId);
    }

}