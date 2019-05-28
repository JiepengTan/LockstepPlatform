using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Collections;
namespace BinarySerializer {
    public interface ITypeHandler {
        string DealType(Type t, List<string> sbfs);
        IFiledHandler[] GetFiledHandlers();
    }
    public interface IFiledHandler {
        string DealDic(Type t, FieldInfo field);
        string DealList(Type t, FieldInfo field);
        string DealArray(Type t, FieldInfo field);
        string DealUserClass(Type t, FieldInfo field);
        string DealEnum(Type t, FieldInfo field);
        string DealStructOrString(Type t, FieldInfo field);
    }

    public class TypeHandlerBinary : ITypeHandler {
        public class FiledHandler : IFiledHandler {
            string prefix = "        ";

            protected string _defaultcodetemplete;
            protected string _enumcodetemplete;
            protected string _clscodetemplete;
            protected string _arraycodetemplete;
            protected string _lstcodetemplete;
            protected string _dictcodetemplete;
            string defaultcodetemplete { get { return _defaultcodetemplete; } }
            string enumcodetemplete { get { return _enumcodetemplete; } }
            string clscodetemplete { get { return _clscodetemplete; } }
            string arraycodetemplete { get { return _arraycodetemplete; } }
            string lstcodetemplete { get { return _lstcodetemplete; } }
            string dictcodetemplete { get { return _dictcodetemplete; } }
            public string DealStructOrString(Type type, FieldInfo field) {
                return string.Format(defaultcodetemplete, prefix, field.Name, GetFuncName(type));
            }
            public string DealEnum(Type type, FieldInfo field) {
                return string.Format(enumcodetemplete, prefix, field.Name, type.ToString().Replace("+", "."));
            }
            public string DealUserClass(Type type, FieldInfo field) {
                return string.Format(clscodetemplete, prefix, field.Name, GetFuncName(type));
            }
            public string DealArray(Type t, FieldInfo field) {
                return string.Format(arraycodetemplete, prefix, field.Name);
            }
            public string DealList(Type t, FieldInfo field) {
                return string.Format(lstcodetemplete, prefix, field.Name);
            }
            public string DealDic(Type t, FieldInfo field) {
                var tepl = dictcodetemplete;
                return string.Format(tepl, prefix, field.Name);
            }
        }
        static string GetTypeName(Type type) {
            return EditorCodeGenerator.GetTypeName(type);
        }
        static string GetFuncName(Type type) {
            return EditorCodeGenerator.GetFuncName(type);
        }
        public class FiledHandlerBinaryReader : FiledHandler {
            public FiledHandlerBinaryReader() {
                _defaultcodetemplete = @"{0}ret.{1} = Read{2}(fieldData,ref cursor);";
                _enumcodetemplete = @"{0}ret.{1} = ({2})ReadInt32(fieldData,ref cursor);";
                _clscodetemplete = @"{0}ret.{1} = Read{2}(fieldData,ref cursor);";
                _arraycodetemplete = @"{0}ret.{1} = ReadArray(fieldData,ref cursor,ret.{1});";
                _lstcodetemplete = @"{0}ret.{1} = ReadList(fieldData,ref cursor,ret.{1});";
                _dictcodetemplete = @"{0}ret.{1} = ReadDict(fieldData,ref cursor,ret.{1});";
            }
        }
        public class FiledHandlerBinaryWriter : FiledHandler {
            public FiledHandlerBinaryWriter() {
                _defaultcodetemplete = @"{0}Write{2}(stream ,val.{1});";
                _enumcodetemplete = @"{0}WriteInt32(stream ,(int)(val.{1}));";
                _clscodetemplete = @"{0}Write{2}(stream ,val.{1});";
                _arraycodetemplete = @"{0}WriteArray(stream ,val.{1});";
                _lstcodetemplete = @"{0}WriteList(stream ,val.{1});";
                _dictcodetemplete = @"{0}WriteDict(stream ,val.{1});";
            }
        }


        IFiledHandler[] filedHandlers;
        public TypeHandlerBinary() { filedHandlers = new IFiledHandler[] { new FiledHandlerBinaryReader(), new FiledHandlerBinaryWriter() }; }
        public IFiledHandler[] GetFiledHandlers() { return filedHandlers; }
        string codetempletestruct = @"
public static partial class BinarySerializer
{
    public static #ClsName Read#ClsFuncName(byte[] fieldData, ref int cursor)
    { 
        var ret = new #ClsName();
//#LOADCODE
        return ret;
    }
    public static void Write#ClsFuncName(System.IO.BinaryWriter stream, #ClsName val)
    {
//#SAVECODE
    }
}
";
        string codetempleteclass = @"
public static partial class BinarySerializer
{
    public static #ClsName Read#ClsFuncName(byte[] fieldData, ref int cursor)
    {         
        bool isNull = ReadBoolean(fieldData, ref cursor);
        if (isNull) return null;
        var ret = new #ClsName();
//#LOADCODE
        return ret;
    }
    public static void Write#ClsFuncName(System.IO.BinaryWriter stream, #ClsName val)
    {
        bool isNull = val == null;
        WriteBoolean(stream, isNull);
        if (isNull) return;
//#SAVECODE
    }
}
";
        public string DealType(Type t, List<string> filedsStrs) {
            var clsTypeName = GetTypeName(t);
            var clsFuncName = GetFuncName(t);
            var codetemplete = t.IsClass ? codetempleteclass : codetempletestruct;
            int idx = 0;
            var str = codetemplete
                .Replace("#ClsName", clsTypeName)
                .Replace("#ClsFuncName", clsFuncName)
                .Replace("//#LOADCODE", filedsStrs[idx++])
                .Replace("//#SAVECODE", filedsStrs[idx++])
                ;
            return str;
        }
    }

    #region Test
    public class TypeHandlerTestPrint : ITypeHandler {
        public class FiledHandler : IFiledHandler {
            string prefix = "    ";
            public string DealDic(Type t, FieldInfo field) {
                var types = t.GetGenericArguments();
                return string.Format("{0}Dict<{2},{3}> {1}", prefix, field.Name, types[0].ToString(), types[1].ToString());
            }
            public string DealList(Type t, FieldInfo field) {
                var type = t.GetGenericArguments()[0];
                return string.Format("{0}List<{2}> {1}", prefix, field.Name, type.ToString());
            }
            public string DealArray(Type t, FieldInfo field) {
                var type = t.GetElementType();
                return string.Format("{0}Array[{2}] {1}", prefix, field.Name, type.ToString());
            }
            public string DealUserClass(Type t, FieldInfo field) {
                return string.Format("{0}{1} {2}", prefix, t.ToString(), field.Name);
            }
            public string DealEnum(Type t, FieldInfo field) {
                return string.Format("{0}{1} {2}", prefix, t.ToString().Replace("+", "."), field.Name);
            }
            public string DealStructOrString(Type t, FieldInfo field) {
                return string.Format("{0}{1} {2}", prefix, t.ToString(), field.Name);
            }
        }

        IFiledHandler[] filedHandlers;
        public TypeHandlerTestPrint() { filedHandlers = new IFiledHandler[] { new FiledHandler() }; }
        public IFiledHandler[] GetFiledHandlers() { return filedHandlers; }
        public string DealType(Type t, List<string> filedsStrs) {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(t.ToString());
            foreach (var filedsStr in filedsStrs) {
                sb.AppendLine(filedsStr);
            }
            return sb.ToString();
        }
    }
    #endregion
}