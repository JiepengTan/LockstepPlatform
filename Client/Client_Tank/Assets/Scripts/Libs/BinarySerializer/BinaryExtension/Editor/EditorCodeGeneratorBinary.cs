﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace BinarySerializer {
    public partial class EditorCodeGeneratorBinary {
        [MenuItem("Tools/BinarySerializer/-1.Test Generate SerializeCode")]
        public static void TestGenerateCode(){
            new EditorCodeGeneratorBinary().GenerateCodeNodeData(true, new Type[] {typeof(TestSerializeClassAlone)});
        }

        [MenuItem("Tools/BinarySerializer/0.Hide Compiler Error")]
        public static void HideCompileError(){
            new EditorCodeGeneratorBinary().HideGenerateCodes(false);
        }

        [MenuItem("Tools/BinarySerializer/1.Generate SerializeCode")]
        public static void GenerateCode(){
            new EditorCodeGeneratorBinary().GenerateCodeNodeData(true);
        }
    }

    public partial class EditorCodeGeneratorBinary : EditorBaseCodeGenerator {
        protected override string GeneratePath {
            get { return Path.Combine(Application.dataPath, "Game/Scripts/Libs/BinarySerializer/BinaryExtension/Generated/"); }
        }

        protected override string GenerateFilePath {
            get { return Path.Combine(GeneratePath, "ExtensionBinary.cs"); }
        }

        public override string prefix {
            get { return "\t\t"; }
        }
        protected override void ReflectRegisterTypes(){
            var tAttr = typeof(ToBinaryAttribute);
            var types = ReflectionUtility.GetAttriTypes(tAttr, true);
            foreach (var t in types) {
                var atrri = (ToBinaryAttribute) t.GetCustomAttributes(tAttr, true)[0];
                if (atrri.AllChildClass && atrri.IsNeedNameSpace) {
                    RegisterBaseTypeWithNamespace(t);
                }
                else if (atrri.AllChildClass) {
                    RegisterBaseType(t);
                }
                else {
                    RegisterType(t);
                }
            }
        }

        void GenerateCodeNodeData(bool isRefresh, params Type[] types){
            var ser = new CodeGenerator();
            var ignoreTypes = new Type[] {typeof(UnityEngine.Transform), typeof(UnityEngine.GameObject)};
            ser.AddIgnoredTypes(ignoreTypes);
            var extensionStr = GenTypeCode(ser,new TypeHandlerBinary(this));
            var registerStr = GenRegisterCode(ser);
            var finalStr = GenFinalCodes(extensionStr, registerStr, isRefresh);
            //save to file
            SaveFile(isRefresh, finalStr);
        }


        private string GenRegisterCode(CodeGenerator gen){
            var allGentedTypes = gen.AllGeneratedTypes;
            var prefix = "        ";
            var RegisterType = "{0}RegisterReaderWriter(Read{1}, Write{1});";
            allGentedTypes.Sort((a, b) => { return GetFuncName(a).CompareTo(GetFuncName(b)); });
            StringBuilder sb = new StringBuilder();
            foreach (var t in allGentedTypes) {
                var clsFuncName = GetFuncName(t);
                sb.AppendLine(string.Format(RegisterType, prefix, clsFuncName));
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
using UnityEngine;
namespace BinarySerializer{
#if DONT_USE_GENERATE_CODE
public static partial class BinarySerializer
{
    public static void RegisterAutoGeneratedTypes(){}
}
#else
public static partial class BinarySerializer
{
    public static void RegisterAutoGeneratedTypes()
    {
//#RegisterOtherTypes
    }
}
#endif

#if !DONT_USE_GENERATE_CODE
//#TypesExtension
#endif
}
";
            return fileContent
                    .Replace("//#RegisterOtherTypes", RegisterStr)
                    .Replace("//#TypesExtension", extensionStr)
                ;
        }

    }
}