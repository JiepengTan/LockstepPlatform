using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Lockstep.Game;
using UnityEngine;
public class TableManager 
{
    public static TableManager _Instance = new TableManager();
    public static TableManager Instance => _Instance;
    /// <summary> 已加载表的单例缓存，比如TableHero.Instance </summary>
    Dictionary<string, BaseTableData> _tableDataDic = new Dictionary<string, BaseTableData>();
    /// <summary> 已加载表的二进制数据，每个表的单例包含一个TableTable </summary>
    Dictionary<string, TableInfo> _tableInfoDic = new Dictionary<string, TableInfo>();

    public TableInfo GetTableTable(string tableName)
    {
        TableInfo tableInfo;

        if (_tableInfoDic.ContainsKey(tableName))
        {
            tableInfo = _tableInfoDic[tableName];
        }
        else
        {
            tableInfo = new TableInfo();
            Dictionary<ulong, TableTypesData> TableBytesDataDic = new Dictionary<ulong, TableTypesData>();
            byte[] data = TableHelper.GetBytes(tableName);
            if (data == null)
            {
                return null;
            }
            MemoryStream stream = new MemoryStream(data);
            BinaryReader reader = null;
            List<TableField> tableFieldList = new List<TableField>();
            try
            {
                reader = new BinaryReader(stream);

                int rowsCount = reader.ReadUInt16(); //行数，不包括标题行
                int columnsCount = reader.ReadByte(); //列数（也就是Table表每行字段数）
                for (int i = 0; i < columnsCount; i++)
                {
                    TableField tableField = new TableField();
                    var val = (int)reader.ReadByte();
                    tableField.isBase = ((val & 0x1) != 0);
                    tableField.isList = ((val & 0x2) != 0);
                    tableField.define = (TableDefine)((val >> 2) & 0x3);
                    tableField.fieldType = (ETableBaseType)((val >> 4) & 0xf);
                    tableFieldList.Add(tableField);
                }


                for (int i = 0; i < rowsCount; i++)
                {
                    ulong key = reader.ReadUInt64();
                    int len = reader.ReadInt32();
                    byte[] allFieldData = reader.ReadBytes(len);
                    TableTypesData TableDate = new TableTypesData();
                    TableDate.Init(allFieldData, tableFieldList, tableName);
                    TableBytesDataDic.Add(key, TableDate);
                }
            }
            catch (Exception exception)
            {
                Debug.LogErrorFormat("{0}表 二进制数据解析出错 {1}", tableName, exception.StackTrace);
                return null;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            tableInfo.Init(TableBytesDataDic, tableFieldList);
            _tableInfoDic.Add(tableName, tableInfo);
        }
        return tableInfo;
    }

    public void AddTableData(string tableName, BaseTableData TableData)
    {
        _tableDataDic.Add(tableName, TableData);
    }

    public void RemoveTableData(string tableName)
    {
        if (_tableDataDic.ContainsKey(tableName))
        {
            _tableDataDic.Remove(tableName);
        }
        if (_tableInfoDic.ContainsKey(tableName))
        {
            _tableInfoDic.Remove(tableName);
        }

    }

    public void UnLoadData(string tableName)
    {
        if (_tableDataDic.ContainsKey(tableName))
        {
            _tableDataDic[tableName].UnLoadData();
        }
        if (_tableInfoDic.ContainsKey(tableName))
        {
            _tableInfoDic[tableName].UnLoad();
        }
        RemoveTableData(tableName);
    }

    public void UnLoadALlTable()
    {
        foreach (BaseTableData item in _tableDataDic.Values)
        {
            item.UnLoadData(false);
        }
        _tableDataDic.Clear();
        _tableInfoDic.Clear();
    }
}

