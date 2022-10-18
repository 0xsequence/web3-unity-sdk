using System;
using UnityEngine;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

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

    static HttpClient client;

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

    /// <summary>
    /// Creates a new instance of <see cref="client"/> with a base address set based on <paramref name="hostName"/>
    /// </summary>
    /// <param name="hostName"></param>
    private static void InitClient(string hostName)
    {
        client = new()
        {
            BaseAddress = new Uri(hostName)
        };
    }

    /// <summary>
    /// Retrive indexer status
    /// </summary>
    /// <returns>true if this chain's indexer is good, false otherwise</returns>
    /// <exception cref="HttpRequestException">If the network request fails</exception>
    public static async Task<bool> Ping(BlockChainType blockChainType)
    {
        InitClient(HostName(blockChainType));

        HttpRequestMessage request = CreateHTTPRequest(blockChainType, HttpMethod.Post, "Ping", null);

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        return BuildResponse<PingReturn>(responseBody).status;
    }

    /// <summary>
    /// Retrieve indexer version information.
    /// </summary>
    /// <exception cref="HttpRequestException">If the network request fails</exception>
    public static async Task<Version> Version(BlockChainType blockChainType)
    {
        InitClient(HostName(blockChainType));

        HttpRequestMessage request = CreateHTTPRequest(blockChainType, HttpMethod.Post, "Version", null);

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        return BuildResponse<VersionReturn>(responseBody).version;
    }

    /// <summary>
    /// Retrieve indexer runtime status information
    /// </summary>
    /// <exception cref="HttpRequestException">If the network request fails</exception>
    public static async Task<RuntimeStatus> RuntimeStatus(BlockChainType blockChainType)
    {
        InitClient(HostName(blockChainType));

        HttpRequestMessage request = CreateHTTPRequest(blockChainType, HttpMethod.Post, "RuntimeStatus", null);

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        return BuildResponse<RuntimeStatusReturn>(responseBody).status;
    }

    /// <summary>
    /// Retrieve the chain ID for a given BlockChainType
    /// </summary>
    /// <exception cref="HttpRequestException">If the network request fails</exception>

    public static async Task<int> GetChainID(BlockChainType blockChainType)
    {
        InitClient(HostName(blockChainType));

        HttpRequestMessage request = CreateHTTPRequest(blockChainType, HttpMethod.Post, "GetChainID", null);

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        return BuildResponse<GetChainIDReturn>(responseBody).chainID;
    }

    /// <summary>
    /// Retrieve the balance of a network's native token for a given account address
    /// </summary>
    /// <exception cref="HttpRequestException">If the network request fails</exception>
    public static async Task<EtherBalance> GetEtherBalance(BlockChainType blockChainType, string accountAddress)
    {
        InitClient(HostName(blockChainType));

        HttpRequestMessage request = CreateHTTPRequest(blockChainType, HttpMethod.Post, "GetEtherBalance", new GetEtherBalanceArgs(accountAddress));

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        return BuildResponse<GetEtherBalanceReturn>(responseBody).balance;
    }

    /// <summary>
    /// Retrieve an account's token balances, optionally for a specific contract
    /// </summary>
    /// <exception cref="HttpRequestException">If the network request fails</exception>
    public static async Task<GetTokenBalancesReturn> GetTokenBalances(BlockChainType blockChainType, GetTokenBalancesArgs args)
    {
        InitClient(HostName(blockChainType));

        HttpRequestMessage request = CreateHTTPRequest(blockChainType, HttpMethod.Post, "GetTokenBalances", args);
        Debug.Log("from indexer gettokenbalance request:" + request + args);

        HttpResponseMessage response = await client.SendAsync(request);
        Debug.Log("from indexer gettokenbalance response:" + response);

        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        return BuildResponse<GetTokenBalancesReturn>(responseBody);
    }

    /// <summary>
    /// Retrieve the token supply for a given contract
    /// </summary>
    /// <exception cref="HttpRequestException">If the network request fails</exception>
    public static async Task<GetTokenSuppliesReturn> GetTokenSupplies(BlockChainType blockChainType, GetTokenSuppliesArgs args)
    {
        InitClient(HostName(blockChainType));

        HttpRequestMessage request = CreateHTTPRequest(blockChainType, HttpMethod.Post, "GetTokenSupplies", args);

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        return BuildResponse<GetTokenSuppliesReturn>(responseBody);
    }

    /// <summary>
    /// Retrieve <see cref="GetTokenSuppliesMapReturn"/>
    /// </summary>
    /// <exception cref="HttpRequestException">If the network request fails</exception>
    public static async Task<GetTokenSuppliesMapReturn> GetTokenSuppliesMap(BlockChainType blockChainType, GetTokenSuppliesMapArgs args)
    {
        InitClient(HostName(blockChainType));

        HttpRequestMessage request = CreateHTTPRequest(blockChainType, HttpMethod.Post, "GetTokenSuppliesMap", args);

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        return BuildResponse<GetTokenSuppliesMapReturn>(responseBody);

    }

    /// <summary>
    /// Retrieve <see cref="GetBalanceUpdatesReturn"/>
    /// </summary>
    /// <exception cref="HttpRequestException">If the network request fails</exception>
    public static async Task<GetBalanceUpdatesReturn> GetBalanceUpdates(BlockChainType blockChainType, GetBalanceUpdatesArgs args)
    {
        InitClient(HostName(blockChainType));

        HttpRequestMessage request = CreateHTTPRequest(blockChainType, HttpMethod.Post, "GetBalanceUpdates", args);

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        return BuildResponse<GetBalanceUpdatesReturn>(responseBody);
    }

    /// <summary>
    /// Retrieve transaction history <see cref="GetTransactionHistoryReturn"/>
    /// </summary>
    /// <exception cref="HttpRequestException">If the network request fails</exception>

    public static async Task<GetTransactionHistoryReturn> GetTransactionHistory(BlockChainType blockChainType, GetTransactionHistoryArgs args)
    {
        InitClient(HostName(blockChainType));

        HttpRequestMessage request = CreateHTTPRequest(blockChainType, HttpMethod.Post, "GetTransactionHistory", args);

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        return BuildResponse<GetTransactionHistoryReturn>(responseBody);
    }

    /// <summary>
    /// Creates and returns a new HTTP Request with mediatype set to application/json
    /// </summary>
    /// <param name="method"></param>
    /// <param name="endPoint"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    private static HttpRequestMessage CreateHTTPRequest(BlockChainType blockChainType, HttpMethod method, string endPoint, object args)
    {
        return new(method, Url(blockChainType, endPoint))
        {
            Content = new StringContent(JsonConvert.SerializeObject(args), Encoding.UTF8, "application/json")
        };
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
