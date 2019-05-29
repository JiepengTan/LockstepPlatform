using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Entitas;
using UnityEditor;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace BinarySerializer {
    public partial class EditorCodeGeneratorEcsExtension {
        [MenuItem("Tools/ECSExtension/0.Hide Compiler Error")]
        public static void HideCompileError(){
            new EditorCodeGeneratorEcsExtension().HideGenerateCodes(false);
        }

        [MenuItem("Tools/ECSExtension/1.Generate Code")]
        public static void GenerateCode(){
            new EditorCodeGeneratorEcsExtension().GenerateCodeNodeData(true);
        }
    }

    public partial class EditorCodeGeneratorEcsExtension : EditorBaseCodeGenerator {
        protected override string GeneratePath {
            get { return Path.Combine(Application.dataPath, "Game/Scripts/ECS/ECSExtension/Generated/"); }
        }

        protected override string GenerateFilePath {
            get { return Path.Combine(GeneratePath, "ECSExtention.cs"); }
        }

        protected override ITypeHandler TypeHandler {
            get { return new TypeHandlerECS(this); }
        }

        public override string prefix {
            get { return "\t\t\t"; }
        }

        protected override void ReflectRegisterTypes(){
            var tAttr = typeof(ToBinaryAttribute);
            var types = ReflectionUtility.GetInterfaces(typeof(IComponent));
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
            var extensionStr = GenTypeCode(ser);
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
//Auto Gen by code please do not modify it by hand
using System;
using System.Collections.Generic;
using UnityEngine;
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