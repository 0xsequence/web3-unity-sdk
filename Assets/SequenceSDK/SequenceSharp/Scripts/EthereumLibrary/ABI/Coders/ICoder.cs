namespace SequenceSharp.ABI
{
    public interface ICoder
    {
        byte[] Encode(object value);
        object Decode(byte[] encoded);
        bool IsSupportedType();
        T DefaultValue<T>();
    }
}