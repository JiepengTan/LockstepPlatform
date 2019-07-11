using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using LitJson;
using Lockstep.Util;

namespace CopySourceFiles {
    internal class Program {
        public class CopyFileInfos {
            public string __commment;
            public string srcDir;
            public string dstDir;
            public List<string> srcProjectDirs;
            public string sourceFileDir;
        }


        public static void Main(string[] args){
            Console.WriteLine("pwd " + AppDomain.CurrentDomain.BaseDirectory);
            if (args.Length > 0) {
                foreach (var path in args) {
                    Console.WriteLine(path);
                    CopyFilesByConfig(Path.Combine(AppDomain.CurrentDomain.BaseDirectory + path));
                }
            }
            else {
                Console.WriteLine("Need config path");
                //CopyFilesByConfig(AppDomain.CurrentDomain.BaseDirectory +"../Tools/Config/CopySourceFiles/Tank2Common.json");
                //PathUtil.Walk(ConfigPath, "*.json", (path) => { CopyFilesByConfig(path); });
            }
        }

        static void CopyFilesByConfig(string configPath){
            var allTxt = File.ReadAllText(configPath);
            var configs = JsonMapper.ToObject<CopyFileInfos[]>(allTxt);
            foreach (var config in configs) {
                var prefix = AppDomain.CurrentDomain.BaseDirectory;
                var srcPrefix = Path.Combine(prefix + config.srcDir);
                var dstPrefix = Path.Combine(prefix + config.dstDir);
                var sourceDir = config.sourceFileDir;
                foreach (var projectDir in config.srcProjectDirs) {
                    var srcDir = srcPrefix + projectDir + sourceDir;
                    var dstDir = dstPrefix + projectDir + sourceDir;
                    if (Directory.Exists(dstDir)) {
                        Directory.Delete(dstDir, true);
                    }

                    Console.WriteLine("Copy: " + projectDir);
                    PathUtil.CopyDir(srcDir, dstDir, "*.cs");
                }
            }

            Console.WriteLine("Done");
        }
    }
}