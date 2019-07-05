using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Lockstep.ECSGenerator;
using Lockstep.Util;

internal class Program {
    public static void Main(string[] args){
        CodeGenForEntitas.GenCode();
    }
}