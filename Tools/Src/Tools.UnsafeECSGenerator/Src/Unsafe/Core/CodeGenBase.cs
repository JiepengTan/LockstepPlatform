using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Lockstep.ECS.ECDefine;

namespace Lockstep.ECS.UnsafeECS.CodeGen {
    public class CodeGenBase {
        public Lockstep.ECSGenerator.ConfigInfo Info;
        public StringBuilder _sb;
        protected static string prefix = "    ";
        protected static string attriPrefix = prefix + "    ";

        protected void AppendLineType(string str){
            _sb.AppendLine(prefix + str);
        }

        protected void AppendLineAttri(string str){
            _sb.AppendLine(attriPrefix + str);
        }

        private Type[] _rawTypes;

        public virtual void GenCode(Type[] rawTypes){
            _rawTypes = rawTypes;
            OnBeforeGenCode(rawTypes);
            foreach (var type in rawTypes) {
                if (FilterFunc(type)) {
                    DealType(type);
                }
            }

            OnAfterGenCode(rawTypes);
        }

        protected virtual void OnBeforeGenCode(Type[] rawTypes){ }
        protected virtual void OnAfterGenCode(Type[] rawTypes){ }

        protected Type[] GetTypesEntity(){return GetTypes<IEntity>();}
        protected Type[] GetTypesAsset(){return GetTypes<IAsset>();}
        protected Type[] GetTypesComponent(){return GetTypes<IComponent>();}
        protected Type[] GetTypesEvent(){return GetTypes<IEvent>();}
        protected Type[] GetTypesEntityField(){return GetTypes<IEntity>()
            .Select( (t) => t.GetNestedTypes()[0]).ToArray();}
        Type[] GetTypes<T>(){return _rawTypes.Where(IsTargetType<T>).ToArray();}
        protected bool IsTargetType<T>(Type t){return t != typeof(T) && typeof(T).IsAssignableFrom(t);}

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
                return $"{type.Name.Replace("`1", "")}<{type.GenericTypeArguments[0].Name}>";
            }
            else {
                return type.Name;
            }
        }
    }
}