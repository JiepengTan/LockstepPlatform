using System;
using System.Collections.Generic;
using System.Text;

namespace Lockstep.Serialization
{
    public class Deserializer
    {
        protected byte[] _data;
        protected int _position;
        protected int _dataSize;
        private int _offset;

        public byte[] RawData
        {
            get { return _data; }
        }

        public int RawDataSize
        {
            get { return _dataSize; }
        }

        public int UserDataOffset
        {
            get { return _offset; }
        }

        public int UserDataSize
        {
            get { return _dataSize - _offset; }
        }

        public bool IsNull
        {
            get { return _data == null; }
        }

        public int Position
        {
            get { return _position; }
        }

        public bool EndOfData
        {
            get { return _position == _dataSize; }
        }

        public int AvailableBytes
        {
            get { return _dataSize - _position; }
        }

        public bool IsEnd {
            get { return _dataSize == _position; }
        }

        public void SetSource(Serializer dataWriter)
        {
            _data = dataWriter.Data;
            _position = 0;
            _offset = 0;
            _dataSize = dataWriter.Length;
        }

        public void SetSource(byte[] source)
        {
            _data = source;
            _position = 0;
            _offset = 0;
            _dataSize = source.Length;
        }

        public void SetSource(byte[] source, int offset)
        {
            _data = source;
            _position = offset;
            _offset = offset;
            _dataSize = source.Length;
        }

        public void SetSource(byte[] source, int offset, int maxSize)
        {
            _data = source;
            _position = offset;
            _offset = offset;
            _dataSize = maxSize;
        }

        public Deserializer()
        {

        }

        public Deserializer(byte[] source)
        {
            SetSource(source);
        }

        public Deserializer(byte[] source, int offset)
        {
            SetSource(source, offset);
        }

        public Deserializer(byte[] source, int offset, int maxSize)
        {
            SetSource(source, offset, maxSize);
        }

        #region GetMethods      
        public byte GetByte()
        {
            byte res = _data[_position];
            _position += 1;
            return res;
        }

        public sbyte GetSByte()
        {
            var b = (sbyte)_data[_position];
            _position++;
            return b;
        }
      
        public bool[] GetBoolArray()
        {
            ushort size = FastBitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new bool[size];
            for (int i = 0; i < size; i++)
            {
                arr[i] = GetBoolean();
            }
            return arr;
        }

        public ushort[] GetUShortArray()
        {
            ushort size = FastBitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new ushort[size];
            for (int i = 0; i < size; i++)
            {
                arr[i] = GetUInt16();
            }
            return arr;
        }

        public short[] GetShortArray()
        {
            ushort size = FastBitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new short[size];
            for (int i = 0; i < size; i++)
            {
                arr[i] = GetInt16();
            }
            return arr;
        }

        public long[] GetLongArray()
        {
            ushort size = FastBitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new long[size];
            for (int i = 0; i < size; i++)
            {
                arr[i] = GetInt64();
            }
            return arr;
        }

        public ulong[] GetULongArray()
        {
            ushort size = FastBitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new ulong[size];
            for (int i = 0; i < size; i++)
            {
                arr[i] = GetUInt64();
            }
            return arr;
        }

        public int[] GetIntArray()
        {
            ushort size = FastBitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new int[size];
            for (int i = 0; i < size; i++)
            {
                arr[i] = GetInt32();
            }
            return arr;
        }

        public uint[] GetUIntArray()
        {
            ushort size = FastBitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new uint[size];
            for (int i = 0; i < size; i++)
            {
                arr[i] = GetUInt32();
            }
            return arr;
        }

        public float[] GetFloatArray()
        {
            ushort size = FastBitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new float[size];
            for (int i = 0; i < size; i++)
            {
                arr[i] = GetSingle();
            }
            return arr;
        }

        public double[] GetDoubleArray()
        {
            ushort size = FastBitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new double[size];
            for (int i = 0; i < size; i++)
            {
                arr[i] = GetDouble();
            }
            return arr;
        }

        public string[] GetStringArray()
        {
            ushort size = FastBitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new string[size];
            for (int i = 0; i < size; i++)
            {
                arr[i] = GetString();
            }
            return arr;
        }

        public string[] GetStringArray(int maxStringLength)
        {
            ushort size = FastBitConverter.ToUInt16(_data, _position);
            _position += 2;
            var arr = new string[size];
            for (int i = 0; i < size; i++)
            {
                arr[i] = GetString(maxStringLength);
            }
            return arr;
        }

