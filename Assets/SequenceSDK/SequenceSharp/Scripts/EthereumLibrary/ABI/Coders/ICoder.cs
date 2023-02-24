namespace SequenceSharp.ABI
{

    public interface ICoder
    {
        byte[] Encode(object value);
        T Decode<T>(byte[] encoded);
        bool IsSupportedType();
        T DefaultValue<T>();
    }
}