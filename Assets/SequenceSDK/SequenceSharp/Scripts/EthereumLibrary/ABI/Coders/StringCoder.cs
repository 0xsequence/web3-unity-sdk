using System.Text;

namespace SequenceSharp.ABI
{

    public class StringCoder : ICoder
    {
        BytesCoder _bytesCoder = new BytesCoder();
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
        /// string: dynamic sized unicode string assumed to be UTF-8 encoded.
        /// string:
        ///       enc(X) = enc(enc_utf8(X)), i.e.X is UTF-8 encoded and this value is interpreted as of bytes type and encoded further.Note that the length used in this subsequent encoding is the number of bytes of the UTF-8 encoded string, not its number of characters.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public byte[] Encode(object value)
        {
            Encoding utf8 = Encoding.UTF8;
            
            return _bytesCoder.Encode(utf8.GetBytes((string)value));
        }

        public string EncodeToString(object value)
        {
            return SequenceCoder.ByteArrayToHexString(Encode(value));
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