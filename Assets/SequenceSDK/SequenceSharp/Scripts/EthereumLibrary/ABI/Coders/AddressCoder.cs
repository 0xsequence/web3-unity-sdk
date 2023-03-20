
using System.Globalization;
using System.Numerics;

namespace SequenceSharp.ABI
{
    /// <summary>
    /// address: equivalent to uint160, except for the assumed interpretation and language typing. For computing the function selector, address is used.
    /// </summary>
    public class AddressCoder : ICoder
    {
        public object Decode(byte[] encoded)
        {
            throw new System.NotImplementedException();
        }

        public T DefaultValue<T>()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// address: as in the uint160 case
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] Encode(object value)
        {
            //Trim 0x at the start
            string encodedString = EncodeToString(value);
            byte[] encoded = SequenceCoder.HexStringToByteArray(encodedString);
            return encoded;
        }

        public string EncodeToString(object value)
        {
            string address = (string)value;
            if (address.StartsWith("0x"))
            {
                address = address.Remove(0, 2);
            }

            string encodedString = new string('0', 64 - address.Length) + address;
            return encodedString.ToLower();
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