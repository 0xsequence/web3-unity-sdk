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

        public Wallet _wallet;



        /// <summary>
        /// Call after signing in sequence wallet
        /// </summary>
        public async void ERC20Examples()
        {

            _wallet = FindObjectOfType<Wallet>();

            var web3 = new Web3();

            web3.Client.OverridingRequestInterceptor = new SequenceInterceptor(_wallet, 137);

            //Set web3 interceptor before using it:
            ERC20.SetWeb3(web3);

            Debug.Log("[Sequence] ERC20 Token Example:");
            var name = await ERC20.Name(contractAddress);
            Debug.Log("name: " + name);
            var symbol = await ERC20.Symbol(contractAddress);
            Debug.Log("symbol: " + symbol);
            var decimals = await ERC20.Decimals(contractAddress);
            Debug.Log("decimals: " + decimals);
            var totalSupply = await ERC20.TotalSupply(contractAddress);
            Debug.Log("totalSupply: " + totalSupply);

            accountAddress = await _wallet.GetAddress();
            var balanceOf = await ERC20.BalanceOf(contractAddress, accountAddress);
            Debug.Log("balanceOf: " + balanceOf);
        }


    }
}