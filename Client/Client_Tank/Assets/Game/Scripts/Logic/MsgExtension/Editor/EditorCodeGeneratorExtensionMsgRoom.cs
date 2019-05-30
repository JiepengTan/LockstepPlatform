using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Entitas;
using Lockstep.Serialization;
using NetMsg.Game;
using NetMsg.Lobby;
using UnityEditor;
using UnityEngine;
using Debug = Lockstep.Logging.Debug;

namespace BinarySerializer {
    public partial class EditorCodeGeneratorExtensionMsgRoom {
        [MenuItem("Tools/MsgExtension/0.Hide Compiler Error")]
        public static void HideCompileError(){
            new EditorCodeGeneratorExtensionMsgLobby().HideGenerateCodes(false);
            new EditorCodeGeneratorExtensionMsgRoom().HideGenerateCodes(false);
        }

        [MenuItem("Tools/MsgExtension/1.Generate Code")]
        public static void GenerateCode(){
            new EditorCodeGeneratorExtensionMsgLobby().GenerateCodeNodeData(true);
            new EditorCodeGeneratorExtensionMsgRoom().GenerateCodeNodeData(true);
        }
    }

    ///Users/jiepengtan/Projects/LockstepDemo/Common/NetMsg.Lobby/Src 

    public partial class EditorCodeGeneratorExtensionMsgLobby : EditorCodeGeneratorExtensionMsgRoom {
        protected override string GeneratePath {
            get { return Path.Combine(Application.dataPath, "../../../Common/NetMsg.Lobby/Src/"); }
        }
        public override Type[] GetTypes(){
            return typeof(EMsgCL).Assembly.GetTypes();
        }     
        public override string GetNameSpace(){
            return typeof(EMsgCL).Namespace;
        }
    }

    public partial class EditorCodeGeneratorExtensionMsgRoom : EditorBaseCodeGenerator {
        protected override string GeneratePath {
            get { return Path.Combine(Application.dataPath, "../../../Common/NetMsg.Game/Src/"); }
        }

        protected override string GenerateFilePath {
            get { return Path.Combine(GeneratePath, "ExtentionMsg.cs"); }
        }

        public override string prefix {
            get { return "\t\t\t"; }
        }

        public virtual Type[] GetTypes(){
            return typeof(EMsgCS).Assembly.GetTypes();
        }
        public virtual string GetNameSpace(){
            return typeof(EMsgCS).Namespace;
        }
        protected override void ReflectRegisterTypes(){
            Type[] types = null;
            HashSet<Type> allTypes = new HashSet<Type>();
            types = GetTypes();
            foreach (var t in types) {
                if( !allTypes.Add(t)) continue;
                if (t.IsSubclassOf(typeof(BaseFormater)) &&
                    t.GetCustomAttribute(typeof(SelfImplementAttribute)) == null) {
                    RegisterType(t);
                }
            }
        }

        public void GenerateCodeNodeData(bool isRefresh, params Type[] types){
            var ser = new CodeGenerator();
            var extensionStr = GenTypeCode(ser, new TypeHandlerMsg(this));
            var finalStr = GenFinalCodes(extensionStr, isRefresh);
            SaveFile(isRefresh, finalStr);
        }

        string GenFinalCodes(string extensionStr, bool isRefresh){
            string fileContent =
                @"//#define DONT_USE_GENERATE_CODE 
//Auto Gen by code please do not modify it
//https://github.com/JiepengTan/LockstepPlatform
using Lockstep.Serialization;
namespace #NAMESPACE{
#if !DONT_USE_GENERATE_CODE
//#TYPES_EXTENSIONS
#endif
}
";
            return fileContent
                    .Replace("#NAMESPACE", GetNameSpace())
                    .Replace("//#TYPES_EXTENSIONS", extensionStr)
                ;
        }
    }
}