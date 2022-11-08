using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
namespace SequenceSharp
{
    public class ERC721Example : MonoBehaviour
    {
        //Set in inspector
        public int chainId = 137;
        public BigInteger tokenId = 1;
        public string contractAddress = "";
        public string accountAddress = "";
        // Start is called before the first frame update


        /// <summary>
        /// Call after signing in sequence wallet
        /// </summary>
        public async void ERC721Examples()
        {
            Debug.Log("[Sequence] ERC721 Token Example:");
            var name = await ERC721.Name(contractAddress, chainId);
            Debug.Log("name: " + name);
            var symbol = await ERC721.Symbol(contractAddress, chainId);
            Debug.Log("symbol: " + symbol);
            var tokenURI = await ERC721.TokenURI(tokenId, contractAddress, chainId);
            Debug.Log("tokenURI: " + tokenURI);

            var balanceOf = await ERC721.BalanceOf(contractAddress, chainId, accountAddress);
            Debug.Log("balanceOf: " + balanceOf);
            var ownerOf = await ERC721.OwnerOf(tokenId, contractAddress, chainId);
            Debug.Log("ownerOf: " + ownerOf);
        }
    }
}