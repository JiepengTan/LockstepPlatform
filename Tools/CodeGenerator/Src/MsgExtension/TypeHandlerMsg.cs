using System;
using System.Collections.Generic;
using Lockstep.Serialization;

namespace Lockstep.CodeGenerator {
    
    
    public class TypeHandlerMsg: ITypeHandler {
        public class HandlerReader : FiledHandler {
            public HandlerReader(ICodeHelper helper):base(helper){
                _defaultCodeTemplete = @"{0}{1} = reader.Get{2}();";
                _enumCodeTemplete = @"{0}{1} = ({2})reader.GetInt32();";
                _clsCodeTemplete = @"{0}{1} = reader.Get(ref this.{1});";
                _arrayCodeTemplete = @"{0}{1} = reader.GetArray(this.{1});";
                _lstCodeTemplete = @"{0}{1} = reader.GetList(this.{1});";
                _dictCodeTemplete = @"{0}{1} = reader.GetDict(this.{1});";
            }
        }

        public class HandlerWriter : FiledHandler {
            public HandlerWriter(ICodeHelper helper):base(helper){
                _defaultCodeTemplete = @"{0}writer.Put{2}({1});";
                _enumCodeTemplete = @"{0}writer.PutInt32((int)({1}));";
                _clsCodeTemplete = @"{0}writer.Put({1});";
                _arrayCodeTemplete = @"{0}writer.PutArray({1});";
                _lstCodeTemplete = @"{0}writer.PutList({1});";
                _dictCodeTemplete = @"{0}writer.PutDict({1});";
            }
        }

        IFiledHandler[] filedHandlers;
        private ICodeHelper helper;
        public TypeHandlerMsg(ICodeHelper helper){
            this.helper = helper;
            filedHandlers = new IFiledHandler[] {
                new HandlerWriter(helper),
                new HandlerReader(helper),
            };
        }

        public IFiledHandler[] GetFiledHandlers(){
            return filedHandlers;
        }

        
        string clsCodeTemplate = @"
    public partial class #ClsName{
        public override void Serialize(Serializer writer){
//#SERIALIZER
        }
    
        public override void Deserialize(Deserializer reader){
//#DESERIALIZER
        }
    }
";
        public bool CanAddType(Type t){
            return typeof(BaseFormater).IsAssignableFrom(t);
        }

        public string DealType(Type t, List<string> filedsStrs){
            var nameSpace = helper.GetNameSpace(t);
            var clsTypeName = helper.GetTypeName(t);
            var codeTemplate = clsCodeTemplate;
            int idx = 0;
            var str = codeTemplate
                    .Replace("#NameSpace", nameSpace)
                    .Replace("#ClsName", clsTypeName)
                    .Replace("//#SERIALIZER", filedsStrs[idx++])
                    .Replace("//#DESERIALIZER", filedsStrs[idx++])
                ;
            return str;
        }
    }
}