        public byte[] GetArray(ref byte[] _){return GetBytes();}
        public short[] GetArray(ref short[] _){return GetShortArray();}
        public ushort[] GetArray(ref ushort[] _){return GetUShortArray();}
        public int[] GetArray(ref int[] _){return GetIntArray();}
        public uint[] GetArray(ref uint[] _){return GetUIntArray();}
        public long[] GetArray(ref long[] _){return GetLongArray();}
        public ulong[] GetArray(ref ulong[] _){return GetULongArray();}
        public float[] GetArray(ref float[] _){return GetFloatArray();}
        public double[] GetArray(ref double[] _){return GetDoubleArray();}
        public bool[] GetArray(ref bool[] _){return GetBoolArray();}
        public string[] GetArray(ref string[] _){return GetStringArray();}
        

        public T[] GetArray<T>(ref T[] _) where T:BaseFormater ,new ()
        {
            ushort len = GetUInt16();
            if (len == 0)
                return null;
            var formatters = new T[len];
            for (int i = 0; i < len; i++) {
                if (GetBoolean())
                    formatters[i] = null;
                else {
                    var val = new T();
                    val.Deserialize(this);
                    formatters[i] = val;
                }
            }
            return formatters;
        }
        public List<T> GetList<T>(ref List<T> _) where T:BaseFormater ,new ()
        {
            ushort len = GetUInt16();
            if (len == 0)
                return null;
            var formatters = new List<T>(len);
            for (int i = 0; i < len; i++) {
                if (GetBoolean())
                    formatters[i] = null;
                else {
                    var val = new T();
                    val.Deserialize(this);
                    formatters[i] = val;
                }
            }
            return formatters;
        }
        public bool GetBoolean()
        {
            bool res = _data[_position] > 0;
            _position += 1;
            return res;
        }

        public char GetChar()
        {
            char result =(char) FastBitConverter.ToInt16(_data, _position);
            _position += 2;
            return result;
        }

        public ushort GetUInt16()
        {
            ushort result = FastBitConverter.ToUInt16(_data, _position);
            _position += 2;
            return result;
        }

        public short GetInt16()
        {
            short result = FastBitConverter.ToInt16(_data, _position);
            _position += 2;
            return result;
        }

        public long GetInt64()
        {
            long result = FastBitConverter.ToInt64(_data, _position);
            _position += 8;
            return result;
        }

        public ulong GetUInt64()
        {
            ulong result = FastBitConverter.ToUInt64(_data, _position);
            _position += 8;
            return result;
        }

        public int GetInt32()
        {
            int result = FastBitConverter.ToInt32(_data, _position);
            _position += 4;
            return result;
        }

        public uint GetUInt32()
        {
            uint result = FastBitConverter.ToUInt32(_data, _position);
            _position += 4;
            return result;
        }

        public float GetSingle()
        {
            float result = FastBitConverter.ToSingle(_data, _position);
            _position += 4;
            return result;
        }

        public double GetDouble()
        {
            double result = FastBitConverter.ToDouble(_data, _position);
            _position += 8;
            return result;
        }
        public T Get<T>(ref T _) where T:BaseFormater ,new(){
            if (GetBoolean())
                return null;
            var val = new T();
            val.Deserialize(this);
            return val;
        }
        public string GetString(int maxLength)
        {
            int bytesCount = GetInt32();
            if (bytesCount <= 0 || bytesCount > maxLength*2)
            {
                return string.Empty;
            }

            int charCount = Encoding.UTF8.GetCharCount(_data, _position, bytesCount);
            if (charCount > maxLength)
            {
                return string.Empty;
            }

            string result = Encoding.UTF8.GetString(_data, _position, bytesCount);
            _position += bytesCount;
            return result;
        }

        public string GetString()
        {
            int bytesCount = GetInt32();
            if (bytesCount <= 0)
            {
                return string.Empty;
            }

            string result = Encoding.UTF8.GetString(_data, _position, bytesCount);
            _position += bytesCount;
            return result;
        }

        public byte[] GetRemainingBytes()
        {
            byte[] outgoingData = new byte[AvailableBytes];
            Buffer.BlockCopy(_data, _position, outgoingData, 0, AvailableBytes);
            _position = _data.Length;
            return outgoingData;
        }

        public void GetBytes(byte[] destination, int start, int count)
        {
            Buffer.BlockCopy(_data, _position, destination, start, count);
            _position += count;
        }

        public void GetBytes(byte[] destination, int count)
        {
            Buffer.BlockCopy(_data, _position, destination, 0, count);
            _position += count;
        }
        public byte[] GetBytes()
        {
            ushort size = GetUInt16();
            if (size == 0) return null;
            var outgoingData = new byte[size];
            Buffer.BlockCopy(_data, _position, outgoingData, 0, size);
            _position += size;
            return outgoingData;
        }      
        public byte[] GetBytes_255()
        {
            ushort size = GetByte();
            if (size == 0) return null;
            var outgoingData = new byte[size];
            Buffer.BlockCopy(_data, _position, outgoingData, 0, size);
            _position += size;
            return outgoingData;
        }
        
