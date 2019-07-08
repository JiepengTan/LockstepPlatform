using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Lockstep.Logging;
using Lockstep.Util;
using NetMsg.Server;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lockstep.CodeGenerator {
    public class EditorBaseCodeGenerator : ICodeHelper {
        public GenInfo GenInfo;
        protected HashSet<Type> togenCodeTypes = new HashSet<Type>();
        protected HashSet<Type> needNameSpaceTypes = new HashSet<Type>();

        public Type[] GetTypes(){
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GenInfo.DllRelPath);
            var assembly =  Assembly.LoadFrom(path);
            return assembly.GetTypes();
        }

        public string NameSpace => GenInfo.NameSpace;
        
        protected string GeneratePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, GenInfo.GeneratePath);

        protected string GenerateFilePath => Path.Combine(GeneratePath, GenInfo.GenerateFileName);


        protected virtual void CustomRegisterTypes(){ }

        protected virtual void ReflectRegisterTypes(){ }

        public string GenTypeCode(CodeGenerator gen, ITypeHandler typeHandler, params Type[] types){
            List<Type> allTypes = new List<Type>();
            allTypes.AddRange(types);
            var registerTypes = GetNeedSerilizeTypes();
            allTypes.AddRange(registerTypes);
            return gen.GenTypeCode(typeHandler, allTypes.ToArray());
        }

        public void HideGenerateCodes(bool isSave = true){
            var path = GenerateFilePath;
            var lines = System.IO.File.ReadAllLines(path);
            lines[0] = lines[0].Replace("//#define", "#define");
            System.IO.File.WriteAllLines(path, lines);
            if (isSave) {
#if UNITY_EDITOR
                AssetDatabase.ImportAsset(path);
                AssetDatabase.Refresh();
                UnityEngine.Debug.Log("Done");
#endif
            }
        }

        protected void SaveFile(bool isRefresh, string finalStr){ //save to file
            //Debug.LogError(GeneratePath);
            if (!Directory.Exists(GeneratePath)) {
                Directory.CreateDirectory(GeneratePath);
            }

            System.IO.File.WriteAllText(GenerateFilePath, finalStr);
            if (isRefresh) {
#if UNITY_EDITOR
                //EditorUtility.OpenWithDefaultApp(GenerateFilePath);
                AssetDatabase.Refresh();
                UnityEngine.Debug.Log("Done");
#endif
            }
        }

        protected virtual Type[] GetNeedSerilizeTypes(){
            needNameSpaceTypes.Clear();
            togenCodeTypes.Clear();
            CustomRegisterTypes();
            ReflectRegisterTypes();
            var list = togenCodeTypes.ToList();
            list.Sort((a, b) => { return a.Name.CompareTo(b.Name); });
            return list.ToArray();
        }


        protected void RegisterType(Type type){
            togenCodeTypes.Add(type);
        }

        protected void RegisterBaseType(Type type){
            var types = ReflectionUtility.GetSubTypes(type);
            foreach (var t in types) {
                RegisterType(t);
            }
        }

        protected void RegisterTypeWithNamespace(Type type){
            needNameSpaceTypes.Add(type);
        }

        protected void RegisterBaseTypeWithNamespace(Type type){
            var types = ReflectionUtility.GetSubTypes(type);
            foreach (var t in types) {
                RegisterTypeWithNamespace(t);
            }
        }


        protected bool IsNeedNameSpace(Type t){
            return needNameSpaceTypes.Contains(t);
        }

        public virtual string prefix {
            get { return "\t\t"; }
        }

        public string GetNameSpace(Type type){
            return type.Namespace;
        }

        public string GetTypeName(Type type, bool isWithNameSpaceIfNeed = true){
            var str = type.ToString();
            if (IsNeedNameSpace(type)) {
                return str.Replace("+", ".");
            }
            else {
                return str.Substring(str.LastIndexOf(".") + 1).Replace("+", ".");
            }
        }

        public string GetFuncName(Type type, bool isWithNameSpaceIfNeed = true){
            var str = type.ToString();
            if (IsNeedNameSpace(type)) {
                return str.Replace(".", "").Replace("+", "");
            }
            else {
                return str.Substring(str.LastIndexOf(".") + 1).Replace("+", "");
            }
        }
    }
};