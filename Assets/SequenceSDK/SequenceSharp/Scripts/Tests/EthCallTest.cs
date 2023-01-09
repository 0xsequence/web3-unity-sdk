using NBitcoin;
using Nethereum.ABI.EIP712;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.JsonRpc.Client;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;
using Nethereum.RPC;
using Nethereum.RPC.Eth.DTOs;
using SequenceSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SequenceSharp
{
    public class EthCallTest : MonoBehaviour
    {
        public Web3 web3;
        private SequenceWeb3Client _sequenceWeb3Client;
        

        [SerializeField]
        private SequenceSharp.Wallet wallet;

        private int testId = 1;
        private string TestFailMessage = " <color=#B54423>[Eth Call Test]</color>";
        // Start is called before the first frame update
        async void Start()
        {
            _sequenceWeb3Client = new SequenceWeb3Client(wallet, Chain.Polygon);
            web3 = new Web3(_sequenceWeb3Client);

            Connect();
        }

        public async void Connect()
        {
            var connectDetails = await wallet.Connect(
                new ConnectOptions { app = "Demo Unity Dapp" }
            );

            Debug.Log(
                "[DemoDapp] Connect Details:  "
                    + JsonConvert.SerializeObject(
                        connectDetails,
                        Formatting.Indented,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
                    )
            );

            bool isConnected = await wallet.IsConnected();
            Debug.Assert(isConnected, TestFailMessage + "Wallet is not connected! ");
        }

            public async Task RunTests()
        {
            
        }

        #region API_Method_methods tests

        
        public async Task API_Method_Eth_ChainId()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_chainId.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);
            Debug.Assert(response.GetType() == typeof(HexBigInteger), TestFailMessage + " Eth ChainId Return Type is not HexBigInteger");

        }

        
        public async Task API_Method_Eth_ProtocolVersion()
        {

            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_protocolVersion.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);
        }

        
        public async Task API_Method_Eth_Syncing()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_syncing.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }

        
        public async Task API_Method_Eth_Coinbase()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_coinbase.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }

        
        public async Task API_Method_Eth_Mining()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_mining.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }

        
        public async Task API_Method_Eth_Hashrate()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_hashrate.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }

        
        public async Task API_Method_Eth_GasPrice()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_gasPrice.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }

        
        public async Task API_Method_Eth_FeeHistory()
        {

            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_feeHistory.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);
        }

        
        public async Task API_Method_Eth_Accounts()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_accounts.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);
            Debug.Assert(response.GetType() == typeof(string[]), TestFailMessage + " Eth Accounts Return Type is not string[] ");

        }

        
        public async Task API_Method_Eth_BlockNumber()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_blockNumber.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }
        
        public async Task API_Method_Eth_GetBalance()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_getBalance.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);
            Debug.Assert(response.GetType() == typeof(EtherBalance), TestFailMessage + " Eth GetBalance Return Type is not EtherBalance");

        }
        
        public async Task API_Method_Eth_GetStorageAt()
        {

            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_getStorageAt.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);
        }
        
        public async Task API_Method_Eth_GetTransactionCount()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_getTransactionCount.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }
        
        public async Task API_Method_Eth_GetBlockTransactionCountByHash()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_getBlockTransactionCountByHash.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }
        
        public async Task API_Method_Eth_GetBlockTransactionCountByNumber()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_getBlockTransactionCountByNumber.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }
        
        public async Task API_Method_Eth_GetUncleCountByBlockHash()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_getUncleCountByBlockHash.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }
        
        public async Task API_Method_Eth_GetUncleCountByBlockNumber()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_getUncleCountByBlockNumber.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }
        
        public async Task API_Method_Eth_GetCode()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_getCode.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }

        public async Task API_Method_Eth_Sign()
        {

            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_sign.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);
            Debug.Assert(response.GetType() == typeof(string), TestFailMessage + " Eth Sign Return Type is not string");

        }
        
        public async Task API_Method_Eth_SendTransaction()
        {
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_sendTransaction.ToString(), new object[] { });
            var response = await web3.Client.SendRequestAsync<object>(request);
            Debug.Assert(response.GetType() == typeof(string), TestFailMessage + " Eth SendTransaction Return Type is not string");

        }
        
        public async Task API_Method_Eth_SendRawTransaction()
        {

            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_sendRawTransaction.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);
        }
        
        public async Task API_Method_Eth_Call()
        {
            var message = "test message";
            RpcRequest request = new RpcRequest(1, ApiMethods.eth_call.ToString(), new object[] { message });
            var response = await web3.Client.SendRequestAsync<object>(request);           
            Debug.Assert(response.GetType() == typeof(string), TestFailMessage + " Eth Call Return Type is not string");


        }
        
        public async Task API_Method_Eth_EstimateGas()
        {

            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_estimateGas.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);
            Debug.Assert(response.GetType() == typeof(HexBigInteger), TestFailMessage + " Eth EstimateGas Return Type is not HexBigInteger");
        }

        
        public async Task API_Method_Eth_GetBlockByHash()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_getBlockByHash.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }

        
        public async Task API_Method_Eth_GetBlockByNumber()
        {

            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_getBlockByNumber.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);
        }

        
        public async Task API_Method_Eth_GetTransactionByBlockHashAndIndex()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_getTransactionByBlockHashAndIndex.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }

        
        public async Task API_Method_Eth_GetTransactionByBlockNumberAndIndex()
        {

            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_getTransactionByBlockNumberAndIndex.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);
        }

        
        public async Task API_Method_Eth_GetTransactionReceipt()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_getTransactionReceipt.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);
            Debug.Assert(response.GetType() == typeof(TransactionReceipt), TestFailMessage + " Eth GetTransactionReceipt Return Type is not TransactionReceipt");
        }
        
        public async Task API_Method_Eth_GetUncleByBlockHashAndIndex()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_getUncleByBlockNumberAndIndex.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }
        
        public async Task API_Method_Eth_GetUncleByBlockNumberAndIndex()
        {

            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_getUncleByBlockNumberAndIndex.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);
        }
        
        public async Task API_Method_Eth_GetCompilers()
        {

            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_getCompilers.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);
        }
        
        public async Task API_Method_Eth_CompileLLL()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_compileLLL.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }
        
        public async Task API_Method_Eth_CompileSolidity()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_compileSolidity.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }
        
        public async Task API_Method_Eth_CompileSerpent()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_compileSerpent.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }
        
        public async Task API_Method_Eth_NewFilter()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_newFilter.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }
        
        public async Task API_Method_Eth_NewBlockFilter()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_newBlockFilter.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }
        
        public async Task API_Method_Eth_NewPendingTransactionFilter()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_newPendingTransactionFilter.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }
        
        public async Task API_Method_Eth_UninstallFilter()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_uninstallFilter.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }
        
        public async Task API_Method_Eth_GetFilterChanges()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_getFilterChanges.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }
        
        public async Task API_Method_Eth_GetFilterLogs()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_getFilterLogs.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }
        
        public async Task API_Method_Eth_GetLogs()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_getLogs.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }
        
        public async Task API_Method_Eth_GetWork()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_getWork.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }
        
        public async Task API_Method_Eth_SubmitWork()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_submitWork.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }
        
        public async Task API_Method_Eth_SubmitHashrate()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_submitHashrate.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }
        
        public async Task API_Method_Eth_Subscribe()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_subscribe.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }
        
        public async Task API_Method_Eth_Unsubscribe()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.eth_unsubscribe.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

        }

        public async Task API_Method_Wallet_SwitchEthereumChain()
        {
            var parameters = new object[] { };
            RpcRequest request = new RpcRequest(testId, ApiMethods.wallet_switchEthereumChain.ToString(), parameters);
            var response = await web3.Client.SendRequestAsync<object>(request);

            Debug.Assert(response.GetType() == typeof(Nullable), TestFailMessage + " Wallet_SwitchEthereumChain Return Type is not null");
        }
        #endregion
    }
}
