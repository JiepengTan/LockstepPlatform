using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Lockstep.ECGenerator.Common;
using Lockstep.ECS.ECDefine;
using Lockstep.Util;

namespace Lockstep.ECSGenerator {
    public class CodeGenerator {
        const string NameSpaceTag = "Lockstep.ECS.ECDefine";
        const string IgnoreDll = "Lockstep.ECS.ECDefine.dll";
        protected string _typeCodePrefix = "    ";

        protected virtual string BuildECdefineShell => "../Tools/BuildECDefine";
        protected virtual string BuildShell => "../Tools/BuildCodeGenEntitas";
        protected virtual string SrcCopyRelDir => "../CodeGenEntitas/Src/Components/";
        protected virtual string DstCopyRelDir => "../ECSOutput/Src/Entitas/Components/";
        protected virtual string DesFilePath => "../CodeGenEntitas/Src/Components/ComponentDefine.cs";

        protected virtual string FileHeaderStr =>
            @"using Entitas.CodeGeneration.Attributes; 
using Lockstep.Math; 
using Entitas;";

        public void OnGenCode(){
            //. build ECDefine project
            BuildCodes(BuildECdefineShell);
            //. parse define file and gen entitas code
            ParseECDefine();
            //. build target project
            BuildCodes(BuildShell);
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
        
        static void BuildCodes(string shellName){
            Process process = new Process();
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.ErrorDialog = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = "./" + shellName;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            process.Start();
            ProjectUtil.Log("building  ... process start rojectPath = !!" + AppDomain.CurrentDomain.BaseDirectory);
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            ProjectUtil.Log("build done " + output);
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


            PathUtil.Walk(path, "*.dll", (filePath) => {
                if (filePath.Contains(NameSpaceTag) && !filePath.Contains(IgnoreDll)) {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(FileHeaderStr);
                    var types = DllUtil.LoadDll(filePath,
                        (t) => t.Namespace != null && typeof(IComponent).IsAssignableFrom(t));
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