using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace BinarySerializer {
    public class EditorBaseCodeGenerator : ICodeHelper {
        protected HashSet<Type> togenCodeTypes = new HashSet<Type>();
        protected HashSet<Type> needNameSpaceTypes = new HashSet<Type>();

        protected virtual string GeneratePath {
            get { return ""; }
        }

        protected virtual string GenerateFilePath {
            get { return ""; }
        }


        protected virtual void CustomRegisterTypes(){ }

        protected virtual void ReflectRegisterTypes(){ }

        public string GenTypeCode(CodeGenerator gen, ITypeHandler typeHandler, params Type[] types){
            List<Type> allTypes = new List<Type>();
            allTypes.AddRange(types);
            var registerTypes = GetNeedSerilizeTypes();
            allTypes.AddRange(registerTypes);
            return gen.GenTypeCode(typeHandler, allTypes.ToArray());
        }

        protected void HideGenerateCodes(bool isSave = true){
            var path = GenerateFilePath;
            var lines = System.IO.File.ReadAllLines(path);
            lines[0] = lines[0].Replace("//#define", "#define");
            System.IO.File.WriteAllLines(path, lines);
            if (isSave) {
                AssetDatabase.ImportAsset(path);
                AssetDatabase.Refresh();
                UnityEngine.Debug.Log("Done");
            }
        }

        protected void SaveFile(bool isRefresh, string finalStr){ //save to file
            if (!Directory.Exists(GeneratePath)) {
                Directory.CreateDirectory(GeneratePath);
            }

            System.IO.File.WriteAllText(GenerateFilePath, finalStr);
            if (isRefresh) {
                //EditorUtility.OpenWithDefaultApp(GenerateFilePath);
                AssetDatabase.Refresh();
                UnityEngine.Debug.Log("Done");
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