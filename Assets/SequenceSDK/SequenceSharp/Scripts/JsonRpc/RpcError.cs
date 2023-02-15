using Newtonsoft.Json.Linq;

namespace SequenceSharp
{
    public class RpcError
    {
        public RpcError(int code, string message, JToken data = null) { }

        public int Code { get; }
        public string Message { get; }
        public JToken Data { get; }
    }
}