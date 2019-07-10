using System;
using System.Linq;
using System.Reflection;
using System.Text;
using static Lockstep.Logging.Debug;

namespace Lockstep.ECS.UnsafeECS.CodeGen {
    public class CodeGenEnum : CodeGenBase {
        protected override bool FilterFunc(Type type){
            return type.IsEnum;
        }
        protected override void DealType(Type type){
            AppendLineType($"public enum {type.Name}: int {{");
            var fileds = type.GetFields();
            foreach (var filed in fileds) {
                if (filed.FieldType.IsEnum) {
                    var name = filed.Name;
                    var val = (int) filed.GetValue(null);
                    AppendLineAttri($"{name} = {val},");
                }
            }
            AppendLineType($"}}");
        }
    }
}