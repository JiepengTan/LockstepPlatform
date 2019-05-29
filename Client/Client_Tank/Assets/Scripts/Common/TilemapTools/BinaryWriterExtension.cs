
using System.IO;
using UnityEngine;

public static class BinaryWriterExtension {
    public static void Write(this BinaryWriter writer, Vector3 vec){
        writer.Write(vec.x);
        writer.Write(vec.y);
        writer.Write(vec.z);
    }

    public static void Write(this BinaryWriter writer, byte v1, byte v2, byte v3){
        writer.Write(v1);
        writer.Write(v2);
        writer.Write(v3);
    }

    public static void Write(this BinaryWriter writer, ushort v1, ushort v2, byte v3){
        writer.Write(v1);
        writer.Write(v2);
        writer.Write(v3);
    }

    public static void ReadVector3(this BinaryReader reader, ref Vector3 vec){
        vec.x = reader.ReadSingle();
        vec.y = reader.ReadSingle();
        vec.z = reader.ReadSingle();
    }
}