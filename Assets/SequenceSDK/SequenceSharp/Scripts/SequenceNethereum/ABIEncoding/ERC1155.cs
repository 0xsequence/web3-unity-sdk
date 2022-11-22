using System.Numerics;
using System.Threading.Tasks;
using System;
using UnityEngine;
using System.Collections.Generic;
using Nethereum.Web3;
namespace SequenceSharp
{
    public struct ERC1155Balance
    {
        public string type;
        public string hex;
    }
    public class ERC1155 : MonoBehaviour
    {
        private static string abi = "[ { \"inputs\": [ { \"internalType\": \"string\", \"name\": \"uri_\", \"type\": \"string\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"constructor\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"account\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"operator\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"bool\", \"name\": \"approved\", \"type\": \"bool\" } ], \"name\": \"ApprovalForAll\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"operator\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"from\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256[]\", \"name\": \"ids\", \"type\": \"uint256[]\" }, { \"indexed\": false, \"internalType\": \"uint256[]\", \"name\": \"values\", \"type\": \"uint256[]\" } ], \"name\": \"TransferBatch\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"operator\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"from\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"id\", \"type\": \"uint256\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"value\", \"type\": \"uint256\" } ], \"name\": \"TransferSingle\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": false, \"internalType\": \"string\", \"name\": \"value\", \"type\": \"string\" }, { \"indexed\": true, \"internalType\": \"uint256\", \"name\": \"id\", \"type\": \"uint256\" } ], \"name\": \"URI\", \"type\": \"event\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"account\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"id\", \"type\": \"uint256\" } ], \"name\": \"balanceOf\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address[]\", \"name\": \"accounts\", \"type\": \"address[]\" }, { \"internalType\": \"uint256[]\", \"name\": \"ids\", \"type\": \"uint256[]\" } ], \"name\": \"balanceOfBatch\", \"outputs\": [ { \"internalType\": \"uint256[]\", \"name\": \"\", \"type\": \"uint256[]\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"account\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"operator\", \"type\": \"address\" } ], \"name\": \"isApprovedForAll\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"_address\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"_amount\", \"type\": \"uint256\" } ], \"name\": \"ownerMint\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"from\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"internalType\": \"uint256[]\", \"name\": \"ids\", \"type\": \"uint256[]\" }, { \"internalType\": \"uint256[]\", \"name\": \"amounts\", \"type\": \"uint256[]\" }, { \"internalType\": \"bytes\", \"name\": \"data\", \"type\": \"bytes\" } ], \"name\": \"safeBatchTransferFrom\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"from\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"id\", \"type\": \"uint256\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" }, { \"internalType\": \"bytes\", \"name\": \"data\", \"type\": \"bytes\" } ], \"name\": \"safeTransferFrom\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"operator\", \"type\": \"address\" }, { \"internalType\": \"bool\", \"name\": \"approved\", \"type\": \"bool\" } ], \"name\": \"setApprovalForAll\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"bytes4\", \"name\": \"interfaceId\", \"type\": \"bytes4\" } ], \"name\": \"supportsInterface\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"name\": \"uri\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"stateMutability\": \"view\", \"type\": \"function\" } ]";

        public static Wallet _wallet;
        private static Web3 web3 = null;
        private void Awake()
        {
            _wallet = FindObjectOfType<Wallet>();
            web3 = new Web3();
            web3.Client.OverridingRequestInterceptor = new SequenceInterceptor(_wallet);
        }

        /// <summary>
        /// Returns the URI for token type _id
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public static async Task<string> URI(BigInteger _id, string address)
        {
            try
            {
                var contract = web3.Eth.GetContract(abi, address);
                var URIFunction = contract.GetFunction("uri");
                var URI = await URIFunction.CallAsync<string>(_id);

                return URI;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return "";
            }

        }

        /// <summary>
        /// Returns the amount of tokens of token type id owned by account
        /// </summary>
        /// <param name="account"> cannot be zero address </param>
        /// <param name="id"></param>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public static async Task<BigInteger> BalanceOf( BigInteger id, string address,  string account = null)
        {
            try
            {
                if (account == null)
                {
                    //account address not provided

                    account = await _wallet.GetAddress();
                }

                var contract = web3.Eth.GetContract(abi, address);
                var balanceOfFunction = contract.GetFunction("balanceOf");
                var balanceOf = await balanceOfFunction.CallAsync<BigInteger>(account, id);

                return balanceOf;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return -1;
            }


        }

        /// <summary>
        /// Batched version of balanceOf, accounts and ids must have same length
        /// </summary>
        /// <param name="accounts"> must have same length as ids</param>
        /// <param name="ids">must have same length as accounts</param>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public static async Task<List<BigInteger>> BalanceOfBatch(List<string> accounts, List<int> ids, string address)
        {

            try
            {
                var contract = web3.Eth.GetContract(abi, address);
                var balanceOfBatchFunction = contract.GetFunction("balanceOfBatch");
                var balanceOfBatch = await balanceOfBatchFunction.CallAsync<List<BigInteger>>(accounts, ids);

                return balanceOfBatch;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return null;
            }

        }

        public static async Task SetApprovalForAll(string operatorAddress, bool approved)
        {
            throw new NotImplementedException();
        }
        public static async Task<bool> IsApprovedForAll(string account, string operatorAddress)
        {
            throw new NotImplementedException();
        }
        public static async Task SafeTransferFrom(string from, string to, BigInteger id, BigInteger amount, string data)
        {
            throw new NotImplementedException();
        }
        public static async Task SafeBatchTransferFrom(string from, string to, BigInteger[] ids, BigInteger[] amounts, string data)
        {
            throw new NotImplementedException();
        }
    }
}