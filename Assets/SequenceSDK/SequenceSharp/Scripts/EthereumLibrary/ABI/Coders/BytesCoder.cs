using System;

namespace SequenceSharp.ABI
{
    /// <summary>
    /// Note that in the dynamic case, head(X(i)) is well-defined since the lengths of the head parts only depend on the types and not the values. The value of head(X(i)) is the offset of the beginning of tail(X(i)) relative to the start of enc(X).
    /// </summary>
    public class BytesCoder : ICoder
    {
        IntCoder _intCoder;
        public T Decode<T>(byte[] encoded)
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
            //head:
            //The value of head(X(i)) is the offset of the beginning of tail(X(i)) relative to the start of enc(X).
            var offset = _intCoder.EncodeUnsignedInt(32, 64); // TODO: hardcoded, will change later
            var numberOfBytes = ((byte[])value).Length;
            //uint256 -> 32 bytes -> 64 hex
            byte[] numberOfBytesByteArray = _intCoder.EncodeUnsignedInt(numberOfBytes, 64);

            // followed by the minimum number of zero-bytes such that len(enc(X)) is a multiple of 32
            int currentTotalLength = offset.Length + numberOfBytes + numberOfBytesByteArray.Length;
            int zeroBytesNeeded = 32 - currentTotalLength % 32;

            int totalLength = currentTotalLength + zeroBytesNeeded;
            byte[] encodedByteArray = new byte[totalLength];

            //copy offset
            Array.Copy(offset, 0, encodedByteArray, 0, offset.Length);
            //copy number of bytes
            Array.Copy(numberOfBytesByteArray, 0, encodedByteArray, offset.Length, numberOfBytesByteArray.Length);
            //copy encoded bytes:
            Array.Copy((byte[])value, 0, encodedByteArray, offset.Length + numberOfBytesByteArray.Length, numberOfBytes);
            //add 0s
            for (int i = offset.Length + numberOfBytesByteArray.Length + numberOfBytes; i < totalLength; i++) encodedByteArray[i] = 0;
            return encodedByteArray;

        }

        public bool IsSupportedType()
        {
            throw new System.NotImplementedException();
        }
    }
}