using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Lockstep.Logging;

namespace Lockstep.ECS.UnsafeECS.CodeGen {
    public class CodeGenAsset : CodeGenEntity {
        protected override string ClsPrototype => @"
    [StructLayoutAttribute(LayoutKind.Sequential, Pack=4)]
    public unsafe partial struct #CLS_NAME :IAsset {
#ATTRIS_DEFINE
    }
";
        protected override bool FilterFunc(Type type){
            return typeof(ECDefine.IAsset).IsAssignableFrom(type);
        }

    }
}