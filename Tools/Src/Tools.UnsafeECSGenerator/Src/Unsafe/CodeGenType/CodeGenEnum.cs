using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Lockstep.ECS.ECDefine;
using static Lockstep.Logging.Debug;

namespace Lockstep.ECS.UnsafeECS.CodeGen {
    public class CodeGenEnum : CodeGenEntity {
        protected override string ClsPrototype => @"
    public enum  #CLS_NAME : int {
#ATTRIS_DEFINE
    }
";

        protected override void OnBeforeGenCode(Type[] rawTypes){
            //Primers = CalcPrimers(200);
            var types = GetTypesEntity();
            StringBuilder tsb = new StringBuilder();
            for (int i = 0; i < types.Length; i++) {
                var type = types[i];
                var postfix = i == types.Length - 1 ? "" : "\n";
                tsb.Append(attriPrefix + $"{type.Name} = {i}, {postfix}");
            }
            var typeStr = ClsPrototype
                    .Replace("#CLS_NAME", "EEntityTypes")
                    .Replace("#ATTRIS_DEFINE", tsb.ToString())
                ;
            _sb.AppendLine(typeStr);
        }

        protected override bool FilterFunc(Type type){
            return type.IsEnum;
        }

        protected override string GetDefineString(FieldInfo[] fields){
            return IterateFields(fields,
                (field) => {
                    if (!field.FieldType.IsEnum) return;
                    AppendAttri(attriPrefix + $"{field.Name} = {(int) field.GetValue(null)},");
                });
        }
    }
}