using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LitJson;
using Lockstep.ECGenerator;
using Lockstep.ECS.UnsafeECS.CodeGen;
using Lockstep.Util;

namespace Lockstep.ECSGenerator {
    public class ConfigInfo {
        public string DllNameTag;
        public string IgnoreDll;
        public string BuildECdefineShell;
        public string BuildECdefineShellWorkingDir;
        public string DesFileDir;
        public string TypeNameSpaceTag;
        public string[] FileContent;
    }

    public class CodeGenForUnsafeECS {
        private ConfigInfo Info;

        public void OnGenCode(ConfigInfo configInfo){
            this.Info = configInfo;
            ProjectUtil.Log("cur Dir" + AppDomain.CurrentDomain.BaseDirectory);
            //. build ECDefine project
            //ExecuteCmd(Info.BuildECdefineShell, Info.BuildECdefineShellWorkingDir);
            //. parse define file and gen entitas code
            ParseECDefine();
        }

        static void ExecuteCmd(string shellName, string workingDir){
            Utils.ExecuteCmd(shellName, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, workingDir));
        }

        public static void GenCode(string configPath){
            var allTxt = File.ReadAllText(configPath);
            var configInfo = JsonMapper.ToObject<ConfigInfo>(allTxt);
            //Console.WriteLine(JsonMapper.ToJson(configInfo));
            new CodeGenForUnsafeECS().OnGenCode(configInfo);
        }

        protected void ParseECDefine(){
            var path = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine(path);
            Dictionary<string, Type[]> dllPath2Types = new Dictionary<string, Type[]>();
            ProjectUtil.Log("ParseECDefine ");

            PathUtil.Walk(path, "*.dll", (filePath) => {
                if (filePath.Contains(Info.DllNameTag) && !filePath.Contains(Info.IgnoreDll)) {
                    ProjectUtil.Log("DllPath " + filePath);
                    var types = DllUtil.LoadDll(filePath, (t) => true);
                    types = types.Where(t => t.Namespace.Contains(Info.TypeNameSpaceTag)).ToArray();
                    var geners = GetType().Assembly.GetTypes()
                        .Where(t => t.BaseType != null && typeof(CodeGenBase).IsAssignableFrom(t) && t!= typeof(CodeGenBase));
                    foreach (var gener in geners) {
                        StringBuilder sb = new StringBuilder();
                        var instance = (CodeGenBase) Activator.CreateInstance(gener);
                        instance._sb = sb;
                        instance.Info = Info;
                        instance.GenCode(types);
                        var outputPath = Path.Combine(Path.Combine(path, Info.DesFileDir), gener.Name.Replace("CodeGen","UnsafeECSGen_") + ".cs");
                        SaveToFile(outputPath, sb);
                    }
                }
            });
        }

        private void SaveToFile(string outputPath, StringBuilder sb){
            var dir = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }

            var finalContent = string.Join(' ', Info.FileContent).Replace("#TYPES_EXTENSIONS", sb.ToString());
            File.WriteAllText(outputPath, finalContent);
            Console.WriteLine("Save code " + outputPath);
        }
    }
}