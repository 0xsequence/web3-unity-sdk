
namespace SequenceSharp
{
    public class RpcResponse<T>
    {
        public int Id { get; set; }
        public T Result { get; set; }
        public RpcError Error { get; set; }
    }
}
