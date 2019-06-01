using System;
using Lockstep.Logging;

namespace Lockstep.Networking {
    public class ByteHelper {
        public static void CopyBytes(short value, byte[] buffer, int index){
            CopyBytesImpl(value, 2, buffer, index);
        }

        /// <summary>
        ///     Copies the specified 32-bit signed integer value into the specified byte array,
        ///     beginning at the specified index.
        /// </summary>
        /// <param name="value">The number to convert.</param>
        /// <param name="buffer">The byte array to copy the bytes into</param>
        /// <param name="index">The first index into the array to copy the bytes into</param>
        public static void CopyBytes(int value, byte[] buffer, int index){
            CopyBytesImpl(value, 4, buffer, index);
        }

        protected static void CopyBytesImpl(long value, int bytes, byte[] buffer, int index){
            for (var i = 0; i < bytes; i++) {
                buffer[i + index] = unchecked((byte) (value & 0xff));
                value = value >> 8;
            }
        }

        /// <summary>
        ///     Returns a 16-bit signed integer converted from two bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 16-bit signed integer formed by two bytes beginning at startIndex.</returns>
        public static short ToInt16(byte[] value, int startIndex){
            return unchecked((short) CheckedFromBytes(value, startIndex, 2));
        }

        /// <summary>
        ///     Returns a 32-bit signed integer converted from four bytes at a specified position in a byte array.
        /// </summary>
        /// <param name="value">An array of bytes.</param>
        /// <param name="startIndex">The starting position within value.</param>
        /// <returns>A 32-bit signed integer formed by four bytes beginning at startIndex.</returns>
        public static int ToInt32(byte[] value, int startIndex){
            return unchecked((int) CheckedFromBytes(value, startIndex, 4));
        }

        private static long CheckedFromBytes(byte[] value, int startIndex, int bytesToConvert){
            CheckByteArgument(value, startIndex, bytesToConvert);
            return FromBytes(value, startIndex, bytesToConvert);
        }

        /// <summary>
        ///     Checks the given argument for validity.
        /// </summary>
        /// <param name="value">The byte array passed in</param>
        /// <param name="startIndex">The start index passed in</param>
        /// <param name="bytesRequired">The number of bytes required</param>
        /// <exception cref="ArgumentNullException">value is a null reference</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     startIndex is less than zero or greater than the length of value minus bytesRequired.
        /// </exception>
        private static void CheckByteArgument(byte[] value, int startIndex, int bytesRequired){
            if (value == null)
                throw new ArgumentNullException("value");
            if ((startIndex < 0) || (startIndex > value.Length - bytesRequired))
                throw new ArgumentOutOfRangeException("startIndex");
        }

        /// <summary>
        ///     Returns a value built from the specified number of bytes from the given buffer,
        ///     starting at index.
        /// </summary>
        /// <param name="buffer">The data in byte array format</param>
        /// <param name="startIndex">The first index to use</param>
        /// <param name="bytesToConvert">The number of bytes to use</param>
        /// <returns>The value built from the given bytes</returns>
        protected static long FromBytes(byte[] buffer, int startIndex, int bytesToConvert){
            long ret = 0;
            for (var i = 0; i < bytesToConvert; i++)
                ret = unchecked((ret << 8) | buffer[startIndex + bytesToConvert - 1 - i]);
            return ret;
        }
    }

    public class MessageFactory : IMessageFactory {
        public IMessage Create(short opCode){
            return new Message(opCode);
        }

        public IMessage Create(short opCode, byte[] data){
            return new Message(opCode, data);
        }

        /// <summary>
        ///     Used raw byte data to create an <see cref="IIncommingMessage" />
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="start"></param>
        /// <param name="peer"></param>
        /// <returns></returns>
        public IIncommingMessage FromBytes(byte[] buffer, int start, IPeer peer){
            try {
                //var converter = EndianBitConverter.Big;
                var flags = buffer[start];
                var opCode = ByteHelper.ToInt16(buffer, start + 1);
                var pointer = start + 3;

                var dataLength = ByteHelper.ToInt32(buffer, pointer);
                pointer += 4;
                var data = new byte[dataLength];
                Array.Copy(buffer, pointer, data, 0, dataLength);
                pointer += dataLength;

                var message = new IncommingMessage(opCode, flags, data, DeliveryMethod.Reliable, peer) {
                    SequenceChannel = 0
                };

                if ((flags & (byte) MessageFlag.AckRequest) > 0) {
                    // We received a message which requests a response
                    message.AckResponseId = ByteHelper.ToInt32(buffer, pointer);
                    pointer += 4;
                }

                if ((flags & (byte) MessageFlag.AckResponse) > 0) {
                    // We received a message which is a response to our ack request
                    var ackId = ByteHelper.ToInt32(buffer, pointer);
                    message.AckRequestId = ackId;
                    pointer += 4;

                    var statusCode = buffer[pointer];

                    message.Status =
                        (ResponseStatus) statusCode; // TODO look into not exposing status code / ackRequestId
                    pointer++;
                }

                return message;
            }
            catch (Exception e) {
                Debug.LogError("WS Failed parsing an incoming message " + e);
            }

            return null;
        }
    }
}