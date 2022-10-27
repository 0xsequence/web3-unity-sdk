using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;
using Nethereum.RPC;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SequenceSharp
{
    public class SequenceInterceptor : RequestInterceptor
    {
        private readonly Wallet _wallet;

        public SequenceInterceptor(Wallet wallet)
        {
            _wallet = wallet;
        }

        public override async Task<object> InterceptSendRequestAsync<T>(
            Func<RpcRequest, string, Task<T>> interceptedSendRequestAsync, RpcRequest request,
            string route = null)
        {
            
            Debug.Log("METHOD!!"+ request.Method);
            if (request.Method == ApiMethods.eth_sendTransaction.ToString())
            {
                TransactionInput transactionInput =(TransactionInput)request.RawParameters[0];
                var dataType = new {abi = "", contractAddress = "", functionData="" };


                var data = JsonConvert.DeserializeAnonymousType(transactionInput.Data.Substring(2), dataType);
                //Debug.Log("data" + data);

                var txnResponse = await _wallet.ExecuteSequenceJS(@"

                const signer = seq.getWallet().getSigner();
  
                const tx = {
                    delegateCall: false,
                    revertOnError: false,
                    gasLimit: '0x55555',
                    to: '"+ data.contractAddress+@"',
                    value: "+ transactionInput.Value.ToString()+@",
                    data: new ethers.utils.Interface("+data.abi+@").encodeFunctionData("+data.functionData+@")
                }

                const txnResponse = await signer.sendTransactionBatch([tx]);

                return txnResponse;
;");
                
                return txnResponse;

            }
            else if (request.Method == ApiMethods.eth_gasPrice.ToString() || request.Method == ApiMethods.eth_call.ToString())
            {
                
                Debug.Log(request.RawParameters);
                var dataType = new { to= "", data = "" };
                var data = JsonConvert.DeserializeAnonymousType("", dataType);
                Debug.Log("data" + data);
                var estimatedGas = await _wallet.ExecuteSequenceJS(@"
                    const wallet = sequence.getWallet();
                    const provider = wallet.getProvider()!

                    const tx ={
                        to: '"+data.to+@"',
                        data:'"+data.data+@"'
                    }
                    const estimate = await provider.estimateGas(tx);
");

                return estimatedGas;
                

            }
            else if (request.Method == ApiMethods.eth_signTypedData_v4.ToString())
            {
                throw new NotImplementedException();
            }
            else if (request.Method == ApiMethods.eth_sign.ToString())
            {
                
                return await _wallet.ExecuteSequenceJS(@"
                const wallet = sequence.getWallet();

                const signer = wallet.getSigner();

                const message = `" + request.RawParameters[1] + @"`

            // sign
            const sig = await signer.signMessage(message);
            return sig;");
                 
            }else if(request.Method == ApiMethods.eth_getBalance.ToString())
            {
                Debug.Log(request.Method);
                var accountAddress = await _wallet.GetAddress();
                var ethBalance = await Indexer.GetEtherBalance(BlockChainType.Polygon, accountAddress);
                return ethBalance;

            }else if(request.Method == ApiMethods.eth_chainId.ToString())
            {
                var chainID = await Indexer.GetChainID(BlockChainType.Polygon);
                return chainID;
            }
            else
            {
                return null;

            }

        }

        public override async Task<object> InterceptSendRequestAsync<T>(
            Func<string, string, object[], Task<T>> interceptedSendRequestAsync, string method,
            string route = null, params object[] paramList)
        {
            if (method == ApiMethods.eth_sendTransaction.ToString())
            {
                return null;

            }
            else if (method == ApiMethods.eth_estimateGas.ToString() || method == ApiMethods.eth_call.ToString())
            {
                return null;

            }
            else if (method == ApiMethods.eth_signTypedData_v4.ToString() || method == ApiMethods.personal_sign.ToString())
            {
                return null;

            }
            else
            {
                return null;

            }

        }

        private string GetSelectedAccount()
        {
            return "";
        }

        protected void HandleRpcError(RpcResponseMessage response)
        {
            if (response.HasError)
                throw new RpcResponseException(new Nethereum.JsonRpc.Client.RpcError(response.Error.Code, response.Error.Message,
                    response.Error.Data));
        }

        private T ConvertResponse<T>(RpcResponseMessage response,
            string route = null)
        {
            HandleRpcError(response);
            try
            {
                return response.GetResult<T>();
            }
            catch (FormatException formatException)
            {
                throw new RpcResponseFormatException("Invalid format found in RPC response", formatException);
            }
        }

    }
}