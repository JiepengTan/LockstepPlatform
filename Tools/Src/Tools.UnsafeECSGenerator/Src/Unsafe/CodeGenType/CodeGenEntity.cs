using System;
using System.Reflection;
using System.Text;

namespace Lockstep.ECS.UnsafeECS.CodeGen {
    public class CodeGenEntity : CodeGenBase {
        protected StringBuilder _attriSb = new StringBuilder();

        protected virtual string ClsPrototype => @"
    [StructLayoutAttribute(LayoutKind.Sequential, Pack=4)]
    public unsafe partial struct #CLS_NAME :IEntity {
#ATTRIS_DEFINE
    }
";

        protected virtual string DefineStr => "public {0} {1};";

        protected override bool FilterFunc(Type type){
            return typeof(ECDefine.IEntity).IsAssignableFrom(type);
        }


        protected override void DealType(Type type){
            var fields = type.GetFields();
            var strDefines = GetDefineString(fields);
            var typeStr = ClsPrototype
                    .Replace("#CLS_NAME", type.Name)
                    .Replace("#ATTRIS_DEFINE", strDefines)
                ;
            _sb.AppendLine(typeStr);
        }

        protected virtual string GetDefineString(FieldInfo[] fields){
            return IterateFields(fields,
                (field) => {
                    AppendAttri(attriPrefix + string.Format(DefineStr, GetTypeName(field.FieldType), field.Name));
                });
        }

        private bool _isNeedLine = false;

        protected string IterateFields(FieldInfo[] fileds, Action<FieldInfo> func){
            _attriSb.Clear();
            _isNeedLine = true;
            for (int index = 0; index < fileds.Length; index++) {
                var field = fileds[index];
                if (index == fileds.Length - 1) {
                    _isNeedLine = false;
                }
                func(field);
            }
            return _attriSb.ToString();
        }

        protected void AppendAttri(string info){
            if (_isNeedLine) {
                _attriSb.AppendLine(info);
            }
            else {
                _attriSb.Append(info);
            }
        }
    }
}