using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using Nethereum.Web3;
using NBitcoin;
using System.Text;

namespace SequenceSharp
{
   

    [System.Serializable]
    public class ERC1155Example : MonoBehaviour
    {
        //Set in inspector

        public string contractAddress = "";
        
        public List<BigInteger> tokenIds = new List<BigInteger>(3);
        public List<string> accounts = new List<string>();
        public List<BigInteger> amounts = new List<BigInteger>(3);
        public string data = "";
        public Wallet _wallet;
        



        /// <summary>
        /// Call after signing in sequence wallet
        /// </summary>
        public async void ERC1155Examples()
        {
            
            _wallet = FindObjectOfType<Wallet>();
            var web3 = new Web3();
            web3.Client.OverridingRequestInterceptor = new SequenceInterceptor(_wallet, 137);           
            var erc1155 = new ERC1155(web3,contractAddress);

            //Generate a random address for example testing
            var randomWallet = new Nethereum.HdWallet.Wallet(exampleWords, examplePassword);
            var randomAddress = randomWallet.GetAccount(0).Address;
            //Example tokenIds and amounts
            tokenIds.Add(67617);
            tokenIds.Add(68628);
            tokenIds.Add(69555);

            amounts.Add(1);
            amounts.Add(1);
            amounts.Add(1);

            Debug.Log("[Sequence] ERC1155 Token Example:");
            var URI = await erc1155.URI(tokenIds[0]);
            Debug.Log("URI: " + URI);
            accounts[0] = await _wallet.GetAddress();
            var balanceOf = await erc1155.BalanceOf(tokenIds[0],  accounts[0]);
            Debug.Log("balanceOf: " + balanceOf);
            var balanceOfBatch = await erc1155.BalanceOfBatch(accounts, tokenIds);
            foreach(var balance in balanceOfBatch)
            {
                Debug.Log("balanceOfBatch: " + balance);
            }

            //Tests for transaction functions:
            //await erc1155.SetApprovalForAll(accounts[0], true);
            //erc1155.IsApprovedForAll(accounts[0], randomAddress);
            //await erc1155.SafeTransferFrom(accounts[0], randomAddress, tokenIds[0], 1, Encoding.ASCII.GetBytes(data));
            await erc1155.SafeBatchTransferFrom(accounts[0], randomAddress, tokenIds, amounts, Encoding.ASCII.GetBytes(data));

        }

        private static Mnemonic exampleMnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
        private static string exampleWords = exampleMnemo.ToString(); // "ripple scissors kick mammal hire column oak again sun offer wealth tomorrow wagon turn fatal"
        private static string examplePassword = "password";
    }
}
