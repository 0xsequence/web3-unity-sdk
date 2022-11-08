using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SequenceSharp
{
    public class ERC20Example : MonoBehaviour
    {
        //Set in inspector
        public int chainId = 137;
        public string contractAddress = "";
        public string accountAddress = "";
        // Start is called before the first frame update


        /// <summary>
        /// Call after signing in sequence wallet
        /// </summary>
        public async void ERC20Examples()
        {
            Debug.Log("[Sequence] ERC20 Token Example:");
            var name = await ERC20.Name(contractAddress, chainId);
            Debug.Log("name: " + name);
            var symbol = await ERC20.Symbol(contractAddress, chainId);
            Debug.Log("symbol: " + symbol);
            var decimals = await ERC20.Decimals(contractAddress, chainId);
            Debug.Log("decimals: " + decimals);
            var totalSupply = await ERC20.TotalSupply(contractAddress, chainId);
            Debug.Log("totalSupply: " + totalSupply);
            var balanceOf = await ERC20.BalanceOf(contractAddress, chainId, accountAddress);
            Debug.Log("balanceOf: " + balanceOf);
        }


    }
}