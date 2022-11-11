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
using Nethereum.Hex.HexTypes;

namespace SequenceSharp
{
    public struct EstimatedGas
    {
        public string type;
        public string hex;
    }
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
            
            if (request.Method == ApiMethods.eth_sendTransaction.ToString())
            {
                TransactionInput transactionInput =(TransactionInput)request.RawParameters[0];               
                string rpcResponse = await _wallet.ExecuteSequenceJS(@"
                const signer = seq.getWallet().getSigner();
                
                 const tx = {
                    delegateCall: false,
                    revertOnError: false,
                    gasLimit: '0x"+ transactionInput.Gas +@"',
                    to: '" + transactionInput.To + @"',
                    value: " + transactionInput.Value + @",
                    data: '"+ transactionInput.Data + @"'
                }
                const txnResponse = await signer.sendTransactionBatch([tx]);
                console.log(txnResponse);
                
                
                let rpcResponse = {
                        jsonrpc: '2.0',
                        result: txnResponse,
                        id: 0, //parsedMessage.id,(???)
                        error: null
                    };
                return rpcResponse;
                
                ");

                RpcResponseMessage rpcResponseMessage = JsonConvert.DeserializeObject<RpcResponseMessage>(rpcResponse);
                var response = ConvertResponse<string>(rpcResponseMessage);

                return response;

            }
            else if (request.Method == ApiMethods.eth_estimateGas.ToString())
            {

                CallInput callInput = (CallInput)request.RawParameters[0];

                var estimatedGas = await _wallet.ExecuteSequenceJS(@"
                    const wallet = seq.getWallet();
                    const provider = wallet.getProvider();
                    console.log(provider);
                    const estimate = await provider.estimateGas({
                        to: '" + callInput.To + @"',
                        data:'" + callInput.Data + @"'
                    });

                    return estimate;
");
                EstimatedGas gas = JsonConvert.DeserializeObject<EstimatedGas>(estimatedGas);
                

                return new HexBigInteger(gas.hex);
                
                

            }
            else if(request.Method == ApiMethods.eth_call.ToString())
            {
                
                var callInput = (CallInput)request.RawParameters[0];


                if (callInput.From == null)
                {
                    var address = _wallet.GetAddress();
                }
                string rpcResponse =  await _wallet.ExecuteSequenceJS(@"
                var wallet = seq.getWallet();
                var provider = wallet.getProvider();
                var hexString = await provider.call({to:'" + callInput.To + @"',data:'" + callInput.Data + @"'});
                
                
                let rpcResponse = {
                        jsonrpc: '2.0',
                        result: hexString,
                        id: 0, //parsedMessage.id,(???)
                        error: null
                    };
                return rpcResponse;
                
                ");

                RpcResponseMessage rpcResponseMessage= JsonConvert.DeserializeObject<RpcResponseMessage>(rpcResponse);
                var response = ConvertResponse<string>(rpcResponseMessage);
                
                return response;
                
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