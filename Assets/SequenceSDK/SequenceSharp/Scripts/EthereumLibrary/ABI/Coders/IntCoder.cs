using System;
using System.Numerics;

namespace SequenceSharp.ABI
{

    public class IntCoder : ICoder
    {
        public T Decode<T>(byte[] encoded)
        {
            throw new System.NotImplementedException();
        }

        public T DefaultValue<T>()
        {
            throw new System.NotImplementedException();
        }

        public byte[] Encode(object number)
        {

            //The BigInteger structure does not include constructors with a parameter of type Byte, Int16, SByte, or UInt16. However, the Int32 type supports the implicit conversion of 8-bit and 16-bit signed and unsigned integers to signed 32-bit integers.

            byte[] encodedInt = EncodeSignedInt((BigInteger)number, 64);
            // Make sure Big Endian
            /*if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(encodedInt); // not 100% sure, check later
            }*/


            return encodedInt;

        }

        public bool IsSupportedType()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// int<M>: enc(X) is the big-endian two’s complement encoding of X, padded on the higher-order (left) side with 0xff bytes for negative X and with zero-bytes for non-negative X such that the length is 32 bytes.
        /// </summary>
        /// <param name="number"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] EncodeSignedInt(BigInteger number, int length)
        {
            var hex = number.ToString("x");
            string encodedString;
            if (number.Sign > 0)
            {
                encodedString = new string('0', length - hex.Length) + hex;
            }
            else
            {
                encodedString = new string('f', length - hex.Length) + hex;
            }
            byte[] encoded = SequenceCoder.HexStringToByteArray(encodedString);
            return encoded;
        }

        /// <summary>
        /// uint<M>: enc(X) is the big-endian encoding of X, padded on the higher-order (left) side with zero-bytes such that the length is 32 bytes.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public byte[] EncodeUnsignedInt(BigInteger number, int length)
        {
            var hex = number.ToString("x");
            string encodedString = new string('0', length - hex.Length) + hex;

            byte[] encoded = SequenceCoder.HexStringToByteArray(encodedString);
            return encoded;
        }


    }
}