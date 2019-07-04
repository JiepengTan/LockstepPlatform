using System;
using System.Collections.Generic;
using Lockstep.Serialization;
using NetMsg.Common;

namespace Lockstep.CodeGenerator {
    
    public class TypeHandlerMsg: ITypeHandler {
        public class HandlerDeserialize : FiledHandler {
            public HandlerDeserialize(ICodeHelper helper):base(helper){
                _defaultCodeTemplete = @"{0}{1} = reader.Read{2}();";
                _enumCodeTemplete = @"{0}{1} = ({2})reader.ReadInt32();";
                _clsCodeTemplete = @"{0}{1} = reader.ReadRef(ref this.{1});";
                _arrayCodeTemplete = @"{0}{1} = reader.ReadArray(this.{1});";
                _lstCodeTemplete = @"{0}{1} = reader.ReadList(this.{1});";
                _dictCodeTemplete = @"{0}{1} = reader.ReadDict(this.{1});";
            }
        }

        public class HandlerSerialize : FiledHandler {
            public HandlerSerialize(ICodeHelper helper):base(helper){
                _defaultCodeTemplete = @"{0}writer.Write({1});";
                _enumCodeTemplete = @"{0}writer.Write((int)({1}));";
                _clsCodeTemplete = @"{0}writer.Write({1});";
                _arrayCodeTemplete = @"{0}writer.Write({1});";
                _lstCodeTemplete = @"{0}writer.Write({1});";
                _dictCodeTemplete = @"{0}writer.Write({1});";
            }
        }

        protected List<IFiledHandler> filedHandlers = new List<IFiledHandler>() ;
        protected ICodeHelper helper;

        public IFiledHandler[] GetFiledHandlers(){
            return filedHandlers.ToArray();
        }

        public TypeHandlerMsg(ICodeHelper helper){
            this.helper = helper;
            filedHandlers.Add(new HandlerSerialize(helper));
            filedHandlers.Add(new HandlerDeserialize(helper));
        }
        
        string clsCodeTemplate = @"
    [System.Serializable]
    public partial class #ClsName{
        public override void Serialize(Serializer writer){
//#SERIALIZER
        }
    
        public override void Deserialize(Deserializer reader){
//#DESERIALIZER
        }
    }
";
        public virtual bool CanAddType(Type t){
            return typeof(BaseMsg).IsAssignableFrom(t);
        }

        public virtual string DealType(Type t, List<string> filedsStrs){
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