﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Entitas;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.CodeGenerator {

    public partial class EditorCodeGeneratorExtensionEcs : EditorBaseCodeGenerator {
        public override string prefix {
            get { return "\t\t\t"; }
        }

        protected override void ReflectRegisterTypes(){
            var types = new Type[0];// //TODO fixed by load dll// typeof(Lockstep.ECS.Actor.IdComponent).Assembly.GetTypes();
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
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Lockstep.CodeGenerator
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
//     https://github.com/JiepengTan/LockstepPlatform
// </auto-generated>
//------------------------------------------------------------------------------

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