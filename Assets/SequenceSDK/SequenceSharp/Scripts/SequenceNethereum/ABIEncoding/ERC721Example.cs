using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using Nethereum.Web3;

namespace SequenceSharp
{
    public class ERC721Example : MonoBehaviour
    {
        //Set in inspector

        public BigInteger tokenId = 1;
        public string contractAddress = "";
        public string accountAddress = "";

        public Wallet _wallet;

        /// <summary>
        /// Call after signing in sequence wallet
        /// </summary>
        public async void ERC721Examples()
        {

            _wallet = FindObjectOfType<Wallet>();

            var web3 = new Web3();

            web3.Client.OverridingRequestInterceptor = new SequenceInterceptor(_wallet, 137);

            ERC721.SetWeb3(web3);

            Debug.Log("[Sequence] ERC721 Token Example:");
            var name = await ERC721.Name(contractAddress);
            Debug.Log("name: " + name);
            var symbol = await ERC721.Symbol(contractAddress );
            Debug.Log("symbol: " + symbol);
            var tokenURI = await ERC721.TokenURI(tokenId, contractAddress);
            Debug.Log("tokenURI: " + tokenURI);

            accountAddress = await _wallet.GetAddress();

            var balanceOf = await ERC721.BalanceOf(contractAddress,  accountAddress);
            Debug.Log("balanceOf: " + balanceOf);
            var ownerOf = await ERC721.OwnerOf(tokenId, contractAddress);
            Debug.Log("ownerOf: " + ownerOf);
        }
    }
}