
using System.Threading.Tasks;

namespace SequenceSharp
{
    public interface IClient
    {
        Task<T> SendRequestAsync<T>(RpcRequest request, string route = null);
        Task<T> SendRequestAsync<T>(string method, string route = null, params object[] paramList);
    }
}