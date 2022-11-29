using NBitcoin;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SequenceSharp
{
    public class ERC20Example : MonoBehaviour
    {
        //Set in inspector
        public string contractAddress = "";
        public string accountAddress = "";
        public string ownerAddress = "";
        public string spenderAddress = "";

        public string recipientAddress = "";
        public string amount = "";

        public Wallet _wallet;



        /// <summary>
        /// Call after signing in sequence wallet
        /// </summary>
        public async void ERC20Examples()
        {
            _wallet = FindObjectOfType<Wallet>();
            var web3 = new Web3();
            web3.Client.OverridingRequestInterceptor = new SequenceInterceptor(_wallet, 137);
            var erc20 = new ERC20(web3, contractAddress);

            Debug.Log("[Sequence] ERC20 Token Example:");
            var name = await erc20.Name();
            Debug.Log("name: " + name);
            var symbol = await erc20.Symbol();
            Debug.Log("symbol: " + symbol);
            var decimals = await erc20.Decimals();
            Debug.Log("decimals: " + decimals);
            var totalSupply = await erc20.TotalSupply();
            Debug.Log("totalSupply: " + totalSupply);

            //accountAddress = await _wallet.GetAddress();
            var balanceOf = await erc20.BalanceOf(accountAddress);
            Debug.Log("balanceOf: " + balanceOf);

            var randomWallet = new Nethereum.HdWallet.Wallet(exampleWords, examplePassword);

            var recipientAddress = randomWallet.GetAccount(0).Address;
            spenderAddress = randomWallet.GetAccount(1).Address;

            var allowance = await erc20.Allowance(ownerAddress, spenderAddress);
            Debug.Log("allowance: " + allowance);

            var approve = await erc20.Approve(accountAddress, amount);
            Debug.Log("approve: " + approve);



/*            var transferFrom = await erc20.TransferFrom(accountAddress, recipientAddress, amount);
            Debug.Log("transfer:" + transferFrom);*/
            







        }

        private static Mnemonic exampleMnemo = new Mnemonic(Wordlist.English, WordCount.Twelve);
        private static string exampleWords = exampleMnemo.ToString(); // "ripple scissors kick mammal hire column oak again sun offer wealth tomorrow wagon turn fatal"
        private static string examplePassword = "password";
    };
}