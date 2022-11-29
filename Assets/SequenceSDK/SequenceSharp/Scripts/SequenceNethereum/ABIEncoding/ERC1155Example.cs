using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;
using Nethereum.Web3;

namespace SequenceSharp
{
    [System.Serializable]
    public class ERC1155Example : MonoBehaviour
    {
        //Set in inspector

        public string contractAddress = "";
        public List<int> tokenIds = new List<int>();
        public List<string> accounts = new List<string>();
        public Wallet _wallet;
        // Start is called before the first frame update


        /// <summary>
        /// Call after signing in sequence wallet
        /// </summary>
        public async void ERC1155Examples()
        {
            
            _wallet = FindObjectOfType<Wallet>();

            var web3 = new Web3();

            web3.Client.OverridingRequestInterceptor = new SequenceInterceptor(_wallet, 137);

            //Set web3 interceptor before using it:
            ERC1155.SetWeb3(web3);

            Debug.Log("[Sequence] ERC1155 Token Example:");
            var URI = await ERC1155.URI(tokenIds[0], contractAddress);
            Debug.Log("URI: " + URI);

            accounts[0] = await _wallet.GetAddress();
            var balanceOf = await ERC1155.BalanceOf(tokenIds[0], contractAddress,  accounts[0]);
            Debug.Log("balanceOf: " + balanceOf);
            var balanceOfBatch = await ERC1155.BalanceOfBatch(accounts, tokenIds, contractAddress);
            foreach(var balance in balanceOfBatch)
            {
                Debug.Log("balanceOfBatch: " + balance);
            }
            
        }
    }
}
