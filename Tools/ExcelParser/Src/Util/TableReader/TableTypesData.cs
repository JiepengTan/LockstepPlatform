using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TableTypesData
{
    private byte[] fieldData;
    private int cursor;

    public void Init(byte[] allFieldData, List<TableField> fieldList,string tableName)
    {
        this.fieldData = allFieldData;
        cursor = 0;
    }
    public void BeginLoad()
    {
        cursor = 0;
    }
    #region Reader
    public byte ReadByte()
    {
        byte byteValue = fieldData[cursor];
        cursor++;
        return byteValue;
    }

    public ushort ReadUInt16()
    {
        ushort ushortValue = BitConverter.ToUInt16(fieldData, cursor);
        cursor += 2;
        return ushortValue;
    }

    public short ReadInt16()
    {
        short shortValue = BitConverter.ToInt16(fieldData, cursor);
        cursor += 2;
        return shortValue;
    }

    public uint ReadUInt32()
    {
        uint uintValue = BitConverter.ToUInt32(fieldData, cursor);
        cursor += 4;
        return uintValue;
    }

    public int ReadInt32()
    {
        int intValue = BitConverter.ToInt32(fieldData, cursor);
        cursor += 4;
        return intValue;
    }


    public ulong ReadUInt64()
    {
        ulong ulongValue = BitConverter.ToUInt64(fieldData, cursor);
        cursor += 8;
        return ulongValue;
    }

    public long ReadInt64()
    {
        long longValue = BitConverter.ToInt64(fieldData, cursor);
        cursor += 8;
        return longValue;
    }

    public bool ReadBoolean()
    {
        bool boolValue = BitConverter.ToBoolean(fieldData, cursor);
        cursor += 1;
        return boolValue;
    }

    public float ReadSingle()
    {
        float floatValue = BitConverter.ToSingle(fieldData, cursor);
        cursor += 4;
        return floatValue;
    }
    public string ReadString(bool needTranslate = false,string tableName = "")
    {
        int length = BitConverter.ToInt32(fieldData, cursor);
        cursor += 4;
        string strValue = Encoding.UTF8.GetString(fieldData, cursor, length);
        cursor += length;
        if (needTranslate )
        {
            //strValue = TableHelper.GetLanguageValue(strValue, tableName);
        }
        return strValue;
    }
    #endregion
    
    #region Writer

    #endregion

 
    public string ReadListInt32()
    {
        return ReadString(false);
    }
    public string ReadListInt16()
    {
        return ReadString(false);
    }
    public string ReadListByte()
    {
        return ReadString(false);
    }
}
