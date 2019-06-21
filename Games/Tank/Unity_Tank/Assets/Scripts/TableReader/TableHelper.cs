using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

/// <summary>
/// Table解析类
/// </summary>
public class TableHelper
{

    public static byte[] GetBytes(string tableName)
    {
        byte[] data = null;

        return data;
    }
    /// <summary>获取文本 </summary>
    public static string GetLanguageValue(string _chinese_content, string _table_name)
    {
        return "";
    }

    public static ulong GetKey(int key1)
    {
        return Convert.ToUInt64(key1);
    }

    public static ulong GetKey(int key1, int key2)
    {
        return (((ulong)key1 & 0xffffffff) | (((ulong)key2 & 0xffffffff) << 32));
    }

    public static ulong GetKey(int key1, int key2, int key3)
    {
        short shortKey2 = Convert.ToInt16(key2);
        short shortKey3 = Convert.ToInt16(key3);
        return (((ulong)key1 & 0xffffffff) | (((ulong)shortKey2 & 0xffff) << 32) | (((ulong)shortKey3 & 0xffff) << 48));
    }


    public static ulong GetKey(int key1, int key2, int key3, int key4)
    {
        short shortKey1 = Convert.ToInt16(key1);
        short shortKey2 = Convert.ToInt16(key2);
        short shortKey3 = Convert.ToInt16(key3);
        short shortKey4 = Convert.ToInt16(key4);
        return (((ulong)shortKey1 & 0xffff) | (((ulong)shortKey2 & 0xffff) << 16) | (((ulong)shortKey3 & 0xffff) << 32) | (((ulong)shortKey4 & 0xffff) << 48));
    }


    public static void WriteByte(BinaryWriter writer, string str)
    {
        var val = string.IsNullOrEmpty(str) ? (byte)0 : byte.Parse(str);
        writer.Write(val);
    }
    public static void WriteInt16(BinaryWriter writer, string str)
    {
        var val = string.IsNullOrEmpty(str) ? (short)0 : short.Parse(str);
        writer.Write(val);
    }
    public static void WriteInt32(BinaryWriter writer, string str)
    {
        var val = string.IsNullOrEmpty(str) ? (int)0 : int.Parse(str);
        writer.Write(val);
    }
    public static void WriteInt64(BinaryWriter writer, string str)
    {
        var val = string.IsNullOrEmpty(str) ? (long)0 : long.Parse(str);
        writer.Write(val);
    }
    public static void WriteSingle(BinaryWriter writer, string str)
    {
        var val = string.IsNullOrEmpty(str) ? (float)0 : float.Parse(str);
        writer.Write(val);
    }
    public static void WriteBoolean(BinaryWriter writer, string str)
    {
        if (str == "0")
        {
            writer.Write(false);
            return;
        }
        if (str == "1")
        {
            writer.Write(true);
            return;
        }
        var val = !string.IsNullOrEmpty(str) && bool.Parse(str);
        writer.Write(val);
    }
    public static void WriteString(BinaryWriter writer, string str)
    {
        var val = str ?? "";
        writer.Write(val.Length);
        writer.Write(val);
    }
    public static void WriteListInt32(BinaryWriter writer, string str)
    {
        var val = str ?? "";
        writer.Write(val.Length);
        writer.Write(val);
    }
    public static void WriteListInt16(BinaryWriter writer, string str)
    {
        var val = str ?? "";
        writer.Write(val.Length);
        writer.Write(val);
    }
    public static void WriteListByte(BinaryWriter writer, string str)
    {
        var val = str ?? "";
        writer.Write(val.Length);
        writer.Write(val);
    }

}
