﻿using System;
using System.IO;
using Lockstep.Util;

namespace Lockstep.CodeGenerator {
    internal class Program {
        static void DeleteUselessFiles(string relPath){
            var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relPath);
            PathUtil.Walk(dllPath,"*.meta|*.DS_Store", (path) => {
                Console.WriteLine(path);
                File.Delete(path);
            });
        }

        public static void Main(string[] args){
            EditorCodeGeneratorExtensionEcs.GenerateCode();
            EditorCodeGenerator.GenerateCode();
            DeleteUselessFiles("../Client/Client.Tank");
            DeleteUselessFiles("../Client/Client.Common");
            DeleteUselessFiles("../Common/Lockstep.ECS.Common");
        }
    }
}