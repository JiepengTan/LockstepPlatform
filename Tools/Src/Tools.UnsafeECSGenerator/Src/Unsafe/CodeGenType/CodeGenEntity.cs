using System;
using System.Reflection;
using System.Text;

namespace Lockstep.ECS.UnsafeECS.CodeGen {
    public class CodeGenEntity : CodeGenBase {
        protected virtual string ClsPrototype => @"
    [StructLayoutAttribute(LayoutKind.Sequential, Pack=4)]
    public unsafe partial struct #CLS_NAME :IEntity {
#ATTRIS_DEFINE
    }
";

        protected override bool FilterFunc(Type type){
            return typeof(ECDefine.IEntity).IsAssignableFrom(type);
        }


        protected string DefineStr = "public {0} {1};";
        protected StringBuilder _attriSb = new StringBuilder();

        protected override void DealType(Type type){
            var fields = type.GetFields();
            var strDefines = GetDefineString(fields);
            var typeStr = ClsPrototype
                    .Replace("#CLS_NAME", type.Name)
                    .Replace("#ATTRIS_DEFINE", strDefines)
                ;
            sb.AppendLine(typeStr);
        }

        protected string GetDefineString(FieldInfo[] fields){
            return IterateFields(fields,
                (field) => {
                    _attriSb.AppendLine(attriPrefix +
                                        string.Format(DefineStr, GetTypeName(field.FieldType), field.Name));
                });
        }

        protected string IterateFields(FieldInfo[] fileds, Action<FieldInfo> func){
            _attriSb.Clear();
            foreach (var field in fileds) {
                func(field);
            }

            return _attriSb.ToString();
        }
    }
}