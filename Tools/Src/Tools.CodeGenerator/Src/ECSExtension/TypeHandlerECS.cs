using System;
using System.Collections.Generic;
using Entitas;

namespace Lockstep.CodeGenerator {
    public class TypeHandlerECS : TypeHandlerMsg, ITypeHandler {
        public class HandlerCopyTo : FiledHandler {
            public HandlerCopyTo(ICodeHelper helper) : base(helper){
                _defaultCodeTemplete = @"{0}dst.{1} = {1};";
                _enumCodeTemplete = @"{0}dst.{1} = {1};";
                _clsCodeTemplete = @"{0}dst.{1} = {1};"; //for error
            }

            //ret.lvec2 = ReadLVector2(fieldData,ref cursor);
        }

        public class HandlerEquals : FiledHandler {
            public HandlerEquals(ICodeHelper helper) : base(helper){
                _defaultCodeTemplete = @"{0}if ({1} != dst.{1}) return false;";
                _enumCodeTemplete = @"{0}if ({1} != dst.{1}) return false;";
                _clsCodeTemplete = @"{0}if ({1} != dst.{1}) return false;";
            }
        }

        public class HandlerToString : FiledHandler {
            public HandlerToString(ICodeHelper helper) : base(helper){
                _defaultCodeTemplete = @"{0}""{1}="" + {1} +";
                _enumCodeTemplete = @"{0}""{1}="" + {1} +";
                _clsCodeTemplete = @"{0}""{1}="" + {1} +";
            }
        }


        public TypeHandlerECS(ICodeHelper helper)
            : base(helper){
            filedHandlers.Add(new HandlerCopyTo(helper));
            filedHandlers.Add(new HandlerEquals(helper));
            filedHandlers.Add(new HandlerToString(helper));
        }


        string clsCodeTemplate = @"
namespace #NameSpace{                                                                                                                                                                                                                       
    [System.Serializable]                                                                                                                           
    public partial class #ClsName  {                                                                                                                            
        public override void Serialize(Serializer writer){                                                                                                                          
//#SERIALIZER                                                                                                                           
        }                                                                                                                           
                                                                                                                                
        public override void Deserialize(Deserializer reader){                                                                                                                          
//#DESERIALIZER                                                                                                                         
        }                                                                                                                           
                                                                                                                            
        public override void CopyTo(object comp){                                                                                                                           
            var dst = (#ClsName) comp;                                                                                                                          
            if (dst == null) {                                                                                                                          
                throw new CopyToUnExceptTypeException(comp == null ? ""null"" : comp.GetType().ToString());                                                                                                                         
            }                                                                                                                           
//#COPYTO                                                                                                                           
        }                                                                                                                           
                                                                                                                                    
        public override object Clone(){                                                                                                                         
            var dst = new #ClsName();                                                                                                                           
            CopyTo(dst);                                                                                                                            
            return dst;                                                                                                                         
        }                                                                                                                           
                                                                                                                                    
        public override int GetHashCode(){return base.GetHashCode();}                                                                                                                           
        public override bool Equals(object obj){                                                                                                                            
            var dst = (#ClsName) obj;                                                                                                                           
            if (dst == null) return false;                                                                                                                          
//#EUQALS                                                                                                                           
            return true;                                                                                                                            
        }                                                                                                                           
    }                                                                                                                           
}                                                                                                                           
";                                                                                                                          
                                                                                                                            
        public override bool CanAddType(Type t){
            return typeof(IComponent).IsAssignableFrom(t);
        }

        public override string DealType(Type t, List<string> filedsStrs){
            var nameSpace = helper.GetNameSpace(t);
            var clsTypeName = helper.GetTypeName(t);
            var compName = clsTypeName.Replace("Component", "");
            var clsFuncName = helper.GetFuncName(t);
            var codeTemplate = clsCodeTemplate;
            int idx = 0;
            var str = codeTemplate
                    .Replace("#NameSpace", nameSpace)
                    .Replace("#ClsName", clsTypeName)
                    .Replace("#CompName", compName)
                    .Replace("#ClsFuncName", clsFuncName)
                    .Replace("//#SERIALIZER", filedsStrs[idx++])
                    .Replace("//#DESERIALIZER", filedsStrs[idx++])
                    .Replace("//#COPYTO", filedsStrs[idx++])
                    .Replace("//#EUQALS", filedsStrs[idx++])
                    .Replace("//#TOSTRING", filedsStrs[idx++])
                ;
            return str;
        }
    }
}