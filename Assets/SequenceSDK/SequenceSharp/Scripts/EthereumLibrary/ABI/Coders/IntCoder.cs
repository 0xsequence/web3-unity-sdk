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
            
            byte[] valueBytes = ((BigInteger)number).ToByteArray();
            // Make sure Big Endian
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(valueBytes); // not 100% sure, check later
            }
            
            //Check number is signed int or unsigned

            throw new System.NotImplementedException();
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
        byte[] EncodeSignedInt(BigInteger number, int length)
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
        byte[] EncodeUnsignedInt(BigInteger number, int length)
        {
            var hex = number.ToString("x");
            string encodedString = new string('0', length - hex.Length) + hex;

            byte[] encoded = SequenceCoder.HexStringToByteArray(encodedString);
            return encoded;
        }


    }
}