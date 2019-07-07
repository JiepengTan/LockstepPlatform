using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Lockstep.ECGenerator.Common;
using Lockstep.ECS.ECDefine;
using Lockstep.Util;

namespace Lockstep.ECSGenerator {
    public class ConfigInfo {
        public string NameSpaceTag;
        public string IgnoreDll;
        public string BuildECdefineShell;
        public string BuildECdefineShellWorkingDir;
        public string BuildShell;
        public string BuildShellWorkingDir;
        public string SrcCopyRelDir;
        public string DstCopyRelDir;
        public string DesFilePath;
        public string FileHeaderStr;
        public string JennyPropertyPath;
    }

    public class CodeGenerator {
        protected string _typeCodePrefix = "    ";
        protected string NameSpaceTag => _configInfo.NameSpaceTag;
        protected string IgnoreDll => _configInfo.IgnoreDll;
        protected string BuildECdefineShell => _configInfo.BuildECdefineShell;
        protected string BuildECdefineShellWorkingDir => _configInfo.BuildECdefineShellWorkingDir;
        protected string BuildShell => _configInfo.BuildShell;
        protected string BuildShellWorkingDir => _configInfo.BuildShellWorkingDir;
        protected string SrcCopyRelDir => _configInfo.SrcCopyRelDir;
        protected string DstCopyRelDir => _configInfo.DstCopyRelDir;
        protected string DesFilePath => _configInfo.DesFilePath;
        protected string FileHeaderStr =>_configInfo.FileHeaderStr;
        protected string JennyPropertyPath =>_configInfo.JennyPropertyPath;
        
        protected ConfigInfo _configInfo;

        public void OnGenCode(ConfigInfo configInfo){
            this._configInfo = configInfo;
            ProjectUtil.Log("cur Dir" + AppDomain.CurrentDomain.BaseDirectory);
            //. build ECDefine project
            ExecuteCmd(BuildECdefineShell, BuildECdefineShellWorkingDir);
            //. parse define file and gen entitas code
            ParseECDefine();
            //. build target project
            ExecuteCmd(BuildShell, BuildShellWorkingDir);
            //. copy component to dst project
            CopyComponents(SrcCopyRelDir, DstCopyRelDir);
            //. update Project file
            UpdateProjectFile();
            //. generate codes
            GenerateCodes();
        }


        protected virtual void UpdateProjectFile(){ }
        protected virtual void GenerateCodes(){ }
        protected virtual void GenTypeCode(StringBuilder sb, Type type){ }

        static void ExecuteCmd(string shellName, string workingDir){
            Process process = new Process();
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.ErrorDialog = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = shellName;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.WorkingDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, workingDir);
            process.Start();
            ProjectUtil.Log("building  ... process start WorkingDirectory = " + process.StartInfo.WorkingDirectory);
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            ProjectUtil.Log(output);
        }

        static void CopyComponents(string srcRelDir, string dstRelDir){
            var srcPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, srcRelDir);
            var dstPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dstRelDir);
            if (Directory.Exists(dstPath)) {
                Directory.Delete(dstPath, true);
            }

            CopyFiles(srcPath, dstPath);
            // update sln file
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


        protected void ParseECDefine(){
            var path = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine(path);
            Dictionary<string, Type[]> dllPath2Types = new Dictionary<string, Type[]>();
            ProjectUtil.Log("ParseECDefine ");

            PathUtil.Walk(path, "*.dll", (filePath) => {
                if (filePath.Contains(NameSpaceTag) && !filePath.Contains(IgnoreDll)) {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(FileHeaderStr);
                    var types = DllUtil.LoadDll(filePath,
                        (t) => t.Namespace != null && typeof(IComponent).IsAssignableFrom(t));
                    ProjectUtil.Log("DllPath " + filePath);
                    if (types == null) return;
                    var typeLst = new List<Type>(types);
                    typeLst.Sort((a, b) => String.CompareOrdinal(a.Namespace, b.Namespace));
                    var curNameSpace = typeLst[0].Namespace;
                    var lastIdx = 0;
                    for (int i = 0; i < typeLst.Count; i++) {
                        if (typeLst[i].Namespace != curNameSpace) {
                            var sameNameSpaceTypes = typeLst.GetRange(lastIdx, i - lastIdx);
                            lastIdx = i;
                            curNameSpace = typeLst[i].Namespace;
                            DealSameNameSpaceTypes(sb, sameNameSpaceTypes);
                        }
                    }

                    if (lastIdx != typeLst.Count) {
                        var sameNameSpaceTypes = typeLst.GetRange(lastIdx, typeLst.Count - lastIdx);
                        DealSameNameSpaceTypes(sb, sameNameSpaceTypes);
                    }

                    var outputPath = Path.Combine(path, DesFilePath);
                    var dir = Path.GetDirectoryName(outputPath);
                    if (!Directory.Exists(dir)) {
                        Directory.CreateDirectory(dir);
                    }

                    File.WriteAllText(outputPath, sb.ToString());
                    Console.WriteLine("Save code " + outputPath);
                }
            });
        }

        void DealSameNameSpaceTypes(StringBuilder sb, List<Type> types){
            if (types == null || types.Count == 0) return;
            var isNeedNameSpace = !string.IsNullOrEmpty(types[0].Namespace);
            if (isNeedNameSpace) {
                sb.AppendLine("namespace " + types[0].Namespace + "{");
            }

            _typeCodePrefix = isNeedNameSpace ? "    " : "";
            foreach (var type in types) {
                GenTypeCode(sb, type);
                sb.AppendLine();
            }

            if (isNeedNameSpace) {
                sb.AppendLine("}");
            }
        }
    }
}