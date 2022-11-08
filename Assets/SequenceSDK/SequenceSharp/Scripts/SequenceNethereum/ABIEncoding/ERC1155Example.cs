using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
namespace SequenceSharp
{
    [System.Serializable]
    public class ERC1155Example : MonoBehaviour
    {
        //Set in inspector
        public int chainId = 137;
        public string contractAddress = "";
        public List<int> tokenIds = new List<int>();
        public List<string> accounts = new List<string>();
        
        // Start is called before the first frame update


        /// <summary>
        /// Call after signing in sequence wallet
        /// </summary>
        public async void ERC1155Examples()
        {
            Debug.Log("[Sequence] ERC1155 Token Example:");
            var URI = await ERC1155.URI(tokenIds[0], contractAddress, chainId);
            Debug.Log("URI: " + URI);


            var balanceOf = await ERC1155.BalanceOf(tokenIds[0], contractAddress, chainId, accounts[0]);
            Debug.Log("balanceOf: " + balanceOf);
            var balanceOfBatch = await ERC1155.BalanceOfBatch(accounts, tokenIds, contractAddress, chainId);
            foreach(var balance in balanceOfBatch)
            {
                Debug.Log("balanceOfBatch: " + balance);
            }
            
        }
    }
}
