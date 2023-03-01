namespace SequenceSharp.ABI
{

    public class StringCoder : ICoder
    {
        public T Decode<T>(byte[] encoded)
        {
            throw new System.NotImplementedException();
        }

        public T DefaultValue<T>()
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// string: dynamic sized unicode string assumed to be UTF-8 encoded.
        /// string:
        ///       enc(X) = enc(enc_utf8(X)), i.e.X is UTF-8 encoded and this value is interpreted as of bytes type and encoded further.Note that the length used in this subsequent encoding is the number of bytes of the UTF-8 encoded string, not its number of characters.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] Encode(object value)
        {
            throw new System.NotImplementedException();
        }

        public bool IsSupportedType()
        {
            throw new System.NotImplementedException();
        }
    }
}