using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SequenceSharp.ABI
{
    public class FixedBytesCoder : ICoder
    {
        IntCoder _intCoder = new IntCoder();
        public object Decode(byte[] encoded)
        {
            throw new System.NotImplementedException();
        }

        public T DefaultValue<T>()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// bytes<M>: binary type of M bytes, 0 < M <= 3
        /// bytes, of length k (which is assumed to be of type uint256):
        ///enc(X) = enc(k) pad_right(X), i.e.the number of bytes is encoded as a uint256 followed by the actual value of X as a byte sequence, followed by the minimum number of zero-bytes such that len(enc(X)) is a multiple of 32.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] Encode(object value)
        {
            string encodedString = EncodeToString(value);
            byte[] encoded = SequenceCoder.HexStringToByteArray(encodedString);
            return encoded;
        }

        public string EncodeToString(object value)
        {
            var numberOfBytes = ((byte[])value).Length;
            string numberOfBytesStr = _intCoder.EncodeUnsignedIntString(numberOfBytes, 64);
            // followed by the minimum number of zero-bytes such that len(enc(X)) is a multiple of 32
            int currentTotalLength = 64 + numberOfBytes;
            int zeroBytesNeeded = 64 - currentTotalLength % 64;
            int totalLength = currentTotalLength + zeroBytesNeeded;
            string valueStr = SequenceCoder.ByteArrayToHexString(((byte[])value));
            string encodedStr = (numberOfBytesStr + valueStr).PadRight(totalLength, '0');
            return encodedStr;
        }

        public string DecodeToString(byte[] encoded)
        {
            throw new System.NotImplementedException();
        }

        public bool IsSupportedType()
        {
            throw new System.NotImplementedException();
        }

    }
}