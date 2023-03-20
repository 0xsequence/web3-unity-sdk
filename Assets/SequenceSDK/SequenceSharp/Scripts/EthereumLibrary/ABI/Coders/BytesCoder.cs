using System;
using System.Collections.Generic;

namespace SequenceSharp.ABI
{
    /// <summary>
    /// Note that in the dynamic case, head(X(i)) is well-defined since the lengths of the head parts only depend on the types and not the values. The value of head(X(i)) is the offset of the beginning of tail(X(i)) relative to the start of enc(X).
    /// </summary>
    public class BytesCoder : ICoder
    {
        FixedBytesCoder _fixedBytesCoder = new FixedBytesCoder();
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
            string headStr = _intCoder.EncodeToString(32); 
            string bytesStr = _fixedBytesCoder.EncodeToString(value);
            return headStr + bytesStr;
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