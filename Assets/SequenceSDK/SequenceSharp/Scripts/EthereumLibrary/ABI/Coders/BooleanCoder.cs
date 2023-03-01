namespace SequenceSharp.ABI
{

    public class BooleanCoder : ICoder
    {
        public object Decode(byte[] encoded)
        {
            int length = encoded.Length;
            if (encoded[length - 1] == 1) return true;
            return false;
        }

        public T DefaultValue<T>()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// as in the uint8 case, where 1 is used for true and 0 for false
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] Encode(object value)
        {
            if((bool)value)
            {
                return SequenceCoder.HexStringToByteArray(new string('0', 63) + '1');
            }
            return SequenceCoder.HexStringToByteArray(new string('0', 63) + '0');
        }

        public bool IsSupportedType()
        {
            throw new System.NotImplementedException();
        }
    }
}