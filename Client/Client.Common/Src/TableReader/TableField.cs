using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TableField
{
    public string strDefine;
    public TableDefine define;
    public string strFieldType;
    public ETableBaseType fieldType;
    public bool isBase = true;
    public bool isList = false;

    public const String BYTE = "byte";
    public const String SHORT = "short";
    public const String INT = "int";
    public const String FLOAT = "float";
    public const String BOOL = "bool";
    public const String STRING = "string";
    public const String ULONG = "long";

    public const String BeTranslate = "translate";
    public const String KEY = "key";
    public const String DEL = "del";

    public void SetDefine(string strDefine)
    {
        this.strDefine = strDefine;
        switch (strDefine)
        {
            case KEY:
                define = TableDefine.Key;
                break;
            case BeTranslate:
                define = TableDefine.BeTranslate;
                break;
            default:
                define = TableDefine.Normal;
                break;
        }
    }

    public void SetFieldType(string strFieldType)
    {
        this.strFieldType = strFieldType;
        switch (strFieldType)
        {
            case BYTE:
                fieldType = ETableBaseType.Byte;
                break;
            case SHORT:
                fieldType = ETableBaseType.Short;
                break;
            case INT:
                fieldType = ETableBaseType.Int;
                break;
            case FLOAT:
                fieldType = ETableBaseType.Float;
                break;
            case BOOL:
                fieldType = ETableBaseType.Bool;
                break;
            case STRING:
                fieldType = ETableBaseType.String;
                break;
            case ULONG:
                fieldType = ETableBaseType.Long;
                break;
            default:
                throw new Exception("SetFieldType");
                break;
        }
    }
}

public enum ETableBaseType
{
    Byte,
    Short,
    Int,
    Long,
    Bool,
    Float,
    String,
}

public enum TableDefine
{
    Normal,
    Key,
    BeTranslate,
}