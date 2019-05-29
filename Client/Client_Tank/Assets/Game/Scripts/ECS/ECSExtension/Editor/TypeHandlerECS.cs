using System;
using System.Collections.Generic;
using Entitas;

namespace BinarySerializer {
    
    public interface ICodeHelper {
        string prefix { get; }
        string GetNameSpace(Type type);
        string GetTypeName(Type type,bool isWithNameSpaceIfNeed = true);
        string GetFuncName(Type type,bool isWithNameSpaceIfNeed = true);
    }
    
    public class TypeHandlerECS : ITypeHandler {
        public class HandlerCopyTo : FiledHandler {
            public HandlerCopyTo(ICodeHelper helper):base(helper){
                _defaultCodeTemplete = @"{0}dst.{1} = {1};";
                _enumCodeTemplete = @"{0}dst.{1} = {1};";
                _clsCodeTemplete = @"{0}dst.{1} = {1};"; //for error
            }

            //ret.lvec2 = ReadLVector2(fieldData,ref cursor);
        }

        public class HandlerEquals : FiledHandler {
            public HandlerEquals(ICodeHelper helper):base(helper){
                _defaultCodeTemplete = @"{0}if ({1} != dst.{1}) return false;";
                _enumCodeTemplete = @"{0}if ({1} != dst.{1}) return false;";
                _clsCodeTemplete = @"{0}if ({1} != dst.{1}) return false;";
            }
        }

        public class HandlerToString : FiledHandler {
            public HandlerToString(ICodeHelper helper):base(helper){
                _defaultCodeTemplete = @"{0}""{1}="" + {1} +";
                _enumCodeTemplete = @"{0}""{1}="" + {1} +";
                _clsCodeTemplete = @"{0}""{1}="" + {1} +";
            }
        }

        IFiledHandler[] filedHandlers;
        private ICodeHelper helper;
        public TypeHandlerECS(ICodeHelper helper){
            this.helper = helper;
            filedHandlers = new IFiledHandler[] {
                new HandlerCopyTo(helper),
                new HandlerEquals(helper),
                new HandlerToString(helper)
            };
        }

        public IFiledHandler[] GetFiledHandlers(){
            return filedHandlers;
        }
        
        string clsCodeTemplate = @"
namespace #NameSpace{
    public partial class #ClsName  {
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
        
        public override bool Equals(object obj){
            var dst = (#ClsName) obj;
                if (dst == null) {
                return false;
            }
//#EUQALS
            return true;
        }
        
        public override string ToString(){
            return ""#CompName{"" +
//#TOSTRING
            ""}"";
            ;
        }
    }
}
";
        public bool CanAddType(Type t){
            return typeof(IComponent).IsAssignableFrom(t);
        }

        public string DealType(Type t, List<string> filedsStrs){
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
                    .Replace("//#COPYTO", filedsStrs[idx++])
                    .Replace("//#EUQALS", filedsStrs[idx++])
                    .Replace("//#TOSTRING", filedsStrs[idx++])
                ;
            return str;
        }
    }
}