        #endregion

        #region PeekMethods

        public byte PeekByte()
        {
            return _data[_position];
        }

        public sbyte PeekSByte()
        {
            return (sbyte)_data[_position];
        }

        public bool PeekBool()
        {
            return _data[_position] > 0;
        }

        public char PeekChar()
        {
            return (char) FastBitConverter.ToInt16(_data, _position);
        }

        public ushort PeekUShort()
        {
            return FastBitConverter.ToUInt16(_data, _position);
        }

        public short PeekShort()
        {
            return FastBitConverter.ToInt16(_data, _position);
        }

        public long PeekLong()
        {
            return FastBitConverter.ToInt64(_data, _position);
        }

        public ulong PeekULong()
        {
            return FastBitConverter.ToUInt64(_data, _position);
        }

        public int PeekInt()
        {
            return FastBitConverter.ToInt32(_data, _position);
        }

        public uint PeekUInt()
        {
            return FastBitConverter.ToUInt32(_data, _position);
        }

        public float PeekFloat()
        {
            return FastBitConverter.ToSingle(_data, _position);
        }

        public double PeekDouble()
        {
            return FastBitConverter.ToDouble(_data, _position);
        }

        public string PeekString(int maxLength)
        {
            int bytesCount = BitConverter.ToInt32(_data, _position);
            if (bytesCount <= 0 || bytesCount > maxLength * 2)
            {
                return string.Empty;
            }

            int charCount = Encoding.UTF8.GetCharCount(_data, _position + 4, bytesCount);
            if (charCount > maxLength)
            {
                return string.Empty;
            }

            string result = Encoding.UTF8.GetString(_data, _position + 4, bytesCount);
            return result;
        }

        public string PeekString()
        {
            int bytesCount = BitConverter.ToInt32(_data, _position);
            if (bytesCount <= 0)
            {
                return string.Empty;
            }

            string result = Encoding.UTF8.GetString(_data, _position + 4, bytesCount);
            return result;
        }
        #endregion

        #region TryGetMethods
        public bool TryGetByte(out byte result)
        {
            if (AvailableBytes >= 1)
            {
                result = GetByte();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryGetSByte(out sbyte result)
        {
            if (AvailableBytes >= 1)
            {
                result = GetSByte();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryGetBool(out bool result)
        {
            if (AvailableBytes >= 1)
            {
                result = GetBoolean();
                return true;
            }
            result = false;
            return false;
        }

        public bool TryGetChar(out char result)
        {
            if (AvailableBytes >= 2)
            {
                result = GetChar();
                return true;
            }
            result = '\0';
            return false;
        }

        public bool TryGetShort(out short result)
        {
            if (AvailableBytes >= 2)
            {
                result = GetInt16();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryGetUShort(out ushort result)
        {
            if (AvailableBytes >= 2)
            {
                result = GetUInt16();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryGetInt(out int result)
        {
            if (AvailableBytes >= 4)
            {
                result = GetInt32();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryGetUInt(out uint result)
        {
            if (AvailableBytes >= 4)
            {
                result = GetUInt32();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryGetLong(out long result)
        {
            if (AvailableBytes >= 8)
            {
                result = GetInt64();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryGetULong(out ulong result)
        {
            if (AvailableBytes >= 8)
            {
                result = GetUInt64();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryGetFloat(out float result)
        {
            if (AvailableBytes >= 4)
            {
                result = GetSingle();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryGetDouble(out double result)
        {
            if (AvailableBytes >= 8)
            {
                result = GetDouble();
                return true;
            }
            result = 0;
            return false;
        }

        public bool TryGetString(out string result)
        {
            if (AvailableBytes >= 4)
            {
                var bytesCount = PeekInt();
                if (AvailableBytes >= bytesCount + 4)
                {
                    result = GetString();
                    return true;
                }
            }
            result = null;
            return false;
        }

        public bool TryGetStringArray(out string[] result)
        {
            ushort size;
            if (!TryGetUShort(out size))
            {
                result = null;
                return false;
            }

            result = new string[size];
            for (int i = 0; i < size; i++)
            {
                if (!TryGetString(out result[i]))
                {
                    result = null;
                    return false;
                }
            }

            return true;
        }

        #endregion

        public void Clear()
        {
            _position = 0;
            _dataSize = 0;
            _data = null;
        }
    }
}

