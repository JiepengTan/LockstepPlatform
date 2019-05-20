using UnityEngine;

namespace Lockstep.Game {
    public interface IMapService :IService{
        ushort Pos2TileID(Vector2Int pos, bool isCollider);
        void ReplaceTile(Vector2Int pos, ushort srcID, ushort dstID);
    }

}