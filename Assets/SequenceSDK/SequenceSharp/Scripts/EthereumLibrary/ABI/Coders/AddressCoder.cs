
using System.Globalization;
using System.Numerics;

namespace SequenceSharp.ABI
{

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
            string address = (string)value;
            if (address.StartsWith("0x")) address.Remove(0, 2);

            string encodedString = new string('0', 64 - address.Length) + address;
            byte[] encoded = SequenceCoder.HexStringToByteArray(encodedString);
            return encoded;
        }

        public bool IsSupportedType()
        {
            throw new System.NotImplementedException();
        }
    }
}