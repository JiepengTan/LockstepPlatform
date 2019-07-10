using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using Lockstep.Util;

namespace Lockstep.CodeGenerator {
    internal class Program {
        static void DeleteUselessFiles(string relPath){
            var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relPath);
            PathUtil.Walk(dllPath, "*.meta|*.DS_Store", (path) => {
                Console.WriteLine(path);
                File.Delete(path);
            });
        }

        public class CodeGenInfos {
            public GenInfo[] GenInfos;
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
            var config = JsonMapper.ToObject<CodeGenInfos>(allTxt);
            var prefix = AppDomain.CurrentDomain.BaseDirectory;
            foreach (var genInfo in config.GenInfos) {
                GenCode(genInfo);
            }
            DeleteUselessFiles();
        }

        static void GenCode(GenInfo info){
            EditorBaseCodeGenerator gener = null;
            if (info == null || string.IsNullOrEmpty(info.GenerateFileName)) return;
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,info.TypeHandlerConfigPath);
            Console.WriteLine(path);
            var allTxt = File.ReadAllText(path);
            var config = JsonMapper.ToObject<FileHandlerInfo>(allTxt);
            info.FileHandlerInfo = config;
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