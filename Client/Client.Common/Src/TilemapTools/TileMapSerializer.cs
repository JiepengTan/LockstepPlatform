//目的是将Tile map 序列化到文件中
using System;
using System.Collections.Generic;
using System.IO;
using Lockstep.Game;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TileMapSerializer {
    static readonly byte[] MAGIC_BYTES = new byte[4] {102, 109, 74, 80};

    static bool CheckMagicNumber(byte[] bs){
        return CheckBytes(MAGIC_BYTES, bs);
    }

    static bool CheckBytes(byte[] ba, byte[] bb){
        if ((bb == null) != (ba == null))
            return false;
        if (bb == null)
            return true;
        if (bb.Length != ba.Length) return false;
        for (int i = 0; i < 4; i++) {
            if (ba[i] != bb[i])
                return false;
        }
        return true;
    }

    public class FileContentException : Exception {
        public FileContentException(string message) : base(message){ }
    }

    /// <summary>
    /// 将整个Grid 进行序列化到文件中
    /// </summary>
    /// <returns></returns>
    public static byte[] SerializeGrid(Grid grid, Func<TileBase, ushort> FuncGetTileIdx){
        if (grid == null)
            return null;
        var ms = new MemoryStream();
        BinaryWriter writer = new BinaryWriter(ms);

        writer.Write(grid.cellSize);
        writer.Write(grid.cellGap);
        writer.Write((int) grid.cellLayout);
        writer.Write((int) grid.cellSwizzle);

        var allMaps = grid.GetComponentsInChildren<Tilemap>();
        writer.Write(allMaps.Length);
        foreach (var map in allMaps) {
            writer.Write(map.name);
        }

        foreach (var map in allMaps) {
            WriteMap(writer, map, FuncGetTileIdx);
        }

        return ms.ToArray();
    }
    
    public static GridInfo ReadGrid(BinaryReader reader){
        var grid = new GridInfo();
        reader.ReadVector3(ref grid.cellSize);
        reader.ReadVector3(ref grid.cellGap);
        grid.cellLayout = reader.ReadInt32();
        grid.cellSwizzle = reader.ReadInt32();
        var len = reader.ReadInt32();
        grid.tileMaps = new TileInfos[len];
        grid.names = new string[len];
        for (int i = 0; i < len; i++) {
            var name = reader.ReadString();
            grid.names[i] = name;
        }

        for (int i = 0; i < len; i++) {
            var map = ReadMap(reader);
            grid.tileMaps[i] = map;
        }

        return grid;
    }

    static void WriteMap(BinaryWriter writer, Tilemap map, Func<TileBase, ushort> FuncGetTileIdx){
        Debug.Assert(map.cellBounds.size.z == 1, "map.cellBounds.size.z == 1");
        var tiles = map.GetTilesBlock(map.cellBounds);
        Dictionary<TileBase, int> allTileTypes = new Dictionary<TileBase, int>();
        var totalCount = tiles.Length;
        int notNullCount = 0;
        for (int i = 0; i < totalCount; i++) {
            var tile = tiles[i];
            if (tile != null) {
                notNullCount++;
                if (allTileTypes.TryGetValue(tile, out int tileCount)) {
                    allTileTypes[tile] = tileCount + 1;
                }
                else {
                    allTileTypes.Add(tile, 1);
                }
            }
        }

        bool isUseByteForPos = map.cellBounds.size.x < 255 && map.cellBounds.size.y < 255;
        float saveWithPosRate = isUseByteForPos ? (1.0f / (1 + 1 + 1)) : (1.0f / (2 + 2 + 1));
        bool isSaveWithOutPos = notNullCount * 1.0f / totalCount > saveWithPosRate;
        var diffCount = allTileTypes.Count;
        int needSize = 4 //magicNumber
                       + 4 //needSize 
                       + 4 //totalCount
                       + 2 //bool flag
                       + 4 * 4 //bound info
                       + 4 //notNullCount
                       + 4 //diffCount
                       + 2 * diffCount //head
                       + (isSaveWithOutPos
                               ? totalCount
                               : //isSaveWithOutPos
                               (isUseByteForPos
                                   ? notNullCount * 3 //1*2 byte pos 1byte val
                                   : //
                                   notNullCount * 5) //2*2 byte pos 1byte val
                       )
            ;
        writer.Write(MAGIC_BYTES);
        writer.Write(needSize);
        writer.Write(totalCount);
        writer.Write(isUseByteForPos);
        writer.Write(isSaveWithOutPos);
        var bound = map.cellBounds;
        var sizex = bound.size.x;
        var sizey = bound.size.y;
        writer.Write(bound.min.x);
        writer.Write(bound.min.y);
        writer.Write(sizex);
        writer.Write(sizey);
        writer.Write(notNullCount);

        Dictionary<TileBase, int> tile2ID = new Dictionary<TileBase, int>(diffCount);
        int idx = 0;
        ushort[] tileRawIDs = new ushort[diffCount];
        foreach (var pair in allTileTypes) {
            var tile = pair.Key;
            tileRawIDs[idx] = FuncGetTileIdx(pair.Key);
            tile2ID[tile] = ++idx; //0保留作为null 
        }

        //wirte pallete
        Debug.Assert(idx <= 254, string.Format("The num of tile type in single tilemap is too much {0}>254", idx));
        writer.Write(diffCount);
        for (int i = 0; i < diffCount; i++) {
            writer.Write(tileRawIDs[i]);
        }

        if (isSaveWithOutPos) {
            for (int i = 0; i < totalCount; i++) {
                var tile = tiles[i];
                if (tile == null) {
                    writer.Write((byte) 0);
                }
                else {
                    writer.Write((byte) (tile2ID[tile]));
                }
            }
        }
        else {
            if (isUseByteForPos) {
                for (int x = 0; x < sizex; x++) {
                    for (int y = 0; y < sizey; y++) {
                        var tile = tiles[y * sizex + x];
                        if (tile != null) {
                            writer.Write((byte) x, (byte) y, (byte) (tile2ID[tile]));
                        }
                    }
                }
            }
            else {
                for (int x = 0; x < sizex; x++) {
                    for (int y = 0; y < sizey; y++) {
                        var tile = tiles[y * sizex + x];
                        if (tile != null) {
                            writer.Write((ushort) x, (ushort) y, (byte) (tile2ID[tile]));
                        }
                    }
                }
            }
        }
    }

    static TileInfos ReadMap(BinaryReader reader){
        //read 
        var pos = reader.BaseStream.Position;
        var len = reader.BaseStream.Length;
        if (len - pos < 8) {
            throw new FileContentException(string.Format("FileSpace is not enough for head!! pos = {0} len = {1} ", pos,
                len));
        }

        var magicNum = reader.ReadBytes(4);
        if (!CheckMagicNumber(magicNum)) {
            throw new FileContentException("MagicNumberError");
            return null;
        }

        var needSize = reader.ReadInt32();
        //CheckSize
        if (len - pos < needSize) {
            throw new FileContentException(string.Format("FileSpace is not enough!! pos = {0} len = {1} needSize = {2}",
                pos, len, needSize));
            return null;
        }

        var totalCount = reader.ReadInt32();
        var isUseByteForPos = reader.ReadBoolean();
        var isSaveWithOutPos = reader.ReadBoolean();
        var minx = reader.ReadInt32();
        var miny = reader.ReadInt32();
        var sizex = reader.ReadInt32();
        var sizey = reader.ReadInt32();
        var notNullCount = reader.ReadInt32();
        if (notNullCount > totalCount) {
            throw new FileContentException(string.Format("notNullCount {0} > count {1}", notNullCount, totalCount));
            return null;
        }

        var tiles = new ushort[totalCount];
        var retVal = new TileInfos();
        retVal.tileIDs = tiles;
        retVal.min = new Vector2Int(minx, miny);
        retVal.size = new Vector2Int(sizex, sizey);

        var diffCount = reader.ReadInt32();
        ushort[] tileRawIDs = new ushort[diffCount];
        for (int i = 0; i < diffCount; i++) {
            tileRawIDs[i] = reader.ReadUInt16();
        }

        Dictionary<ushort, ushort> id2Tile = new Dictionary<ushort, ushort>();
        for (ushort i = 0; i < diffCount; i++) {
            id2Tile[(ushort)(i+1)] = tileRawIDs[i];
        }

        if (isSaveWithOutPos) {
            for (int i = 0; i < totalCount; i++) {
                var val = reader.ReadByte();
                if (val == 0) {
                    tiles[i] = 0;
                }
                else {
                    if (id2Tile.TryGetValue(val, out ushort tile)) {
                        tiles[i] = tile;
                    }
                }
            }
        }
        else {
            if (isUseByteForPos) {
                for (int idx = 0; idx < notNullCount; idx++) {
                    var x = reader.ReadByte();
                    var y = reader.ReadByte();
                    var val = reader.ReadByte();
                    if (id2Tile.TryGetValue(val, out ushort tile)) {
                        tiles[y * sizex + x] = tile;
                    }

                    Debug.Assert(tiles[y * sizex + x] != 0, "");
                }
            }
            else {
                for (int idx = 0; idx < notNullCount; idx++) {
                    var x = reader.ReadUInt16();
                    var y = reader.ReadUInt16();
                    var val = reader.ReadByte();
                    if (id2Tile.TryGetValue(val, out ushort tile)) {
                        tiles[y * sizex + x] = tile;
                    }

                    Debug.Assert(tiles[y * sizex + x] != 0, "");
                }
            }
        }
        return retVal;
    }
}