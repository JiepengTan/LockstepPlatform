using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace BinarySerializer {
    /// <summary>
    /// 用于标记是否自动生成序列化代码
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public class ToBinaryAttribute : System.Attribute {
        /// <summary>
        /// 是否同时序列化所有子类
        /// </summary>
        public bool AllChildClass;
        /// <summary>
        /// 序列化的代码中是否需要命名空间避免冲突
        /// </summary>
        public bool IsNeedNameSpace;
        public ToBinaryAttribute(bool allChildClass = false, bool isNeedNameSpace = false) {
            this.AllChildClass = allChildClass;
            this.IsNeedNameSpace = isNeedNameSpace;
        }
    }

    /// <summary>
    /// 不序列化到文件中
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class NoToBinaryAttribute : System.Attribute { }

    /// <summary>
    /// 校验二进制文件时强制校验的字段
    /// </summary>
    public class ForceVerifyAttribute : System.Attribute { }
}

