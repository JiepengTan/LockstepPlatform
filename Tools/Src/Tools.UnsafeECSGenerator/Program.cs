using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading;
using Lockstep.ECSGenerator;
using Lockstep.Util;

internal class Program {
    public static void Main(string[] args){
        Console.WriteLine("pwd " + AppDomain.CurrentDomain.BaseDirectory);
        if (args == null || args.Length == 0) {
            args = new[] {"../Config/ECSGenerator/UnsafeECS.json"};    
        }
        if (args.Length > 0) {
            foreach (var path in args) {
                Console.WriteLine(path);
                CodeGenForUnsafeECS.GenCode(Path.Combine(AppDomain.CurrentDomain.BaseDirectory + path));
            }
        }
        else {
            Console.WriteLine("Need config path");
        }
    }
}