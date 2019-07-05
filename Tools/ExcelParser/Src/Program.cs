using System;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
#endif
using LitJson;


class Program {
    public class CopyFileInfos {
        public List<string> InputDirs;
        public string OutputDirCsv;
        public string OutputDirByte;
        public string OutputDirCode;
        public string TemplatePath;
    }

    public static string ConfigPath => AppDomain.CurrentDomain.BaseDirectory + "../Config/ExcelParser/";

    public static void Main(string[] args){
        //args = new[] {"Tank.json"};
        if (args.Length > 0) {
            foreach (var path in args) {
                Console.WriteLine(path);
                CopyFilesByConfig(ConfigPath + path);
            }
        }
        else {
            Console.WriteLine("Need config path");
        }
    }

    static void CopyFilesByConfig(string configPath){
        var allTxt = File.ReadAllText(configPath);
        var config = JsonMapper.ToObject<CopyFileInfos>(allTxt);
        var prefix = AppDomain.CurrentDomain.BaseDirectory + "../";
        var sourceDir = "/Src";
        var gener = new CSVGenCode.TableConfigGenerator();
        try {
            for (int i = 0; i < config.InputDirs.Count; i++) {
                config.InputDirs[i] = prefix + config.InputDirs[i];
            }
            config.OutputDirCsv = prefix + config.OutputDirCsv;
            config.OutputDirByte = prefix + config.OutputDirByte;
            config.OutputDirCode = prefix + config.OutputDirCode;
            config.TemplatePath = prefix + config.TemplatePath;
            
            gener.Run(
                config.InputDirs,
                config.OutputDirCsv,
                config.OutputDirByte,
                config.OutputDirCode,
                config.TemplatePath);
        }
        catch (Exception e) {
            Console.WriteLine(gener.DebugSttring());
            Console.Write(e.StackTrace);
            Console.ReadLine();
            Console.Read();
        }

        Console.WriteLine("Done");
    }
}