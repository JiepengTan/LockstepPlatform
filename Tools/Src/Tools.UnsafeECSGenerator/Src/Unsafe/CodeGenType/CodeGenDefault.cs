using System;
using System.Collections.Generic;
using System.Text;
using Lockstep.ECS.ECDefine;

namespace Lockstep.ECS.UnsafeECS.CodeGen {
    public class CodeGenDefault : CodeGenBase {
        private StringBuilder _builder = new StringBuilder();
        private string ClsPrototype = @"
    public unsafe partial struct __default {
#TYPE_DEFINE
    }
";

        public override void GenCode(Type[] rawtypes){
            GenBitSets();

            AppendDefaultCode();
        }
        
        
        private void GenBitSets(){
            foreach (var size in CodeGenBitSet.GetAllSize()) {
                var name = "BitSet" + size.ToString();
                _builder.AppendLine(attriPrefix + $"public static {name} {name};");
            }
        }

        void AppendDefaultCode(){
            sb.AppendLine(ClsPrototype.Replace("#TYPE_DEFINE", _builder.ToString()));
        }
    }
}