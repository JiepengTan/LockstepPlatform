using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public class DebugHook : CodeGeneratorTrackingHook {
            protected override string name {
                get { return "DebugHook"; }
            }

            protected override DesperateDevs.Analytics.TrackingData GetData(){
                var _preProcessorscount = _preProcessors.Length;
                var _dataProviderscount = _dataProviders.Length;
                var _codeGeneratorscount = _codeGenerators.Length;
                var _postProcessorscount = _postProcessors.Length;

                Log(
                    "_preProcessorscount: " + _preProcessorscount
                                            + "_dataProviderscount: " + _dataProviderscount
                                            + "_codeGeneratorscount: " + _codeGeneratorscount
                                            + "_postProcessorscount: " + _postProcessorscount
                );
                return new DesperateDevs.Analytics.TrackingData();
            }
        }

        private string resolvePath(string name, string[] _basePaths){
            try {
                string name1 = new System.Reflection.AssemblyName(name).Name;
                if (!name1.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) &&
                    !name1.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    name1 += ".dll";
                foreach (string basePath in _basePaths) {
                    string path = basePath + Path.DirectorySeparatorChar.ToString() + name1;
                    if (File.Exists(path)) {
                        Log("    ➜ Resolved: " + path);
                        return path;
                    }
                }
            }
            catch (FileLoadException ex) {
                Log("    × Could not resolve: " + name);
            }

            return (string) null;
        }

        Type[] LoadTypes(Preferences preferences){
            CodeGeneratorConfig andConfigure = preferences.CreateAndConfigure<CodeGeneratorConfig>();
            var dirs = new string[andConfigure.searchPaths.Length];
            for (int i = 0; i < andConfigure.searchPaths.Length; i++) {
                dirs[i] = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, andConfigure.searchPaths[i]);
            }

            List<Assembly> assemblies = new List<Assembly>();
            foreach (string plugin in andConfigure.plugins) {
                var pluginName = plugin;
                if (!plugin.EndsWith(".dll")) {
                    pluginName = plugin + ".dll";
                }

                foreach (var dir in dirs) {
                    var fileName = Path.Combine(dir, pluginName);
                    if (File.Exists(fileName)) {
                        var assembly = Assembly.LoadFrom(fileName);
                        assemblies.Add(assembly);
                    }
                }
            }

            return assemblies.GetAllTypes();
        }

        public static Type[] GetAllTypes(IEnumerable<Assembly> assemblies){
            List<Type> typeList = new List<Type>();
            foreach (Assembly assembly in assemblies) {
                try {
                    typeList.AddRange((IEnumerable<Type>) assembly.GetTypes());
                }
                catch (ReflectionTypeLoadException ex) {
                    typeList.AddRange(
                        ((IEnumerable<Type>) ex.Types).Where<Type>((Func<Type, bool>) (type => type != null)));
                }
            }

            return typeList.ToArray();
        }

        public ICodeGenerationPlugin[] LoadFromPlugins(
            Preferences preferences){
            CodeGeneratorConfig andConfigure = preferences.CreateAndConfigure<CodeGeneratorConfig>();
            AssemblyResolver assemblyResolver = new AssemblyResolver(false, andConfigure.searchPaths);
            foreach (string plugin in andConfigure.plugins)
                assemblyResolver.Load(plugin);
            //Type[] types = assemblyResolver.GetTypes();
            var types = LoadTypes(preferences);
            
            Log($"----------------------------types Count = {types.Length}-------------------" );
            return ((IEnumerable<Type>) types.GetNonAbstractTypes<ICodeGenerationPlugin>())
                .Select<Type, ICodeGenerationPlugin>((Func<Type, ICodeGenerationPlugin>) (type => {
                    try {
                        return (ICodeGenerationPlugin) Activator.CreateInstance(type);
                    }
                    catch (TypeLoadException ex) {
                        Log(ex.Message);
                    }

                    return (ICodeGenerationPlugin) null;
                })).Where<ICodeGenerationPlugin>((Func<ICodeGenerationPlugin, bool>) (instance => instance != null))
                .ToArray<ICodeGenerationPlugin>();
        }

        DesperateDevs.CodeGeneration.CodeGenerator.CodeGenerator CodeGeneratorFromPreferences(Preferences preferences){
            ICodeGenerationPlugin[] instances = LoadFromPlugins(preferences);
            CodeGeneratorConfig andConfigure = preferences.CreateAndConfigure<CodeGeneratorConfig>();
            IPreProcessor[] enabledInstancesOf1 =
                CodeGeneratorUtil.GetEnabledInstancesOf<IPreProcessor>(instances, andConfigure.preProcessors);
            IDataProvider[] enabledInstancesOf2 =
                CodeGeneratorUtil.GetEnabledInstancesOf<IDataProvider>(instances, andConfigure.dataProviders);
            ICodeGenerator[] enabledInstancesOf3 =
                CodeGeneratorUtil.GetEnabledInstancesOf<ICodeGenerator>(instances, andConfigure.codeGenerators);
            IPostProcessor[] enabledInstancesOf4 =
                CodeGeneratorUtil.GetEnabledInstancesOf<IPostProcessor>(instances, andConfigure.postProcessors);
            configure((ICodeGenerationPlugin[]) enabledInstancesOf1, preferences);
            configure((ICodeGenerationPlugin[]) enabledInstancesOf2, preferences);
            configure((ICodeGenerationPlugin[]) enabledInstancesOf3, preferences);
            configure((ICodeGenerationPlugin[]) enabledInstancesOf4, preferences);
            bool trackHooks = true;
            if (preferences.HasKey("Jenny.TrackHooks"))
                trackHooks = preferences["Jenny.TrackHooks"] == "true";
            return new DesperateDevs.CodeGeneration.CodeGenerator.CodeGenerator(enabledInstancesOf1,
                enabledInstancesOf2, enabledInstancesOf3, enabledInstancesOf4, trackHooks);
        }

        private static void configure(ICodeGenerationPlugin[] plugins, Preferences preferences){
            foreach (IConfigurable configurable in plugins.OfType<IConfigurable>())
                configurable.Configure(preferences);
        }

        protected override void GenerateCodes(){
            Log((object) "Generating...");
            var recordpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, JennyPropertyPath);
            LogError("recordpath " + recordpath);
            var text = File.ReadAllText(recordpath);
            var preference = new Preferences(recordpath, recordpath);
            ICodeGenerationPlugin[] instances = CodeGeneratorUtil.LoadFromPlugins(preference);
            CodeGeneratorConfig andConfigure = preference.CreateAndConfigure<CodeGeneratorConfig>();
            AssemblyResolver assemblyResolver = new AssemblyResolver(false, andConfigure.searchPaths);
            foreach (string plugin in andConfigure.plugins) {
                Log("load plugin " + plugin);
                assemblyResolver.Load(plugin);
            }

            DesperateDevs.CodeGeneration.CodeGenerator.CodeGenerator codeGenerator =
                CodeGeneratorFromPreferences(preference);
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
                LogError("Error" + ex.Message + ex.StackTrace);
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