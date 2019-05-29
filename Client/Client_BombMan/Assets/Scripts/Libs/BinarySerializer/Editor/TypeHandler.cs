using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Collections;

namespace BinarySerializer {
    public interface ITypeHandler {
        bool CanAddType(Type t);
        string DealType(Type t, List<string> sbfs);
        IFiledHandler[] GetFiledHandlers();
    }

    public interface IFiledHandler {
        string DealDic(Type t, FieldInfo field);
        string DealList(Type t, FieldInfo field);
        string DealArray(Type t, FieldInfo field);
        string DealUserClass(Type t, FieldInfo field);
        string DealEnum(Type t, FieldInfo field);
        string DealStructOrString(Type t, FieldInfo field);
    }

    public class FiledHandler : IFiledHandler {
        string prefix;
        protected ICodeHelper helper;

        public FiledHandler(ICodeHelper helper){
            this.helper = helper;
            prefix = helper.prefix;
        }

        protected string _defaultCodeTemplete;
        protected string _enumCodeTemplete;
        protected string _clsCodeTemplete;
        protected string _arrayCodeTemplete;
        protected string _lstCodeTemplete;
        protected string _dictCodeTemplete;

        string defaultCodeTemplete {
            get { return _defaultCodeTemplete; }
        }

        string enumCodeTemplete {
            get { return _enumCodeTemplete; }
        }

        string clsCodeTemplete {
            get { return _clsCodeTemplete; }
        }

        string arrayCodeTemplete {
            get { return _arrayCodeTemplete; }
        }

        string lstCodeTemplete {
            get { return _lstCodeTemplete; }
        }

        string dictCodeTemplete {
            get { return _dictCodeTemplete; }
        }

        public string DealStructOrString(Type type, FieldInfo field){
            return string.Format(defaultCodeTemplete, prefix, field.Name, GetFuncName(type));
        }

        public string DealEnum(Type type, FieldInfo field){
            return string.Format(enumCodeTemplete, prefix, field.Name, type.ToString().Replace("+", "."));
        }

        public string DealUserClass(Type type, FieldInfo field){
            return string.Format(clsCodeTemplete, prefix, field.Name, GetFuncName(type));
        }

        public string DealArray(Type t, FieldInfo field){
            return string.Format(arrayCodeTemplete, prefix, field.Name);
        }

        public string DealList(Type t, FieldInfo field){
            return string.Format(lstCodeTemplete, prefix, field.Name);
        }

        public string DealDic(Type t, FieldInfo field){
            var tepl = dictCodeTemplete;
            return string.Format(tepl, prefix, field.Name);
        }

        public string GetTypeName(Type type){
            return helper.GetTypeName(type);
        }

        public string GetFuncName(Type type){
            return helper.GetFuncName(type);
        }
    }

    #region Test

    public class TypeHandlerTestPrint : ITypeHandler {
        public class FiledHandler : IFiledHandler {
            string prefix = "    ";

            public string DealDic(Type t, FieldInfo field){
                var types = t.GetGenericArguments();
                return string.Format("{0}Dict<{2},{3}> {1}", prefix, field.Name, types[0].ToString(),
                    types[1].ToString());
            }

            public string DealList(Type t, FieldInfo field){
                var type = t.GetGenericArguments()[0];
                return string.Format("{0}List<{2}> {1}", prefix, field.Name, type.ToString());
            }

            public string DealArray(Type t, FieldInfo field){
                var type = t.GetElementType();
                return string.Format("{0}Array[{2}] {1}", prefix, field.Name, type.ToString());
            }

            public string DealUserClass(Type t, FieldInfo field){
                return string.Format("{0}{1} {2}", prefix, t.ToString(), field.Name);
            }

            public string DealEnum(Type t, FieldInfo field){
                return string.Format("{0}{1} {2}", prefix, t.ToString().Replace("+", "."), field.Name);
            }

            public string DealStructOrString(Type t, FieldInfo field){
                return string.Format("{0}{1} {2}", prefix, t.ToString(), field.Name);
            }
        }

        IFiledHandler[] filedHandlers;

        public TypeHandlerTestPrint(){
            filedHandlers = new IFiledHandler[] {new FiledHandler()};
        }

        public IFiledHandler[] GetFiledHandlers(){
            return filedHandlers;
        }

        public bool CanAddType(Type t){
            return true;
        }
        public string DealType(Type t, List<string> filedsStrs){
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(t.ToString());
            foreach (var filedsStr in filedsStrs) {
                sb.AppendLine(filedsStr);
            }

            return sb.ToString();
        }
    }

    #endregion
}