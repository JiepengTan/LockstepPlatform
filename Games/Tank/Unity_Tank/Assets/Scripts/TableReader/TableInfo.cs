using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TableInfo
{
    protected Dictionary<ulong, TableTypesData> TableBytesDataDic = new Dictionary<ulong, TableTypesData>();
    protected List<TableField> fieldList = new List<TableField>();

    public void Init(Dictionary<ulong, TableTypesData> TableBytesDataDic, List<TableField> fieldList)
    {
        this.TableBytesDataDic = TableBytesDataDic;
        this.fieldList = fieldList;
    }

    public virtual void UnLoad(bool isRemove = true)
    {
        TableBytesDataDic.Clear();
        fieldList.Clear();
    }

    public TableTypesData GetByKey(int key1)
    {
        ulong key = TableHelper.GetKey(key1);
        TableTypesData TableBytesData = GetTableBytesData(key);
        return TableBytesData;
    }

    public TableTypesData GetByKey(int key1, int key2)
    {
        ulong key = TableHelper.GetKey(key1, key2);
        TableTypesData TableBytesData = GetTableBytesData(key);
        return TableBytesData;
    }

    public TableTypesData GetByKey(int key1, int key2, int key3)
    {
        ulong key = TableHelper.GetKey(key1, key2, key3);
        TableTypesData TableBytesData = GetTableBytesData(key);
        return TableBytesData;
    }

    public TableTypesData GetByKey(int key1, int key2, int key3, int key4)
    {
        ulong key = TableHelper.GetKey(key1, key2, key3, key4);
        TableTypesData TableBytesData = GetTableBytesData(key);
        return TableBytesData;
    }
    

    public TableTypesData GetTableBytesData(ulong key)
    {
        TableTypesData TableBytesData;
        if (TableBytesDataDic.TryGetValue(key, out TableBytesData))
        {
            return TableBytesData;
        }
        else
        {
            return null;
        }
    }

    public Dictionary<ulong, TableTypesData> GetAllTableBytesData()
    {
        return TableBytesDataDic;
    }


}
