using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Entitas;
using UnityEditor;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.CodeGenerator {
    public partial class EditorCodeGeneratorExtensionEcs {
#if UNITY_EDITOR
        [MenuItem("Tools/ECSExtension/0.Hide Compiler Error")]
#endif
        public static void HideCompileError(){
            new EditorCodeGeneratorExtensionEcs().HideGenerateCodes(false);
        }
#if UNITY_EDITOR
        [MenuItem("Tools/ECSExtension/1.Generate Code")]
#endif
        public static void GenerateCode(){
            new EditorCodeGeneratorExtensionEcs().GenerateCodeNodeData(true);
        }
    }

    public partial class EditorCodeGeneratorExtensionEcs : EditorBaseCodeGenerator {
        protected override string GeneratePath {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Common/ECS.Tank/Src/"); }
        }

        protected override string GenerateFilePath {
            get { return Path.Combine(GeneratePath, "ExtentionECS.cs"); }
        }
        public override string prefix {
            get { return "\t\t\t"; }
        }

        protected override void ReflectRegisterTypes(){
            var types =  typeof(Lockstep.ECS.Actor.IdComponent).Assembly.GetTypes();
            //var types = ReflectionUtility.GetInterfaces(typeof(IComponent));
            foreach (var t in types) {
                //代码自动生成的Componennt 不处理
                if (t.GetCustomAttributes(typeof(Entitas.CodeGeneration.Attributes.DontGenerateAttribute),
                    true).Any()) {
                    continue;
                }

                //RegisterTypeWithNamespace(t);
                RegisterType(t);
            }
        }

        public void GenerateCodeNodeData(bool isRefresh, params Type[] types){
            var ser = new CodeGenerator();
            var extensionStr = GenTypeCode(ser,new TypeHandlerECS(this));
            var registerStr = GenRegisterCode(ser);
            var finalStr = GenFinalCodes(extensionStr, registerStr, isRefresh);
            SaveFile(isRefresh, finalStr);
        }

        protected string GenRegisterCode(CodeGenerator gen){
            var allGentedTypes = gen.AllGeneratedTypes;
            var prefix = "";
            var RegisterType = "{0}namespace {2}{{public partial class {1} : BaseComponent{{}}}};";
            allGentedTypes.Sort((a, b) => { return GetTypeName(a).CompareTo(GetTypeName(b)); });
            StringBuilder sb = new StringBuilder();
            foreach (var t in allGentedTypes) {
                var clsFuncName = GetTypeName(t);
                var nameSpace = GetNameSpace(t);
                sb.AppendLine(string.Format(RegisterType, prefix, clsFuncName, nameSpace));
            }

            return sb.ToString();
        }

        string GenFinalCodes(string extensionStr, string RegisterStr, bool isRefresh){
            string fileContent =
                @"//#define DONT_USE_GENERATE_CODE 
//Auto Gen by code please do not modify it
//https://github.com/JiepengTan/LockstepPlatform
using System;
using System.Collections.Generic;
using Lockstep.Serialization;
//#DECLARE_BASE_TYPES

#if !DONT_USE_GENERATE_CODE
//#TYPES_EXTENSIONS
#endif
";
            return fileContent
                    .Replace("//#DECLARE_BASE_TYPES", RegisterStr)
                    .Replace("//#TYPES_EXTENSIONS", extensionStr)
                ;
        }
    }
}