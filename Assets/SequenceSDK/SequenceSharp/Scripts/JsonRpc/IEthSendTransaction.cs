
using System.Threading.Tasks;

namespace SequenceSharp
{
    public interface IEthSendTransaction
    {
        RpcRequest BuildRequest(TransactionInput input, object id = null);
        Task<string> SendRequestAsync(TransactionInput input, object id = null);
    }
}
