using UnityEngine;
using System;
using System.Threading.Tasks;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;
using Nethereum.RPC;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;
using Nethereum.Hex.HexTypes;
using System.Numerics;
using Nethereum.Web3;

namespace SequenceSharp
{
    public static class Web3Extensions
    {
        public static async Task<string> GetAddress(this Web3 web3)
        {
            var address = (await web3.Eth.Accounts.SendRequestAsync())[0];
            return address;
        }
    }

    public struct EstimatedGas
    {
        public string type;
        public string hex;
    }

    public class SequenceInterceptor : RequestInterceptor
    {
        public BigInteger chainID;
        private readonly Wallet _wallet;

        private TransactionReceipt transactionReceipt = new TransactionReceipt();

        public SequenceInterceptor(Wallet wallet, BigInteger chainID)
        {
            _wallet = wallet;
            this.chainID = chainID;
        }

        public override async Task<object> InterceptSendRequestAsync<TResponse>(
            Func<RpcRequest, string, Task<TResponse>> interceptedSendRequestAsync,
            RpcRequest request,
            string route = null
        )
        {
            Debug.Log("intercepted send request async" + interceptedSendRequestAsync.ToString());
            Debug.Log("method:" + request.Method);

            if (request.Method == ApiMethods.eth_sendTransaction.ToString())
            {
                TransactionInput transactionInput = (TransactionInput)request.RawParameters[0];

                string rpcResponse = await _wallet.ExecuteSequenceJS(
                    @"
                    const signer = seq.getWallet().getSigner("
                        + chainID.ToString()
                        + @");
                
                    const tx = {
                        delegateCall: false,
                        revertOnError: false,
                        gasLimit: '0x55555',
                        to: '"
                        + transactionInput.To
                        + @"',
                        value: "
                        + transactionInput.Value
                        + @",
                        data: '"
                        + transactionInput.Data
                        + @"'
                    };
                    console.log(signer);
                    const txnResponse = await signer.sendTransactionBatch([tx]);
                
                    return {
                        jsonrpc: '2.0',
                        result: txnResponse,
                        id: 0, //parsedMessage.id,(???)
                        error: null
                    };
                "
                );
                Debug.Log("rpc response:" + rpcResponse);
                RpcResponseMessage rpcResponseMessage =
                    JsonConvert.DeserializeObject<RpcResponseMessage>(rpcResponse);
                Debug.Log("response mesage result: " + rpcResponseMessage.Result);

                transactionReceipt = ConvertResponse<TransactionReceipt>(rpcResponseMessage);
                Debug.Log("blockhash: " + transactionReceipt.BlockHash);

                return transactionReceipt.ToString();
            }
            else if (request.Method == ApiMethods.eth_getTransactionReceipt.ToString())
            {
                return transactionReceipt;
            }
            else if (request.Method == ApiMethods.eth_estimateGas.ToString())
            {
                CallInput callInput = (CallInput)request.RawParameters[0];

                var estimatedGas = await _wallet.ExecuteSequenceJS(
                    @"
                    const wallet = seq.getWallet();
                    const provider = wallet.getProvider("
                        + chainID.ToString()
                        + @");
                    console.log(provider);
                    const estimate = await provider.estimateGas({
                        to: '"
                        + callInput.To
                        + @"',
                        data:'"
                        + callInput.Data
                        + @"'
                    });

                    return estimate;
"
                );
                EstimatedGas gas = JsonConvert.DeserializeObject<EstimatedGas>(estimatedGas);

                return new HexBigInteger(gas.hex);
            }
            else if (request.Method == ApiMethods.eth_call.ToString())
            {
                var callInput = (CallInput)request.RawParameters[0];

                if (callInput.From == null)
                {
                    var address = _wallet.GetAddress();
                }
                string rpcResponse = await _wallet.ExecuteSequenceJS(
                    @"
                    var wallet = seq.getWallet();
                    var provider = wallet.getProvider("
                        + chainID.ToString()
                        + @");
                    var hexString = await provider.call({to:'"
                        + callInput.To
                        + @"',data:'"
                        + callInput.Data
                        + @"'});
                    
                    
                    let rpcResponse = {
                            jsonrpc: '2.0',
                            result: hexString,
                            id: 0, //parsedMessage.id,(???)
                            error: null
                        };
                    return rpcResponse;
                "
                );

                RpcResponseMessage rpcResponseMessage =
                    JsonConvert.DeserializeObject<RpcResponseMessage>(rpcResponse);
                var response = ConvertResponse<string>(rpcResponseMessage);

                return response;
            }
            else if (request.Method == ApiMethods.eth_signTypedData_v4.ToString())
            {
                throw new NotImplementedException();
            }
            else if (request.Method == ApiMethods.eth_sign.ToString())
            {
                return await _wallet.ExecuteSequenceJS(
                    @"
                    const wallet = sequence.getWallet();

                    const signer = wallet.getSigner("
                        + chainID.ToString()
                        + @");

                    const message = `"
                        + request.RawParameters[1]
                        + @"`

                    return signer.signMessage(message);
                "
                );
            }
            else if (request.Method == ApiMethods.eth_getBalance.ToString())
            {
                var accountAddress = await _wallet.GetAddress();
                var ethBalance = await Indexer.GetEtherBalance(chainID, accountAddress);
                return ethBalance;
            }
            else if (request.Method == ApiMethods.eth_chainId.ToString())
            {
                return chainID;
            }
            else if (request.Method == ApiMethods.wallet_switchEthereumChain.ToString())
            {
                this.chainID = BigInteger.Parse((string)request.RawParameters[0]);
                return null; // should throw 4902 if it's not valid
            }
            else if (request.Method == ApiMethods.eth_accounts.ToString())
            {
                return new string[] { await _wallet.GetAddress() };
            }
            else
            {
                return await base.InterceptSendRequestAsync(
                    interceptedSendRequestAsync,
                    request,
                    route
                )
                    .ConfigureAwait(false);
            }
        }

        protected void HandleRpcError(RpcResponseMessage response)
        {
            if (response.HasError)
                throw new RpcResponseException(
                    new Nethereum.JsonRpc.Client.RpcError(
                        response.Error.Code,
                        response.Error.Message,
                        response.Error.Data
                    )
                );
        }

        private T ConvertResponse<T>(RpcResponseMessage response, string route = null)
        {
            HandleRpcError(response);
            try
            {
                return response.GetResult<T>();
            }
            catch (FormatException formatException)
            {
                throw new RpcResponseFormatException(
                    "Invalid format found in RPC response",
                    formatException
                );
            }
        }
    }
}
