using System;
using UnityEngine;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace SequenceSharp
{
    #region Enums
    public enum BlockChainType
    {
        Polygon = 0,
        Arbitrum = 1,
        Optimism = 2,
        Avalanche = 3
    }

    public enum ContractType
    {
        UNKNOWN,
        ERC20,
        ERC721,
        ERC1155,
        SEQUENCE_WALLET,
        ERC20_BRIDGE,
        ERC721_BRIDGE,
        ERC1155_BRIDGE
    }

    public enum EventLogType
    {
        UNKNOWN,
        BLOCK_ADDED,
        BLOCK_REMOVED
    }

    public enum EventLogDataType
    {
        UNKNOWN,
        TOKEN_TRANSFER,
        SEQUENCE_TXN
    }

    public enum TxnTransferType
    {
        UNKNOWN,
        SEND,
        RECEIVE
    }

    public enum SortOrder
    {
        DESC,
        ASC
    }
    #endregion

    public static class Indexer
    {
        private const string PATH = "/rpc/Indexer/";

        /// <summary>
        /// Combines <see cref="PATH"/> and <paramref name="name"/> to suffix on to the Base Address
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string Url(BlockChainType blockChainName, string endPoint)
        {
            return $"{HostName(blockChainName)}{PATH}{endPoint}";
        }

        /// <summary>
        /// Get HostName directing to specific <paramref name="blockChainType"/>
        /// </summary>
        /// <param name="blockChainType"></param>
        /// <returns></returns>
        private static string HostName(BlockChainType blockChainType)
        {
            return $"https://{blockChainType.ToString().ToLower()}-indexer.sequence.app";
        }

        public static async Task<T[]> FetchMultiplePages<T>(Func<int, Task<(Page, T[])>> func, int maxPages)
        {
            var nextPage = 0;
            List<T> allItems = new List<T>();
            while (nextPage != -1 && nextPage < maxPages)
            {
                (var page, var items) = await func(nextPage);
                allItems.AddRange(items);

                if (page.more && page.page != 0)
                {
                    nextPage = page.page;
                }
                else { nextPage = -1; }
                // todo add rate limit
            }
            return allItems.ToArray();
        }

        /// <summary>
        /// Retrive indexer status
        /// </summary>
        /// <returns>true if this chain's indexer is good, false otherwise</returns>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<bool> Ping(BlockChainType blockChainType)
        {
            string responseBody = await HTTPPost(blockChainType, "Ping", null);
            return BuildResponse<PingReturn>(responseBody).status;
        }

        /// <summary>
        /// Retrieve indexer version information.
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<Version> Version(BlockChainType blockChainType)
        {


            var responseBody = await HTTPPost(blockChainType, "Version", null);
            return BuildResponse<VersionReturn>(responseBody).version;
        }

        /// <summary>
        /// Retrieve indexer runtime status information
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<RuntimeStatus> RuntimeStatus(BlockChainType blockChainType)
        {
            var responseBody = await HTTPPost(blockChainType, "RuntimeStatus", null);
            return BuildResponse<RuntimeStatusReturn>(responseBody).status;
        }

        /// <summary>
        /// Retrieve the chain ID for a given BlockChainType
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>

        public static async Task<int> GetChainID(BlockChainType blockChainType)
        {
            var responseBody = await HTTPPost(blockChainType, "GetChainID", null);
            return BuildResponse<GetChainIDReturn>(responseBody).chainID;
        }

        /// <summary>
        /// Retrieve the balance of a network's native token for a given account address
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<EtherBalance> GetEtherBalance(BlockChainType blockChainType, string accountAddress)
        {
            var responseBody = await HTTPPost(blockChainType, "GetEtherBalance", new GetEtherBalanceArgs(accountAddress));
            return BuildResponse<GetEtherBalanceReturn>(responseBody).balance;
        }

        /// <summary>
        /// Retrieve an account's token balances, optionally for a specific contract
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<GetTokenBalancesReturn> GetTokenBalances(BlockChainType blockChainType, GetTokenBalancesArgs args)
        {
            var responseBody = await HTTPPost(blockChainType, "GetTokenBalances", args);
            //Debug.Log(responseBody);
            return BuildResponse<GetTokenBalancesReturn>(responseBody);
        }

        /// <summary>
        /// Retrieve the token supply for a given contract
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<GetTokenSuppliesReturn> GetTokenSupplies(BlockChainType blockChainType, GetTokenSuppliesArgs args)
        {
            var responseBody = await HTTPPost(blockChainType, "GetTokenSupplies", args);
            return BuildResponse<GetTokenSuppliesReturn>(responseBody);
        }

        /// <summary>
        /// Retrieve <see cref="GetTokenSuppliesMapReturn"/>
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<GetTokenSuppliesMapReturn> GetTokenSuppliesMap(BlockChainType blockChainType, GetTokenSuppliesMapArgs args)
        {
            var responseBody = await HTTPPost(blockChainType, "GetTokenSuppliesMap", args);
            return BuildResponse<GetTokenSuppliesMapReturn>(responseBody);

        }

        /// <summary>
        /// Retrieve <see cref="GetBalanceUpdatesReturn"/>
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>
        public static async Task<GetBalanceUpdatesReturn> GetBalanceUpdates(BlockChainType blockChainType, GetBalanceUpdatesArgs args)
        {
            var responseBody = await HTTPPost(blockChainType, "GetBalanceUpdates", args);
            return BuildResponse<GetBalanceUpdatesReturn>(responseBody);
        }

        /// <summary>
        /// Retrieve transaction history <see cref="GetTransactionHistoryReturn"/>
        /// </summary>
        /// <exception cref="HttpRequestException">If the network request fails</exception>

        public static async Task<GetTransactionHistoryReturn> GetTransactionHistory(BlockChainType blockChainType, GetTransactionHistoryArgs args)
        {
            var responseBody = await HTTPPost(blockChainType, "GetTransactionHistory", args);
            return BuildResponse<GetTransactionHistoryReturn>(responseBody);
        }

        /// <summary>
        /// Makes an HTTP Post Request with content-5ype set to application/json
        /// </summary>
        /// <returns></returns>
        private static async Task<string> HTTPPost(BlockChainType blockChainType, string endPoint, object args)
        {
            var req = UnityWebRequest.Put(Url(blockChainType, endPoint), JsonConvert.SerializeObject(args));
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Accept", "application/json");
            req.method = UnityWebRequest.kHttpVerbPOST;
            await req.SendWebRequest();
            if (req.responseCode < 200 || req.responseCode > 299)
            {
                throw new System.Exception("Failed to make request, non-200 status code " + req.responseCode);
            }
            return req.downloadHandler.text;
        }

        /// <summary>
        /// Parses <paramref name="text"/> into JSON, if it fails due to <paramref name="text"/> not being of JSON format, will throw an exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text"></param>
        /// <returns></returns>
        private static T BuildResponse<T>(string text)
        {
            T data = JsonConvert.DeserializeObject<T>(text);
            return data;
        }
    }
}