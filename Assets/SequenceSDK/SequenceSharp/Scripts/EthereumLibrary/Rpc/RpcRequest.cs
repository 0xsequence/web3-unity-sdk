
namespace SequenceSharp
{

    public class RpcRequest
    {
        public RpcRequest(object id, string method, params object[] parameterList)
        {

        }

        public object Id { get; set; }
        public string Method { get; }
        public object[] RawParameters { get; }
    }
}