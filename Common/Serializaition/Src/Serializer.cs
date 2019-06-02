using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Lockstep.Serialization {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class Limited : Attribute {
        public bool le255;
        public bool le65535;

        public Limited(){
            le255 = false;
            le65535 = true;
        }

        public Limited(bool isLess255){
            le255 = isLess255;
            le65535 = !isLess255;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class SelfImplementAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class UdpAttribute : Attribute { }

    public interface ISerializable {
        void Serialize(Serializer writer);

        void Deserialize(Deserializer reader);
    }

    public class Serializer {
        protected byte[] _data;
        protected int _position;
        private const int InitialSize = 64;
        private int _capacity;

        public int Capacity {
            get { return _capacity; }
        }

        public Serializer() : this(true, InitialSize){ }

        public Serializer(int initialSize) : this(true, initialSize){ }

        private Serializer(bool autoResize, int initialSize){
            _data = new byte[initialSize];
            _capacity = initialSize;
        }

        /// <summary>
        /// Creates NetDataWriter from existing ByteArray
        /// </summary>
        /// <param name="bytes">Source byte array</param>
        /// <param name="copy">Copy array to new location or use existing</param>
        public static Serializer FromBytes(byte[] bytes, bool copy){
            if (copy) {
                var netDataWriter = new Serializer(true, bytes.Length);
                netDataWriter.Put(bytes);
                return netDataWriter;
            }

            return new Serializer(true, 0) {_data = bytes,_capacity = bytes.Length};
        }

        /// <summary>
        /// Creates NetDataWriter from existing ByteArray (always copied data)
        /// </summary>
        /// <param name="bytes">Source byte array</param>
        /// <param name="offset">Offset of array</param>
        /// <param name="length">Length of array</param>
        public static Serializer FromBytes(byte[] bytes, int offset, int length){
            var netDataWriter = new Serializer(true, bytes.Length);
            netDataWriter.Put(bytes, offset, length);
            return netDataWriter;
        }

        public static Serializer FromString(string value){
            var netDataWriter = new Serializer();
            netDataWriter.PutString(value);
            return netDataWriter;
        }

        public void ResizeIfNeed(int newSize){
            int len = _data.Length;
            if (len < newSize) {
                while (len < newSize)
                    len *= 2;
                Array.Resize(ref _data, len);
                _capacity = _data.Length;
            }
        }

        public void Reset(int size){
            ResizeIfNeed(size);
            _position = 0;
        }

        public void Reset(){
            _position = 0;
        }

        public byte[] CopyData(){
            byte[] resultData = new byte[_position];
            Buffer.BlockCopy(_data, 0, resultData, 0, _position);
            return resultData;
        }

        public byte[] Data {
            get { return _data; }
        }

        public int Length {
            get { return _position; }
        }

        public void PutSingle(float value){
            ResizeIfNeed(_position + 4);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 4;
        }

        public void PutDouble(double value){
            ResizeIfNeed(_position + 8);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 8;
        }

        public void PutInt64(long value){
            ResizeIfNeed(_position + 8);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 8;
        }

        public void PutUInt64(ulong value){
            ResizeIfNeed(_position + 8);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 8;
        }

        public void PutInt32(int value){
            ResizeIfNeed(_position + 4);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 4;
        }

        public void PutUInt32(uint value){
            ResizeIfNeed(_position + 4);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 4;
        }


        public void PutUInt16(ushort value){
            ResizeIfNeed(_position + 2);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 2;
        }

        public void PutInt16(short value){
            ResizeIfNeed(_position + 2);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 2;
        }

        public void PutSByte(sbyte value){
            ResizeIfNeed(_position + 1);
            _data[_position] = (byte) value;
            _position++;
        }

        public void PutByte(byte value){
            ResizeIfNeed(_position + 1);
            _data[_position] = value;
            _position++;
        }
        public void PutBoolean(bool value){
            ResizeIfNeed(_position + 1);
            _data[_position] = (byte) (value ? 1 : 0);
            _position++;
        }
        
        public void PutChar(char value){
            ResizeIfNeed(_position + 2);
            FastBitConverter.GetBytes(_data, _position, value);
            _position += 2;
        }

        public void PutIPEndPoint(IPEndPoint endPoint){
            PutString(endPoint.Address.ToString());
            PutInt32(endPoint.Port);
        }

        public void PutString(string value){
            if (string.IsNullOrEmpty(value)) {
                PutInt32(0);
                return;
            }

            //put bytes count
            int bytesCount = Encoding.UTF8.GetByteCount(value);
            ResizeIfNeed(_position + bytesCount + 4);
            PutInt32(bytesCount);

            //put string
            Encoding.UTF8.GetBytes(value, 0, value.Length, _data, _position);
            _position += bytesCount;
        }

        public void PutString(string value, int maxLength){
            if (string.IsNullOrEmpty(value)) {
                PutInt32(0);
                return;
            }

            int length = value.Length > maxLength ? maxLength : value.Length;
            //calculate max count
            int bytesCount = Encoding.UTF8.GetByteCount(value);
            ResizeIfNeed(_position + bytesCount + 4);

            //put bytes count
            PutInt32(bytesCount);

            //put string
            Encoding.UTF8.GetBytes(value, 0, length, _data, _position);

            _position += bytesCount;
        }
        
        public void Put(BaseFormater value){
            PutBoolean(value == null);
            value?.Serialize(this);
        }
        
        /// len should less then ushort.MaxValue
        public void PutBytes(byte[] value){
            ushort len = value == null ? (ushort) 0 : (ushort) value.Length;
            PutUInt16(len);
            Put(value);
        }
        
        public void PutArray(float[] value){
            if (BitConverter.IsLittleEndian) {__PutArrayFastLE(value, sizeof(float));return;}
            ushort len = value == null ? (ushort) 0 : (ushort) value.Length;
            ResizeIfNeed(_position + len * 4 + 2);
            PutUInt16(len);
            for (int i = 0; i < len; i++)
                PutSingle(value[i]);
        }

        public void PutArray(double[] value){
            if (BitConverter.IsLittleEndian) {__PutArrayFastLE(value, sizeof(double));return;}
            ushort len = value == null ? (ushort) 0 : (ushort) value.Length;
            ResizeIfNeed(_position + len * 8 + 2);
            PutUInt16(len);
            for (int i = 0; i < len; i++)
                PutDouble(value[i]);
        }

        public void PutArray(long[] value){
            if (BitConverter.IsLittleEndian) {__PutArrayFastLE(value, sizeof(long));return;}
            ushort len = value == null ? (ushort) 0 : (ushort) value.Length;
            ResizeIfNeed(_position + len * 8 + 2);
            PutUInt16(len);
            for (int i = 0; i < len; i++)
                PutInt64(value[i]);
        }

        public void PutArray(ulong[] value){
            if (BitConverter.IsLittleEndian) {__PutArrayFastLE(value, sizeof(ulong));return;}
            ushort len = value == null ? (ushort) 0 : (ushort) value.Length;
            ResizeIfNeed(_position + len * 8 + 2);
            PutUInt16(len);
            for (int i = 0; i < len; i++)
                PutUInt64(value[i]);
        }

        public void PutArray(int[] value){
            if (BitConverter.IsLittleEndian) {__PutArrayFastLE(value, sizeof(int));return;}
            ushort len = value == null ? (ushort) 0 : (ushort) value.Length;
            ResizeIfNeed(_position + len * 4 + 2);
            PutUInt16(len);
            for (int i = 0; i < len; i++)
                PutInt32(value[i]);
        }

        public void PutArray(uint[] value){
            if (BitConverter.IsLittleEndian) {__PutArrayFastLE(value, sizeof(uint));return;}
            ushort len = value == null ? (ushort) 0 : (ushort) value.Length;
            ResizeIfNeed(_position + len * 4 + 2);
            PutUInt16(len);
            for (int i = 0; i < len; i++)
                PutUInt32(value[i]);
        }

        public void PutArray(ushort[] value){
            if (BitConverter.IsLittleEndian) {__PutArrayFastLE(value, sizeof(ushort));return;}
            ushort len = value == null ? (ushort) 0 : (ushort) value.Length;
            ResizeIfNeed(_position + len * 2 + 2);
            PutUInt16(len);
            for (int i = 0; i < len; i++)
                PutUInt16(value[i]);
        }

        public void PutArray(short[] value){
            if (BitConverter.IsLittleEndian) {__PutArrayFastLE(value, sizeof(short));return;}
            ushort len = value == null ? (ushort) 0 : (ushort) value.Length;
            ResizeIfNeed(_position + len * 2 + 2);
            PutUInt16(len);
            for (int i = 0; i < len; i++)
                PutInt16(value[i]);
        }

        public void PutArray(bool[] value){
            if (BitConverter.IsLittleEndian) {__PutArrayFastLE(value, sizeof(bool));return;}
            ushort len = value == null ? (ushort) 0 : (ushort) value.Length;
            ResizeIfNeed(_position + len + 2);
            PutUInt16(len);
            for (int i = 0; i < len; i++)
                PutBoolean(value[i]);
        }

        public void PutArray(string[] value){
            ushort len = value == null ? (ushort) 0 : (ushort) value.Length;
            PutUInt16(len);
            for (int i = 0; i < len; i++)
                PutString(value[i]);
        }

        public void PutArray(string[] value, int maxLength){
            ushort len = value == null ? (ushort) 0 : (ushort) value.Length;
            PutUInt16(len);
            for (int i = 0; i < len; i++)
                PutString(value[i], maxLength);
        }


        public void PutArray(BaseFormater[] value){
            ushort len = (ushort) (value?.Length ?? 0);
            PutUInt16(len);
            for (int i = 0; i < len; i++) {
                var val = value[i];
                PutBoolean(val == null);
                val?.Serialize(this);
            }
        }


        public void PutArray(byte[] value){
            var isNull = value == null;
            PutBoolean(isNull);
            if (isNull) return;
            PutInt32(value.Length);
            Put(value);
        }

        public void PutBytes_255(byte[] value){
            if (value == null) {
                PutByte((byte) 0);
                return;
            }

            if (value.Length > byte.MaxValue) {
                throw new ArgumentOutOfRangeException($"Input Cmd len should less then {byte.MaxValue}");
            }

            PutByte((byte) value.Length);
            Put(value);
        }
        
        private void Put(byte[] data, int offset, int length){
            ResizeIfNeed(_position + length);
            Buffer.BlockCopy(data, offset, _data, _position, length);
            _position += length;
        }

        private void Put(byte[] data){
            ResizeIfNeed(_position + data.Length);
            Buffer.BlockCopy(data, 0, _data, _position, data.Length);
            _position += data.Length;
        }

        private void __PutArrayFastLE<T>(T[] x, int elemSize)where T : struct{
            ushort len = x == null ? (ushort) 0 : (ushort) x.Length;
            int bytesCount = elemSize * len;
            ResizeIfNeed(_position + 2 + bytesCount);
            FastBitConverter.GetBytes(_data, _position, len);
            _position += 2;
            if (len == 0) {
                return;
            }

            ResizeIfNeed(_position + bytesCount);
            // if we are LE, just do a block copy
            Buffer.BlockCopy(x, 0, _data, _position, bytesCount);
        }
    }
}