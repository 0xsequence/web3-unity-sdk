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

        public byte[] Encode(object value)
        {
            byte[] valueBytes = ((BigInteger)value).ToByteArray();
            // Make sure Big Endian
            if(BitConverter.IsLittleEndian)
            {
                Array.Reverse(valueBytes); // not 100% sure, check later
            }
            // Check bounds are safe for encoding
            //TODO: if length > 32, recursive call:
            //else

            //Encode

            //if signed



            var encoded = new byte[32];
            for( int i = 0; i < encoded.Length; i++ )
            {

            }
            throw new System.NotImplementedException();
        }

        public bool IsSupportedType()
        {
            throw new System.NotImplementedException();
        }
    }
}