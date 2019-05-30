using System;
using System.Collections.Generic;

namespace BinarySerializer {
   
    public class TypeHandlerBinary : ITypeHandler {
    
        public class FiledHandlerBinaryReader : FiledHandler {
            public FiledHandlerBinaryReader(ICodeHelper helper) :base(helper) {
                _defaultCodeTemplete = @"{0}ret.{1} = Read{2}(fieldData,ref cursor);";
                _enumCodeTemplete = @"{0}ret.{1} = ({2})ReadInt32(fieldData,ref cursor);";
                _clsCodeTemplete = @"{0}ret.{1} = Read{2}(fieldData,ref cursor);";
                _arrayCodeTemplete = @"{0}ret.{1} = ReadArray(fieldData,ref cursor,ret.{1});";
                _lstCodeTemplete = @"{0}ret.{1} = ReadList(fieldData,ref cursor,ret.{1});";
                _dictCodeTemplete = @"{0}ret.{1} = ReadDict(fieldData,ref cursor,ret.{1});";
            }
        }
        public class FiledHandlerBinaryWriter : FiledHandler {
            public FiledHandlerBinaryWriter(ICodeHelper helper) :base(helper){
                _defaultCodeTemplete = @"{0}Put(stream ,val.{1});";
                _enumCodeTemplete = @"{0}Put(stream ,(int)(val.{1}));";
                _clsCodeTemplete = @"{0}Put{2}(stream ,val.{1});";
                _arrayCodeTemplete = @"{0}PutArray(stream ,val.{1});";
                _lstCodeTemplete = @"{0}WriteList(stream ,val.{1});";
                _dictCodeTemplete = @"{0}WriteDict(stream ,val.{1});";
            }
        }


        IFiledHandler[] filedHandlers;
        private ICodeHelper helper;
        public TypeHandlerBinary(ICodeHelper helper){
            this.helper = helper;
            filedHandlers = new IFiledHandler[] {
                new FiledHandlerBinaryReader(helper),
                new FiledHandlerBinaryWriter(helper)
            };
        }

        public bool CanAddType(Type t){
            return true;
        }
        public IFiledHandler[] GetFiledHandlers() { return filedHandlers; }
        string CodeTempletestruct = @"
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
        string CodeTempleteclass = @"
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
            var clsTypeName = helper.GetTypeName(t);
            var clsFuncName = helper.GetFuncName(t);
            var CodeTemplete = t.IsClass ? CodeTempleteclass : CodeTempletestruct;
            int idx = 0;
            var str = CodeTemplete
                .Replace("#ClsName", clsTypeName)
                .Replace("#ClsFuncName", clsFuncName)
                .Replace("//#LOADCODE", filedsStrs[idx++])
                .Replace("//#SAVECODE", filedsStrs[idx++])
                ;
            return str;
        }
    }
}