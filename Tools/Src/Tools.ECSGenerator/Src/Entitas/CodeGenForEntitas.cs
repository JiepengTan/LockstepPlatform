using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DesperateDevs.CodeGeneration;
using DesperateDevs.CodeGeneration.CodeGenerator;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;
using LitJson;
using Lockstep.ECS.ECDefine;
using Lockstep.Util;
using static Lockstep.Util.ProjectUtil;

namespace Lockstep.ECSGenerator {
    public class CodeGenForEntitas : CodeGenerator {

        static void TestMergeEntitasFile(){
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../CodeGenEntitas/Src/RawFiles");
            StringBuilder sb = new StringBuilder();
            PathUtil.Walk(path, "*.cs", (filePath) => { sb.AppendLine(File.ReadAllText(filePath)); });
            File.WriteAllText(Path.Combine(path, "../Output/ComponentDefine.cs"), sb.ToString());
        }

        public static void GenCode(string configPath){
            var allTxt = File.ReadAllText(configPath);
            var configInfo = JsonMapper.ToObject<ConfigInfo>(allTxt);
            //Console.WriteLine(JsonMapper.ToJson(configInfo));
            new CodeGenForEntitas().OnGenCode(configInfo);
        }

        protected override void UpdateProjectFile(){
            return;
            //Dotnet 不需要更新Proj 文件
            var jennyConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Jenny.properties");
            var text = File.ReadAllText(jennyConfig);
            var properties = new Properties(text);
            var projectPath = properties.ToDictionary()["DesperateDevs.CodeGeneration.Plugins.ProjectPath"];
            var dstPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "../ECSOutput/Src/Entitas/Components/");
            var relDir = @"Src/Entitas/Components/";

            ProjectUtil.UpdateProjectFile(projectPath, relDir, dstPath);
        }


        protected override void GenerateCodes(){
            Log((object) "Generating...");
            var recordpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, JennyPropertyPath);
            LogError("recordpath " + recordpath);
            var text = File.ReadAllText(recordpath);
            var preference = new Preferences(recordpath, recordpath);
            Log(text);
            DesperateDevs.CodeGeneration.CodeGenerator.CodeGenerator codeGenerator =
                CodeGeneratorUtil.CodeGeneratorFromPreferences(preference);

            codeGenerator.OnProgress += (GeneratorProgress) ((title, info, progress) => {
                Log("progress " + (progress));
            });

            CodeGenFile[] codeGenFileArray1 = new CodeGenFile[0];
            CodeGenFile[] codeGenFileArray2;
            try {
                codeGenFileArray2 = codeGenerator.Generate();
            }
            catch (Exception ex) {
                codeGenFileArray1 = new CodeGenFile[0];
                codeGenFileArray2 = new CodeGenFile[0];
                //LogError("Error" + ex.Message + ex.StackTrace);
            }

            Log((object) ("Done Generated " + (object) ((IEnumerable<CodeGenFile>) codeGenFileArray2)
                          .Select<CodeGenFile, string>((
                              Func<CodeGenFile, string>) (file => file.fileName)).Distinct<string>()
                          .Count<string>() +
                          " files (" + (object) ((
                              IEnumerable<CodeGenFile>) codeGenFileArray1)
                          .Select<CodeGenFile, string>(
                              (Func<CodeGenFile, string>) (file => file.fileContent.ToUnixLineEndings()))
                          .Sum<string>(
                              (Func<string, int>) (content => content.Split(new char[1] {
                                  '\n'
                              }, StringSplitOptions.RemoveEmptyEntries).Length)) + " sloc, " +
                          (object) ((IEnumerable<CodeGenFile>) codeGenFileArray2)
                          .Select<CodeGenFile, string>(
                              (Func<CodeGenFile, string>) (file => file.fileContent.ToUnixLineEndings()))
                          .Sum<string>(
                              (
                                  Func<string, int>) (content => content.Split('\n').Length)) + " loc)"));
        }

        protected override void GenTypeCode(StringBuilder sb, Type type){
            var typeName = type.Name;
            var attriNames = type.GetCustomAttributes(typeof(AttributeAttribute), true)
                .Select((attri) => (attri as AttributeAttribute)?.name).ToArray();
            foreach (var attriName in attriNames) {
                sb.AppendLine(_typeCodePrefix + $"[{attriName}]");
            }

            sb.AppendLine(_typeCodePrefix + $"public partial class {typeName} :IComponent {{");
            string filedStr = "";
            foreach (var filed in type.GetFields()) {
                var filedAttris = filed.GetCustomAttributes(typeof(AttributeAttribute), true)
                    .Select((attri) => (attri as AttributeAttribute)?.name).ToArray();
                sb.Append(_typeCodePrefix + "    ");
                foreach (var filedAttri in filedAttris) {
                    sb.Append($"[{filedAttri}]");
                }

                var fileTypeStr = filed.FieldType.ToString();
                if (type2Str.TryGetValue(filed.FieldType, out var typstr)) {
                    fileTypeStr = typstr;
                }

                sb.AppendLine($"public {fileTypeStr} {filed.Name};");
            }

            sb.AppendLine(_typeCodePrefix + "}");
        }

        public static Dictionary<Type, string> type2Str = new Dictionary<Type, string>() {
            {typeof(bool), "bool"},
            {typeof(string), "string"},
            {typeof(float), "LFloat"},
            {typeof(byte), "byte"},
            {typeof(sbyte), "sbyte"},
            {typeof(short), "short"},
            {typeof(ushort), "ushort"},
            {typeof(int), "int"},
            {typeof(uint), "uint"},
            {typeof(long), "long"},
            {typeof(ulong), "ulong"},
            {typeof(Lockstep.ECS.ECDefine.Vector2), "LVector2"},
            {typeof(Lockstep.ECS.ECDefine.Vector3), "LVector3"},
            {typeof(Lockstep.ECS.ECDefine.Quaternion), "LQuaternion"},
        };
    }
}