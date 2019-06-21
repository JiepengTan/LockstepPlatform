using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BaseTableData
{
    /// <summary>表的所有行的二进制数据，注意只有是单例时，cvsTable才有值，否则是null;这个是基类所以不能使用单例</summary>
    public TableInfo TableTable;

    /// <summary>表中某一行的二进制数据，注意只有是非单例时才有数据，否则是null</summary>
    public TableTypesData bytesData;

    public void LoadTableTable()
    {
        if (TableTable == null)
        {
            TableTable = TableManager.Instance.GetTableTable(Name());
            if (TableTable != null)
            {
                TableManager.Instance.AddTableData(Name(), this);
            }
            else
            {
                Debug.LogErrorFormat("LoadTableTable failed! tableName={0}", Name());
            }
        }
    }

    public virtual void UnLoadData(bool isRemove = true)
    {
        if (TableTable != null)
        {
            TableTable.UnLoad();
            TableTable = null;
        }
    }

    public TableTypesData GetTableBytesData(ulong key)
    {
        return TableTable.GetTableBytesData(key);
    }

    protected virtual string Name()
    {
        return "";
    }

    public Dictionary<ulong, TableTypesData> GetAllTableBytesData()
    {
        return TableTable.GetAllTableBytesData();
    }
    
}

public class TableData<T> : BaseTableData
{
    private static readonly TableData<T> s_instance = new TableData<T>();

    public static TableData<T> Instance { get { return s_instance; } }

    public static Dictionary<ulong, TableData<T>> TableDataDic = new Dictionary<ulong, TableData<T>>();
    public static void Load()
    {
        Instance.LoadTableTable();
    }


    /// <summary>
    /// 获取对象(通过多个Key,最多5个key)
    /// </summary>
    public static TableData<T> GetData(int key1)
    {
        ulong keyID = TableHelper.GetKey(key1);
        TableData<T> data = Instance.Get(keyID);
        return data;
    }

    public static TableData<T> GetData(int key1, int key2)
    {
        ulong keyID = TableHelper.GetKey(key1, key2);
        TableData<T> data = Instance.Get(keyID);
        return data;
    }

    public static TableData<T> GetData(int key1, int key2, int key3)
    {
        ulong keyID = TableHelper.GetKey(key1, key2, key3);
        TableData<T> data = Instance.Get(keyID);
        return data;
    }
    
    public static TableData<T> GetData(int key1, int key2, int key3, int key4)
    {
        ulong keyID = TableHelper.GetKey(key1, key2, key3, key4);
        TableData<T> data = Instance.Get(keyID);
        return data;
    }

    public static Dictionary<ulong, TableData<T>> GetAllRow(bool isCache = true)
    {
        return Instance.GetAll(isCache);
    }

    public static void UnLoad(bool isRemove = true)
    {
        Instance.UnLoadData(isRemove);
    }

    /// <summary>
    /// 通过ID获取对象
    /// </summary>
    private TableData<T> Get(ulong key, bool isCache = true)
    {
        LoadTableTable();
        TableData<T> TableData;
        if (!TableDataDic.TryGetValue(key, out TableData))
        {
            TableTypesData bytesData = GetTableBytesData(key);
            if (bytesData == null)
            {
                return null;
            }
            TableData = GetTableData(bytesData);
            if (isCache)
            {
                if (!TableDataDic.ContainsKey(key))
                {
                    TableDataDic.Add(key, TableData);
                }
            }
        }
        return TableData;
    }

    protected TableData<T> GetTableData(TableTypesData bytesData)
    {
        try
        {
            var TableData = OnGetVal(bytesData);
            return (TableData<T>)TableData;
        }
        catch (Exception exception)
        {
            Debug.LogErrorFormat("{0}表 解析出错 {1}", Name(), exception.StackTrace);
            return null;
        }
    }



    private Dictionary<ulong, TableData<T>> GetAll(bool isCache)
    {
        LoadTableTable();
        Dictionary<ulong, TableData<T>> allDic = new Dictionary<ulong, TableData<T>>();
        if (TableDataDic.Count == GetAllTableBytesData().Count)
        {
            allDic = TableDataDic;
        }
        else
        {
            TableDataDic.Clear();
            var _itor = GetAllTableBytesData().GetEnumerator();
            while (_itor.MoveNext())
            {
                var TableData = Get(_itor.Current.Key);
                allDic.Add(_itor.Current.Key, TableData);
            }
            if (isCache)
            {
                TableDataDic = allDic;
            }
        }
        return allDic;
    }

    public override void UnLoadData(bool isRemove = true)
    {
        base.UnLoadData(isRemove);
        TableDataDic.Clear();
        if (isRemove)
        {
            TableManager.Instance.RemoveTableData(Name());
        }
    }
    protected virtual BaseTableData OnGetVal(TableTypesData bytesData)
    {
        return null;
    }
}