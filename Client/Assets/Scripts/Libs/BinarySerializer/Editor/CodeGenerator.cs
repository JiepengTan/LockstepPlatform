﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace BinarySerializer {
    public class CodeGenerator {
        const BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetField;
        readonly Queue<Type> todoTypes = new Queue<Type>();
        readonly HashSet<Type> todoTypesSet = new HashSet<Type>();
        readonly HashSet<Type> IgnoredTypes = new HashSet<Type>();
        readonly HashSet<Type> generatedTypes = new HashSet<Type>();
        public List<Type> AllGeneratedTypes = new List<Type>();

        private ITypeHandler typeHandler;
        public string GenTypeCode(ITypeHandler typeHandler, params Type[] types){
            
            this.typeHandler = typeHandler;
            foreach (var t in types) {
                AddType(t);
            }


            var type = GetNextType();
            StringBuilder sb = new StringBuilder();
            
            while (type != null) {
                var typeStr = GenTypeCode(type, typeHandler);
                sb.AppendLine(typeStr);
                type = GetNextType();
            }

            return sb.ToString();
        }

        Type GetNextType(){
            if (todoTypes.Count == 0) {
                return null;
            }
            else {
                var ret = todoTypes.Dequeue();
                AllGeneratedTypes.Add(ret);
                todoTypesSet.Remove(ret);
                return ret;
            }
        }

        public void AddIgnoredTypes(params Type[] types){
            foreach (var item in types) {
                IgnoredTypes.Add(item);
            }
        }

        void AddType(Type type){
            if (IgnoredTypes.Contains(type)) {
                Debug.Log("Try to serialize ignore type" + type);
                return;
            }
            if(!typeHandler.CanAddType(type)) return;

            if (!generatedTypes.Add(type)) {
                return;
            }

            if (todoTypesSet.Add(type)) {
                todoTypes.Enqueue(type);
            }
        }

        string GenTypeCode(Type type, ITypeHandler typeHandler){
            var fileds = FilterFields(type.GetFields(bindingAttr));
            IFiledHandler[] Handlers = typeHandler.GetFiledHandlers();
            List<string> sbfs = new List<string>();
            foreach (var Handler in Handlers) {
                var sbf = GetFiledInfo(fileds, Handler);
                sbfs.Add(sbf);
            }

            return typeHandler.DealType(type, sbfs);
        }

        public List<FieldInfo> FilterFields(FieldInfo[] fields){
            List<FieldInfo> retfileds = new List<FieldInfo>();
            for (int i = 0; i < fields.Length; i++) {
                var field = fields[i];
                if (field.IsStatic) {
                    continue;
                }

                var noBytesAttris = field.GetCustomAttributes(typeof(NoToBinaryAttribute), true);
                if (noBytesAttris != null && noBytesAttris.Length > 0) {
                    continue;
                }

                retfileds.Add(field);
            }

            //属性排序
            retfileds.Sort((a, b) => {
                var ta = a.FieldType;
                var tb = b.FieldType;

                //泛型在后面
                var ga = ta.IsGenericType;
                var gb = tb.IsGenericType;
                if (ga != gb) {
                    return ga ? 1 : -1;
                }

                //array在后面
                var aa = ta.IsArray;
                var ab = tb.IsArray;
                if (aa != ab) {
                    return aa ? 1 : -1;
                }

                //用户自定义的在后面
                var ua = IsUserDefineClass(ta);
                var ub = IsUserDefineClass(tb);
                if (ua != ub) {
                    return ua ? 1 : -1;
                }

                //enum 在后面
                var ea = ta.IsEnum;
                var eb = tb.IsEnum;
                if (ea != eb) {
                    return ea ? 1 : -1;
                }

                return String.CompareOrdinal(a.Name, b.Name);
            });
            return retfileds;
        }

        string GetFiledInfo(List<FieldInfo> fileds, IFiledHandler Handler){
            StringBuilder sb = new StringBuilder();
            int i = 0;
            var count = fileds.Count;
            Action<string> AppendString = (string str) => {
                if (i == count - 1)
                    sb.Append(str);
                else
                    sb.AppendLine(str);
            };
            for (; i < count; i++) {
                var field = fileds[i];
                var ty = field.FieldType;
                if (ty.IsGenericType) {
                    var argus = ty.GetGenericArguments();
                    foreach (var arg in argus) {
                        if (IsUserDefineClass(arg)) {
                            AddType(arg);
                        }
                    }

                    string str = "";
                    if (IsList(ty)) {
                        str = Handler.DealList(ty, field);
                    }
                    else if (IsDict(ty)) {
                        str = Handler.DealDic(ty, field);
                    }

                    AppendString(str);
                }
                else if (IsArray(ty)) {
                    var paramT = ty.GetElementType();
                    if (IsUserDefineClass(paramT)) {
                        AddType(paramT);
                    }

                    string str = Handler.DealArray(ty, field);
                    AppendString(str);
                }
                else if (IsUserDefineClass(ty)) {
                    AddType(ty);
                    string str = Handler.DealUserClass(ty, field);
                    AppendString(str);
                }
                else if (ty.IsEnum) {
                    string str = Handler.DealEnum(ty, field);
                    AppendString(str);
                }
                else {
                    //structs
                    var str = Handler.DealStructOrString(ty, field);
                    AppendString(str);
                }
            }

            return sb.ToString();
        }

        public static bool IsUserDefineClass(Type type){
            return type.IsClass && !type.IsGenericType && !type.IsArray && type != typeof(string);
        }

        public static bool IsList(Type t){
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>)) {
                return true;
            }

            return false;
        }

        public static bool IsArray(Type t){
            if (t.IsArray) {
                return true;
            }

            return false;
        }

        public static bool IsDict(Type t){
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
                return true;
            }

            return false;
        }
    }
}