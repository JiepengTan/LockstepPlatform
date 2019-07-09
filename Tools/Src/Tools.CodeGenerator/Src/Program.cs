using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using Lockstep.Util;

namespace Lockstep.CodeGenerator {
    public class FileHandlerInfo {

        public string FileContent;
        public string RegisterCode;
        public string ClsCodeTemplate;
        public FiledHandler[] TypeHandler;
        
    }

    public class GenInfo {
        public string ProjectFilePath;
        public string DllRelPath;
        public string NameSpace;
        public string GeneratePath;
        public string GenerateFileName;
        public string InterfaceName;
        public FileHandlerInfo FileHandlerInfo;
    }

    internal class Program {
        static void DeleteUselessFiles(string relPath){
            var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relPath);
            PathUtil.Walk(dllPath, "*.meta|*.DS_Store", (path) => {
                Console.WriteLine(path);
                File.Delete(path);
            });
        }

        public class CopyFileInfos {
            public GenInfo MsgCommon = new GenInfo();
            public GenInfo MsgServer = new GenInfo();
            public GenInfo EntityConfig = new GenInfo();
            public GenInfo TankEcs = new GenInfo();
        }


        public static void Main(string[] args){
            if (args == null || args.Length == 0) {
                args = new[] {"../Config/CodeGenerator/Config.json"};
            }

            if (args.Length > 0) {
                foreach (var path in args) {
                    Console.WriteLine(path);
                    CopyFilesByConfig(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path));
                }
            }
            else {
                Console.WriteLine("Need config path");
            }
        }

        static void CopyFilesByConfig(string configPath){
            var allTxt = File.ReadAllText(configPath);
            var config = JsonMapper.ToObject<CopyFileInfos>(allTxt);
            var prefix = AppDomain.CurrentDomain.BaseDirectory;
            GenCode(EGenType.Msg, config.MsgCommon);
            GenCode(EGenType.Msg, config.MsgServer);
            GenCode(EGenType.Msg, config.EntityConfig);
            GenCode(EGenType.ECS, config.TankEcs);
            DeleteUselessFiles();
        }

        public enum EGenType {
            ECS,
            Msg,
        }

        static void GenCode(EGenType type, GenInfo info){
            EditorBaseCodeGenerator gener = null;
            if (info == null || string.IsNullOrEmpty(info.GenerateFileName)) return;

            gener = new EditorBaseCodeGenerator() {GenInfo = info};
            gener.HideGenerateCodes();
            gener.BuildProject();
            gener.GenerateCodeNodeData(true);
        }

        public static void DeleteUselessFiles(){
            DeleteUselessFiles("../Client/Client.Tank");
            DeleteUselessFiles("../Client/Client.Common");
            DeleteUselessFiles("../Common/Lockstep.ECS.Common");
        }
    }
}