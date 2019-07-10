using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Lockstep.ECS.UnsafeECS.CodeGen {
    public class CodeGenBase {
        public Lockstep.ECSGenerator.ConfigInfo Info;
        public StringBuilder sb;
        protected static string prefix = "    ";
        protected static string attriPrefix = prefix + "    ";

        protected void AppendLineType(string str){
            sb.AppendLine(prefix + str);
        }

        protected void AppendLineAttri(string str){
            sb.AppendLine(attriPrefix + str);
        }

        public virtual void GenCode(Type[] rawtypes){
            foreach (var type in rawtypes) {
                if (FilterFunc(type)) {
                    DealType(type);
                }
            }
        }

        protected virtual bool FilterFunc(Type type){
            return true;
        }

        protected virtual void DealType(Type type){ }

        public static Dictionary<Type, string> type2Str = new Dictionary<Type, string>() {
            {typeof(bool), "bool"},
            {typeof(string), "string"},
            {typeof(float), "LFloat"},
            {typeof(byte), "byte"},
            {typeof(sbyte), "sbyte"},
            {typeof(short), "short"},
            {typeof(ushort), "ushort"},
            {typeof(int), "int"},
            {typeof(uint), "uint"},
            {typeof(long), "long"},
            {typeof(ulong), "ulong"},
            {typeof(Lockstep.ECS.ECDefine.Vector2), "LVector2"},
            {typeof(Lockstep.ECS.ECDefine.Vector3), "LVector3"},
            {typeof(Lockstep.ECS.ECDefine.Quaternion), "LQuaternion"},
        };

        protected static string GetTypeName(Type type){
            if (type2Str.TryGetValue(type, out string val)) {
                return val;
            }

            if (type.IsGenericType) {
                return $"{type.Name.Replace("`1","") }<{type.GenericTypeArguments[0].Name}>";
            }
            else {
                return type.Name;    
            }
        }
    }
}