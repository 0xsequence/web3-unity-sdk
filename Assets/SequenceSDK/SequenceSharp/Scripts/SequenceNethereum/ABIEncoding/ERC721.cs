using System.Numerics;
using System.Threading.Tasks;
using System;
using UnityEngine;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;

namespace SequenceSharp
{
    public struct ERC721Balance
    {
        public string type;
        public string hex;
    }

    public class ERC721
    {
        private static string abi = "[ { \"inputs\": [ { \"internalType\": \"string\", \"name\": \"name_\", \"type\": \"string\" }, { \"internalType\": \"string\", \"name\": \"symbol_\", \"type\": \"string\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"constructor\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"approved\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"uint256\", \"name\": \"tokenId\", \"type\": \"uint256\" } ], \"name\": \"Approval\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"operator\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"bool\", \"name\": \"approved\", \"type\": \"bool\" } ], \"name\": \"ApprovalForAll\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"from\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"uint256\", \"name\": \"tokenId\", \"type\": \"uint256\" } ], \"name\": \"Transfer\", \"type\": \"event\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"tokenId\", \"type\": \"uint256\" } ], \"name\": \"approve\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" } ], \"name\": \"balanceOf\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"tokenId\", \"type\": \"uint256\" } ], \"name\": \"getApproved\", \"outputs\": [ { \"internalType\": \"address\", \"name\": \"\", \"type\": \"address\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"operator\", \"type\": \"address\" } ], \"name\": \"isApprovedForAll\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"name\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"tokenId\", \"type\": \"uint256\" } ], \"name\": \"ownerOf\", \"outputs\": [ { \"internalType\": \"address\", \"name\": \"\", \"type\": \"address\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"from\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"tokenId\", \"type\": \"uint256\" } ], \"name\": \"safeTransferFrom\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"from\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"tokenId\", \"type\": \"uint256\" }, { \"internalType\": \"bytes\", \"name\": \"_data\", \"type\": \"bytes\" } ], \"name\": \"safeTransferFrom\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"operator\", \"type\": \"address\" }, { \"internalType\": \"bool\", \"name\": \"approved\", \"type\": \"bool\" } ], \"name\": \"setApprovalForAll\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"bytes4\", \"name\": \"interfaceId\", \"type\": \"bytes4\" } ], \"name\": \"supportsInterface\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"symbol\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"uint256\", \"name\": \"tokenId\", \"type\": \"uint256\" } ], \"name\": \"tokenURI\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"from\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"tokenId\", \"type\": \"uint256\" } ], \"name\": \"transferFrom\", \"outputs\": [], \"stateMutability\": \"nonpayable\", \"type\": \"function\" } ]";

        private static Web3 _web3 = null;
        private string _contractAddress = "";
        private string _accountAddress = ""; //TODO : Needs account address from the wallet
        private Nethereum.Contracts.Contract _contract;
        private HexBigInteger zero = new HexBigInteger(0);
        public ERC721(Web3 web3, string contractAddress)
        {
            _web3 = web3;
            _contractAddress = contractAddress;
            _contract = _web3.Eth.GetContract(abi, _contractAddress);
        }

        /// <summary>
        /// Returns the token collection name.
        /// </summary>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public Task<string> Name()
        {
            return _contract.GetFunction("name").CallAsync<string>();
            

        }

        /// <summary>
        /// Returns the token collection symbol.
        /// </summary>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public Task<string> Symbol()
        {
            return _contract.GetFunction("symbol").CallAsync<string>();
        }

        /// <summary>
        /// Returns the Uniform Resource Identifier (URI) for tokenId token.
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public Task<string> TokenURI(BigInteger tokenId)
        {
            return _contract.GetFunction("tokenURI").CallAsync<string>(tokenId);
            
        }

        /// <summary>
        /// Returns the number of tokens in owner's account.
        /// </summary>
        /// <param name="owner">Account address, if not provided, it will be the account address from sequence wallet</param>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public Task<BigInteger> BalanceOf(string owner)
        {
            return _contract.GetFunction("balanceOf").CallAsync<BigInteger>(owner);

        }

        /// <summary>
        /// Returns the owner of the tokenId token.
        /// </summary>
        /// <param name="tokenId">tokenId must exist.</param>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public Task<string> OwnerOf(BigInteger tokenId)
        {
            return _contract.GetFunction("ownerOf").CallAsync<string>(tokenId);
            
        }

        public async Task SafeTransferFrom(string from, string to, BigInteger tokenId)
        {
            var receipt = await _contract.GetFunction("safeTransferFrom").SendTransactionAndWaitForReceiptAsync(_accountAddress, zero, zero, null, from,to,tokenId);
            Debug.Log("[Sequence] receipt form function SafeTransferFrom: " + receipt);

        }

        public async Task TransferFrom(string from, string to, BigInteger tokenId)
        {
            var receipt = await _contract.GetFunction("transferFrom").SendTransactionAndWaitForReceiptAsync(_accountAddress, zero, zero, null, from, to, tokenId);
            Debug.Log("[Sequence] receipt form function TransferFrom: " + receipt);           
        }
        public async Task Approve(string to, BigInteger tokenId)
        {
            
            
            var receipt = await _contract.GetFunction("approve").SendTransactionAndWaitForReceiptAsync(_accountAddress, zero, zero, null, to, tokenId);
            Debug.Log("[Sequence] receipt form function TransferFrom: " + receipt);
        }
        public async Task<string> GetApproved(BigInteger tokenId)
        {
            var receipt = await _contract.GetFunction("getApproved").SendTransactionAndWaitForReceiptAsync(_accountAddress, zero, zero, null, tokenId);
            Debug.Log("[Sequence] receipt form function Approve: " + receipt);
            return receipt.ToString();
        }
        public async Task<bool> SetApprovalForAll(string operatorAddress, bool _approved)
        {
            var receipt = await _contract.GetFunction("setApprovalForAll").SendTransactionAndWaitForReceiptAsync(_accountAddress, zero, zero, null, operatorAddress,_approved);
            Debug.Log("[Sequence] receipt form function SetApprovalForAll: " + receipt);
            return true;
        }

        public async Task<bool> IsApprovedForAll(string owner, string operatorAddress)
        {
            var receipt = await _contract.GetFunction("isApprovedForAll").SendTransactionAndWaitForReceiptAsync(_accountAddress, zero, zero, null, owner, operatorAddress);
            Debug.Log("[Sequence] receipt form function IsApprovedForAll: " + receipt);
            return false;
        }

        
    }
}