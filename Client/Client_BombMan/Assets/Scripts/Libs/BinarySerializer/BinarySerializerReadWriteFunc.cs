using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BinarySerializer {
    public static partial class BinarySerializer {
        public static void Read<T>(byte[] fieldData, ref int cursor, ref T val){
            val = _ReadHelper<T>.Call(fieldData, ref cursor);
        }

        public static void Write<T>(System.IO.BinaryWriter stream, T val){
            _WriteHelper<T>.Call(stream, val);
        }


        public static T[] ReadArray<T>(byte[] fieldData, ref int cursor, T[] _){
            bool isNull = BinarySerializer.ReadBoolean(fieldData, ref cursor);
            if (isNull) return null;
            var count = BinarySerializer.ReadInt32(fieldData, ref cursor);
            var lst = new T[count];
            FuncRead<T> Func = GetReadFunc<T>();
            for (int i = 0; i < count; i++) {
                T val = Func(fieldData, ref cursor);
                lst[i] = val;
            }

            return lst;
        }

        public static void WriteArray<T>(System.IO.BinaryWriter write, T[] lst){
            bool isNull = lst == null;
            BinarySerializer.WriteBoolean(write, isNull);
            if (isNull) return;
            var count = lst.Length;
            BinarySerializer.WriteInt32(write, count);
            FuncWrite<T> Func = GetWriteFunc<T>();
            for (int i = 0; i < count; i++) {
                Func(write, lst[i]);
            }
        }

        public static List<T> ReadList<T>(byte[] fieldData, ref int cursor, List<T> lst){
            bool isNull = BinarySerializer.ReadBoolean(fieldData, ref cursor);
            if (isNull) return null;
            if (lst == null) lst = new List<T>();
            var count = BinarySerializer.ReadInt32(fieldData, ref cursor);
            FuncRead<T> Func = GetReadFunc<T>();
            for (int i = 0; i < count; i++) {
                T val = Func(fieldData, ref cursor);
                lst.Add(val);
            }

            return lst;
        }

        public static void WriteList<T>(System.IO.BinaryWriter write, List<T> lst){
            bool isNull = lst == null;
            BinarySerializer.WriteBoolean(write, isNull);
            if (isNull) return;
            var count = lst.Count;
            BinarySerializer.WriteInt32(write, count);
            var Func = GetWriteFunc<T>();
            for (int i = 0; i < count; i++) {
                Func(write, lst[i]);
            }
        }


        public static Dictionary<T1, T2> ReadDict<T1, T2>(byte[] fieldData, ref int cursor, Dictionary<T1, T2> dict){
            bool isNull = BinarySerializer.ReadBoolean(fieldData, ref cursor);
            if (isNull) return null;
            if (dict == null) dict = new Dictionary<T1, T2>();
            var count = BinarySerializer.ReadInt32(fieldData, ref cursor);
            var FuncKey = GetReadFunc<T1>();
            var FuncVal = GetReadFunc<T2>();
            for (int i = 0; i < count; i++) {
                var key = FuncKey(fieldData, ref cursor);
                var val = FuncVal(fieldData, ref cursor);
                dict.Add(key, val);
            }

            return dict;
        }

        public static void WriteDict<T1, T2>(System.IO.BinaryWriter write, Dictionary<T1, T2> dict){
            bool isNull = dict == null;
            BinarySerializer.WriteBoolean(write, isNull);
            if (isNull) return;
            var count = dict.Count;
            BinarySerializer.WriteInt32(write, count);
            var iter = dict.GetEnumerator();
            var FuncKey = GetWriteFunc<T1>();
            var FuncVal = GetWriteFunc<T2>();
            while (iter.MoveNext()) {
                var key = iter.Current.Key;
                var val = iter.Current.Value;
                FuncKey(write, key);
                FuncVal(write, val);
            }

            iter.Dispose();
        }

        public static bool ReadBoolean(byte[] fieldData, ref int cursor){
            bool boolValue = BitConverter.ToBoolean(fieldData, cursor);
            cursor += 1;
            return boolValue;
        }

        public static string ReadString(byte[] fieldData, ref int cursor){
            bool isNull = BitConverter.ToBoolean(fieldData, cursor);
            cursor += 1;
            if (isNull) return null;
            int length = BitConverter.ToInt32(fieldData, cursor);
            cursor += 4;
            string strValue = Encoding.UTF8.GetString(fieldData, cursor, length);
            cursor += length;
            return strValue;
        }

        public static float ReadSingle(byte[] fieldData, ref int cursor){
            float floatValue = BitConverter.ToSingle(fieldData, cursor);
            cursor += 4;
            return floatValue;
        }

        public static sbyte ReadSByte(byte[] fieldData, ref int cursor){
            sbyte sbyteValue;
            byte byteValue = fieldData[cursor];
            if (byteValue >= 128) {
                sbyteValue = (sbyte) (byteValue - 256);
            }
            else {
                sbyteValue = (sbyte) byteValue;
            }

            cursor++;
            return sbyteValue;
        }

        public static byte ReadByte(byte[] fieldData, ref int cursor){
            byte byteValue = fieldData[cursor];
            cursor++;
            return byteValue;
        }

        public static ushort ReadUInt16(byte[] fieldData, ref int cursor){
            ushort ushortValue = BitConverter.ToUInt16(fieldData, cursor);
            cursor += 2;
            return ushortValue;
        }

        public static short ReadInt16(byte[] fieldData, ref int cursor){
            short shortValue = BitConverter.ToInt16(fieldData, cursor);
            cursor += 2;
            return shortValue;
        }

        public static uint ReadUInt32(byte[] fieldData, ref int cursor){
            uint uintValue = BitConverter.ToUInt32(fieldData, cursor);
            cursor += 4;
            return uintValue;
        }

        public static int ReadInt32(byte[] fieldData, ref int cursor){
            int intValue = BitConverter.ToInt32(fieldData, cursor);
            cursor += 4;
            return intValue;
        }

        public static ulong ReadUInt64(byte[] fieldData, ref int cursor){
            ulong ulongValue = BitConverter.ToUInt64(fieldData, cursor);
            cursor += 8;
            return ulongValue;
        }

        public static long ReadInt64(byte[] fieldData, ref int cursor){
            long longValue = BitConverter.ToInt64(fieldData, cursor);
            cursor += 8;
            return longValue;
        }

        public static Vector2 ReadVector2(byte[] fieldData, ref int cursor){
            Vector2 ret = new Vector2();
            ret.x = BitConverter.ToSingle(fieldData, cursor);
            cursor += 4;
            ret.y = BitConverter.ToSingle(fieldData, cursor);
            cursor += 4;
            return ret;
        }

        public static Vector3 ReadVector3(byte[] fieldData, ref int cursor){
            Vector3 ret = new Vector3();
            ret.x = BitConverter.ToSingle(fieldData, cursor);
            cursor += 4;
            ret.y = BitConverter.ToSingle(fieldData, cursor);
            cursor += 4;
            ret.z = BitConverter.ToSingle(fieldData, cursor);
            cursor += 4;
            return ret;
        }

        public static Vector2Int ReadVector2Int(byte[] fieldData, ref int cursor){
            Vector2Int ret = new Vector2Int();
            ret.x = BitConverter.ToInt32(fieldData, cursor);
            cursor += 4;
            ret.y = BitConverter.ToInt32(fieldData, cursor);
            cursor += 4;
            return ret;
        }

        public static Vector3Int ReadVector3Int(byte[] fieldData, ref int cursor){
            Vector3Int ret = new Vector3Int();
            ret.x = BitConverter.ToInt32(fieldData, cursor);
            cursor += 4;
            ret.y = BitConverter.ToInt32(fieldData, cursor);
            cursor += 4;
            ret.z = BitConverter.ToInt32(fieldData, cursor);
            cursor += 4;
            return ret;
        }

        public static Color ReadColor(byte[] fieldData, ref int cursor){
            Color ret = new Color();
            ret.r = BitConverter.ToSingle(fieldData, cursor);
            cursor += 4;
            ret.g = BitConverter.ToSingle(fieldData, cursor);
            cursor += 4;
            ret.b = BitConverter.ToSingle(fieldData, cursor);
            cursor += 4;
            ret.a = BitConverter.ToSingle(fieldData, cursor);
            cursor += 4;
            return ret;
        }

        public static void WriteSingle(System.IO.BinaryWriter stream, float val){
            stream.Write(val);
        }

        public static void WriteBoolean(System.IO.BinaryWriter stream, bool val){
            stream.Write(val);
        }

        public static void WriteString(System.IO.BinaryWriter stream, string val){
            bool isNull = val == null;
            stream.Write(isNull);
            if (isNull) return;
            var bytes = System.Text.Encoding.UTF8.GetBytes(val);
            stream.Write(bytes.Length);
            stream.Write(bytes);
        }

        public static void WriteByte(System.IO.BinaryWriter stream, byte val){
            stream.Write(val);
        }

        public static void WriteInt16(System.IO.BinaryWriter stream, Int16 val){
            stream.Write(val);
        }

        public static void WriteUInt16(System.IO.BinaryWriter stream, UInt16 val){
            stream.Write(val);
        }

        public static void WriteInt32(System.IO.BinaryWriter stream, Int32 val){
            stream.Write(val);
        }

        public static void WriteUInt32(System.IO.BinaryWriter stream, UInt32 val){
            stream.Write(val);
        }

        public static void WriteInt64(System.IO.BinaryWriter stream, Int64 val){
            stream.Write(val);
        }

        public static void WriteUInt64(System.IO.BinaryWriter stream, UInt64 val){
            stream.Write(val);
        }

        public static void WriteVector2(System.IO.BinaryWriter stream, Vector2 val){
            WriteSingle(stream, val.x);
            WriteSingle(stream, val.y);
        }

        public static void WriteVector3(System.IO.BinaryWriter stream, Vector3 val){
            WriteSingle(stream, val.x);
            WriteSingle(stream, val.y);
            WriteSingle(stream, val.z);
        }

        public static void WriteVector2Int(System.IO.BinaryWriter stream, Vector2Int val){
            WriteInt32(stream, val.x);
            WriteInt32(stream, val.y);
        }

        public static void WriteVector3Int(System.IO.BinaryWriter stream, Vector3Int val){
            WriteInt32(stream, val.x);
            WriteInt32(stream, val.y);
            WriteInt32(stream, val.z);
        }

        public static void WriteColor(System.IO.BinaryWriter stream, Color val){
            WriteSingle(stream, val.r);
            WriteSingle(stream, val.g);
            WriteSingle(stream, val.b);
            WriteSingle(stream, val.a);
        }
    }
}