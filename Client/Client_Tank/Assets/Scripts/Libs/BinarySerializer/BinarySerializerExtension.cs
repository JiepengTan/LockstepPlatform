using System;
using Lockstep.Math;

namespace BinarySerializer {
    public static partial class BinarySerializer
    {
        public static void RegisterExtensionTypes()
        {
            RegisterInnerReaderWriter(ReadLFloat, WriteLFloat);
            RegisterInnerReaderWriter(ReadLVector2, WriteLVector2);
            RegisterInnerReaderWriter(ReadLVector3, WriteLVector3);
        }
    }
}


namespace BinarySerializer {
    //Lockstep.Math  extension
    public static partial class BinarySerializer {
        public static void WriteLFloat(System.IO.BinaryWriter stream, LFloat val){
            stream.Write(val._val);
        }
        public static void WriteLVector2(System.IO.BinaryWriter stream, LVector2 val){
            WriteInt32(stream, val._x);
            WriteInt32(stream, val._y);
        }      
        public static void WriteLVector3(System.IO.BinaryWriter stream, LVector3 val){
            WriteInt32(stream, val._x);
            WriteInt32(stream, val._y);
            WriteInt32(stream, val._z);
        }


        public static LFloat ReadLFloat(byte[] fieldData, ref int cursor){
            var x = BitConverter.ToInt32(fieldData, cursor);
            return new LFloat(true,x);
        }

        public static LVector2 ReadLVector2(byte[] fieldData, ref int cursor){
            var x = BitConverter.ToInt32(fieldData, cursor);
            cursor += 4;
            var y = BitConverter.ToInt32(fieldData, cursor);
            cursor += 4;
            return new LVector2(true,x,y);
        }

        public static LVector3 ReadLVector3(byte[] fieldData, ref int cursor){
            var x = BitConverter.ToInt32(fieldData, cursor);
            cursor += 4;
            var y = BitConverter.ToInt32(fieldData, cursor);
            cursor += 4;
            var z = BitConverter.ToInt32(fieldData, cursor);
            cursor += 4;
            return new LVector3(true,x,y,z);
        }
    }
}
