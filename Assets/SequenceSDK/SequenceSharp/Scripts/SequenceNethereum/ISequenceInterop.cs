using System.Threading.Tasks;
using Nethereum.JsonRpc.Client.RpcMessages;

namespace SequenceSharp
{
    public interface ISequenceInterop
    {
        ValueTask<string> EnableEthereumAsync();
        ValueTask<bool> CheckMetamaskAvailability();
        ValueTask<string> GetSelectedAddress();
        ValueTask<RpcResponseMessage> SendAsync(RpcRequestMessage rpcRequestMessage);
        ValueTask<RpcResponseMessage> SendTransactionAsync(SequenceRpcRequestMessage rpcRequestMessage);
        ValueTask<string> SignAsync(string utf8Hex);
    }
}