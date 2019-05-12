using Lockstep.Math;
using Lockstep.Serialization;

namespace Lockstep.NetMsg.Lobby{
    public static partial class LMathExtensions {
        public static LFloat GetLFloat(this Deserializer reader){
            var x = reader.GetInt();
            return new LFloat(true, x);
        }

        public static LVector2 GetLVector2(this Deserializer reader){
            var x = reader.GetInt();
            var y = reader.GetInt();
            return new LVector2(true, x, y);
        }

        public static LVector3 GetLVector3(this Deserializer reader){
            var x = reader.GetInt();
            var y = reader.GetInt();
            var z = reader.GetInt();
            return new LVector3(true, x, y, z);
        }

        public static void Put(this Serializer writer, LFloat value){
            writer.Put(value._val);
        }

        public static void Put(this Serializer writer, LVector2 value){
            writer.Put(value._x);
            writer.Put(value._y);
        }

        public static void Put(this Serializer writer, LVector3 value){
            writer.Put(value._x);
            writer.Put(value._y);
            writer.Put(value._z);
        }
    }
}