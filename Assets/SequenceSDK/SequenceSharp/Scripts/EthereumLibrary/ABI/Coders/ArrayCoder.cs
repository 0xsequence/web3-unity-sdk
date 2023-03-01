namespace SequenceSharp.ABI
{
    /// <summary>
    /// T[k] for any T and k:
    /// enc(X) = enc((X[0], ..., X[k-1]))
    /// i.e. it is encoded as if it were a tuple with k elements of the same type.
    /// T[] where X has k elements (k is assumed to be of type uint256):
    /// enc(X) = enc(k) enc([X[0], ..., X[k-1]])
    /// i.e. it is encoded as if it were an array of static size k, prefixed with the number of elements.
    /// </summary>
    public class ArrayCoder : ICoder
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
            throw new System.NotImplementedException();
        }

        public bool IsSupportedType()
        {
            throw new System.NotImplementedException();
        }

        public byte[] EncodeStatic(object value, int k)
        {
            throw new System.NotImplementedException();
        }

        
    }
}
