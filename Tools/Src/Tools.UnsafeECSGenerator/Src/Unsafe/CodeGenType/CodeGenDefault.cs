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

        public override void GenCode(Type[] rawTypes){
            GenBitSets();

            AppendDefaultCode();
        }


        private void GenBitSets(){
            var allSize = CodeGenBitSet.GetAllSize();
            for (int i = 0; i < allSize.Length; i++) {
                var size = allSize[i];
                var name = "BitSet" + size.ToString();
                var postfix = i == allSize.Length - 1 ? "" : "\n";
                _builder.Append(attriPrefix + $"public static {name} {name}; {postfix}");
            }
        }

        void AppendDefaultCode(){
            _sb.AppendLine(ClsPrototype.Replace("#TYPE_DEFINE", _builder.ToString()));
        }
    }
}