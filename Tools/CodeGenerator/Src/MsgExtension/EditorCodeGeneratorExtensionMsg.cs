using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Lockstep.Game;
using Lockstep.Serialization;
using NetMsg.Common;
using NetMsg.Server;
using Debug = Lockstep.Logging.Debug;

namespace Lockstep.CodeGenerator {
    public class EditorCodeGenerator {
#if UNITY_EDITOR
        [MenuItem("Tools/MsgExtension/0.Hide Compiler Error")]
#endif
        public static void HideCompileError(){
            new EditorCodeGeneratorExtensionMsgCommon().HideGenerateCodes(false);
            new EditorCodeGeneratorExtensionMsgServer().HideGenerateCodes(false);
            new EditorCodeGeneratorExtensionEntityConfig().HideGenerateCodes(false);
        }
#if UNITY_EDITOR
        [MenuItem("Tools/MsgExtension/1.Generate Code")]
#endif
        public static void GenerateCode(){
            new EditorCodeGeneratorExtensionMsgCommon().GenerateCodeNodeData(true);
            new EditorCodeGeneratorExtensionMsgServer().GenerateCodeNodeData(true);
            new EditorCodeGeneratorExtensionEntityConfig().GenerateCodeNodeData(true);
        }
    }
    public partial class EditorCodeGeneratorExtensionEntityConfig : EditorCodeGeneratorExtensionMsg {
        public override Type[] GetTypes(){
            return typeof(EntityConfig).Assembly.GetTypes();
        }

        public override string GetNameSpace(){
            return typeof(EntityConfig).Namespace;
        }
        protected override string GeneratePath {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Common/ECS.Tank/Src/"); }
        }
        protected override string GenerateFilePath {
            get { return Path.Combine(GeneratePath, "ExtentionConfig.cs"); }
        }
    }
    public partial class EditorCodeGeneratorExtensionMsgCommon : EditorCodeGeneratorExtensionMsg {
        public override Type[] GetTypes(){
            return typeof(EMsgSC).Assembly.GetTypes();
        }

        public override string GetNameSpace(){
            return typeof(EMsgSC).Namespace;
        }
        protected override string GeneratePath {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Common/NetMsg.Common/Src/"); }
        }
    }
    public partial class EditorCodeGeneratorExtensionMsgServer : EditorCodeGeneratorExtensionMsg {
        
        public override Type[] GetTypes(){
            return typeof(EMsgSS).Assembly.GetTypes();
        }

        public override string GetNameSpace(){
            return typeof(EMsgSS).Namespace;
        }
        
        protected override string GeneratePath {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Server/NetMsg.Server/Src/"); }
        }
    }
    public abstract partial class EditorCodeGeneratorExtensionMsg : EditorBaseCodeGenerator {
        protected override string GeneratePath {
            get { return ""; }
        }

        protected override string GenerateFilePath {
            get { return Path.Combine(GeneratePath, "ExtentionMsg.cs"); }
        }

        public override string prefix {
            get { return "\t\t\t"; }
        }

        public abstract Type[] GetTypes();
        public abstract string GetNameSpace();

        protected override void ReflectRegisterTypes(){
            Type[] types = null;
            HashSet<Type> allTypes = new HashSet<Type>();
            types = GetTypes();
            foreach (var t in types) {
                if (!allTypes.Add(t)) continue;
                if (t.IsSubclassOf(typeof(BaseMsg)) 
                    &&t.GetCustomAttribute(typeof(SelfImplementAttribute)) == null
                    ) {
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