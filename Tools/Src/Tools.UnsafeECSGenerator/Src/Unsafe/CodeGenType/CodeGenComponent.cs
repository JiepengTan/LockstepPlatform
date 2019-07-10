using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Lockstep.Logging;

namespace Lockstep.ECS.UnsafeECS.CodeGen {
    public class CodeGenComponent : CodeGenEntity {
        private int[] Primers;
        private int curPrimerIdx = 13;
        static int[] CalcPrimers(int size){
            var primers = new int[size];
            primers[0] = 2;
            int k = 1;
            for (int i = 3; k < size; i++) { //枚举每个数
                bool ok = true;
                for (int j = 0; j < k; j++) { //枚举已经得到的质数
                    if (i % primers[j] == 0) {
                        ok = false;
                        break;
                    }
                }

                if (ok) {
                    primers[k] = i;
                    k++;
                }
            }

            return primers;
        }

        public override void GenCode(Type[] rawtypes){
            Primers = CalcPrimers(200);
            base.GenCode(rawtypes);
        }

        protected override string ClsPrototype => @"
    [StructLayoutAttribute(LayoutKind.Sequential, Pack=4)]
    public unsafe partial struct #CLS_NAME {
#ATTRIS_DEFINE
        public override Int32 GetHashCode() {
            unchecked {
                var hash = #PRIMER;
#HASH_CODES
                return hash;
            }
        }
    }
";

        protected override bool FilterFunc(Type type){
            return typeof(ECDefine.IComponent).IsAssignableFrom(type);
        }


        protected override void DealType(Type type){
            var fields = type.GetFields();
            var strDefines = GetDefineString(fields);
            var strHashCodes = GetHashCodeString(fields);
            var typeStr = ClsPrototype
                    .Replace("#CLS_NAME", type.Name)
                    .Replace("#PRIMER", Primers[curPrimerIdx++].ToString())
                    .Replace("#ATTRIS_DEFINE", strDefines)
                    .Replace("#HASH_CODES", strHashCodes)
                ;
            sb.AppendLine(typeStr);
        }

        private string HashStr = " hash = hash * 31 +{0}.GetHashCode();";
        private string hashPrefix = attriPrefix + "        ";
        string GetHashCodeString(FieldInfo[] fields){
            return IterateFields(fields,
                (field) => { _attriSb.AppendLine(hashPrefix + string.Format(HashStr, field.Name)); });
        }
    }
}