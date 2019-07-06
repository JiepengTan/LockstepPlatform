using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using LitJson;
using Lockstep.Util;

namespace CopySourceFiles {
    internal class Program {
        public class CopyFileInfos {
            public string srcDir;
            public string dstDir;
            public List<string> srcProjectDirs;
            public string sourceFileDir;
        }

        public static string ConfigPath => AppDomain.CurrentDomain.BaseDirectory + "../Tools/Config/CopySourceFiles/";

        public static void Main(string[] args){
            if (args.Length > 0) {
                foreach (var path in args) {
                    Console.WriteLine(path);
                    CopyFilesByConfig(AppDomain.CurrentDomain.BaseDirectory + path);
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
            var config = JsonMapper.ToObject<CopyFileInfos>(allTxt);
            var prefix = AppDomain.CurrentDomain.BaseDirectory + "../";
            var srcPrefix = prefix + config.srcDir;
            var dstPrefix = prefix + config.dstDir;
            var sourceDir = "/Src";
            foreach (var projectDir in config.srcProjectDirs) {
                var srcDir = srcPrefix + projectDir + sourceDir;
                var dstDir = dstPrefix + projectDir + sourceDir;
                if (Directory.Exists(dstDir)) {
                    Directory.Delete(dstDir, true);
                }
                Console.WriteLine("Copy: " + projectDir);
                CopyFiles(srcDir, dstDir);
            }

            Console.WriteLine("Done");
        }

        static void CopyFiles(string srcDir, string dstDir){
            var srcDirName = Path.GetDirectoryName(srcDir);
            var dstDirName = Path.GetDirectoryName(dstDir);
            PathUtil.Walk(srcDir, "*.cs", (path) => {
                var dstPath = path.Replace(srcDirName, dstDirName);
                CopyFile(path, dstPath);
            });
        }

        static void CopyFile(string srcPath, string dstPath){
            var dstDir = Path.GetDirectoryName(dstPath);
            if (!Directory.Exists(dstDir)) {
                Directory.CreateDirectory(dstDir);
            }

            File.Copy(srcPath, dstPath, true);
        }
    }
}