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
            var rpcReq = await web3.Eth.Accounts.SendRequestAsync();
            // One day, we'll need to replace this to use the current account, once Sequence has multi account support.
            var address = (rpcReq)[0];
            return address;
        }
    }

    public struct EstimatedGas
    {
        public string type;
        public string hex;
    }

    public class SequenceWeb3Client : IClient
    {
        public BigInteger chainID;
        private readonly Wallet _wallet;

        // TODO THIS IS WRONG! we need to get tx receipts from specific TXs.
        private TransactionReceipt transactionReceipt = new TransactionReceipt();

        public RequestInterceptor OverridingRequestInterceptor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SequenceWeb3Client(Wallet wallet, BigInteger chainID)
        {
            _wallet = wallet;
            this.chainID = chainID;
        }

        public async Task<T> SendRequestAsync<T>(
                  RpcRequest request, string route = null
              )
        {
            return (T)await _SendRequestAsync(request, route);
        }


        public Task<RpcRequestResponseBatch> SendBatchRequestAsync(RpcRequestResponseBatch rpcRequestResponseBatch)
        {
            throw new NotImplementedException();
        }

        public async Task<T> SendRequestAsync<T>(string method, string route = null, params object[] paramList)
        {
            return (T)await _SendRequestAsync(new RpcRequest("1234", method, paramList));
        }

        public async Task SendRequestAsync(RpcRequest request, string route = null)
        {
            await _SendRequestAsync(request, route);
        }

        public async Task SendRequestAsync(string method, string route = null, params object[] paramList)
        {
            await _SendRequestAsync(new RpcRequest("1234", method, paramList));
        }
        private async Task<object> _SendRequestAsync(
            RpcRequest request, string route = null
        )
        {
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
                        id: 0, //parsedMessage.id,(???) // TODO?
                        error: null
                    };
                "
                );
                RpcResponseMessage rpcResponseMessage =
                    JsonConvert.DeserializeObject<RpcResponseMessage>(rpcResponse);

                transactionReceipt = ConvertResponse<TransactionReceipt>(rpcResponseMessage);

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
                ");

                RpcResponseMessage rpcResponseMessage =
                    JsonConvert.DeserializeObject<RpcResponseMessage>(rpcResponse);
                var response = ConvertResponse<string>(rpcResponseMessage);

                return response;
            }
            else if (request.Method == ApiMethods.eth_signTypedData_v4.ToString())
            {
                // TODO
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
                ");
            }
            else if (request.Method == ApiMethods.eth_getBalance.ToString())
            {
                var accountAddress = await _wallet.GetAddress();
                var ethBalance = await Indexer.GetEtherBalance(chainID, accountAddress);
                return ethBalance;
            }
            else if (request.Method == ApiMethods.eth_chainId.ToString())
            {
                return new HexBigInteger(chainID);
            }
            else if (request.Method == ApiMethods.wallet_switchEthereumChain.ToString())
            {
                this.chainID = BigInteger.Parse((string)request.RawParameters[0]);
                return null; // TODO should throw 4902 if it's not valid
            }
            else if (request.Method == ApiMethods.eth_accounts.ToString())
            {
                var addr = await _wallet.GetAddress();
                return new string[] { addr };
            }
            else
            {
                Debug.Log("Non-intercepted Sequence call: " + request.Method);
                throw new NotImplementedException();
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
