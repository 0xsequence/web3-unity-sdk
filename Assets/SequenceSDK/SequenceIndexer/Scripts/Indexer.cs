using System;
using UnityEngine;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

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

    public delegate void PingReturnCallback(PingReturn pingReturn);
    public delegate void GetTokenBalancesCallback(GetTokenBalancesReturn tokenBalancesReturn);
    public delegate void VersionCallback(VersionReturn versionReturn);
    public delegate void GetEtherBalanceCallback(GetEtherBalanceReturn getEtherBalanceReturn);
    public delegate void RuntimeStatusCallback(RuntimeStatusReturn runtimeStatusReturn);
    public delegate void GetChainIDCallback(GetChainIDReturn getChainIDReturn);
    public delegate void GetTokenSuppliesCallback(GetTokenSuppliesReturn getTokenSuppliesReturn);
    public delegate void GetTokenSuppliesMapCallback(GetTokenSuppliesMapReturn getTokenSuppliesMapReturn);
    public delegate void GetBalanceUpdatesCallback(GetBalanceUpdatesReturn getBalanceUpdatesReturn);
    public delegate void GetTransactionHistoryCallback(GetTransactionHistoryReturn getTransactionHistoryReturn);
    public delegate void SyncBalanceCallback(SyncBalanceReturn syncBalanceReturn);

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
    /// Retrive <see cref="PingReturn"/> via <paramref name="eventResponse"/> return value
    /// </summary>
    /// <param name="eventResponse"></param>
    public static async void Ping(BlockChainType blockChainType, PingReturnCallback eventResponse)
    {
        try
        {
            InitClient(HostName(blockChainType));

            HttpRequestMessage request = CreateHTTPRequest(blockChainType, HttpMethod.Post, "Ping", null);

            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            eventResponse?.Invoke(BuildResponse<PingReturn>(responseBody));
        }
        catch (HttpRequestException e)
        {
            Debug.Log($"[ Exception Caught ] Message : {e.Message}");
            eventResponse?.Invoke(new PingReturn(false));
        }
    }

    /// <summary>
    /// Retrieve <see cref="VersionReturn"/> via <paramref name="eventResponse"/> return value
    /// </summary>
    /// <param name="eventResponse"></param>
    public static async void Version(BlockChainType blockChainType, VersionCallback eventResponse)
    {
        try
        {
            InitClient(HostName(blockChainType));

            HttpRequestMessage request = CreateHTTPRequest(blockChainType, HttpMethod.Post, "Version", null);

            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            eventResponse?.Invoke(BuildResponse<VersionReturn>(responseBody));
        }
        catch (HttpRequestException e)
        {
            Debug.Log($"[ Exception Caught ] Message : {e.Message}");
            eventResponse?.Invoke(null);
        }
    }

    /// <summary>
    /// Retrieve <see cref="RuntimeStatusReturn"/> via <paramref name="eventResponse"/> return value
    /// </summary>
    /// <param name="eventResponse"></param>
    public static async void RuntimeStatus(BlockChainType blockChainType, RuntimeStatusCallback eventResponse)
    {
        try
        {
            InitClient(HostName(blockChainType));

            HttpRequestMessage request = CreateHTTPRequest(blockChainType, HttpMethod.Post, "RuntimeStatus", null);

            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            eventResponse?.Invoke(BuildResponse<RuntimeStatusReturn>(responseBody));
        }
        catch (HttpRequestException e)
        {
            Debug.Log($"[ Exception Caught ] Message : {e.Message}");
            eventResponse?.Invoke(null);
        }
    }

    /// <summary>
    /// Retrieve <see cref="GetChainIDReturn"/> via <paramref name="eventResponse"/> return value
    /// </summary>
    /// <param name="eventResponse"></param>
    public static async void GetChainID(BlockChainType blockChainType, GetChainIDCallback eventResponse)
    {
        try
        {
            InitClient(HostName(blockChainType));

            HttpRequestMessage request = CreateHTTPRequest(blockChainType, HttpMethod.Post, "GetChainID", null);

            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            eventResponse?.Invoke(BuildResponse<GetChainIDReturn>(responseBody));
        }
        catch (HttpRequestException e)
        {
            Debug.Log($"[ Exception Caught ] Message : {e.Message}");
            eventResponse?.Invoke(null);
        }
    }

    /// <summary>
    /// Need example data to test this
    /// </summary>
    /// <param name="args"></param>
    /// <param name="eventResponse"></param>
    public static async void GetEtherBalance(BlockChainType blockChainType, GetEtherBalanceArgs args, GetEtherBalanceCallback eventResponse)
    {
        try
        {
            InitClient(HostName(blockChainType));

            HttpRequestMessage request = CreateHTTPRequest(blockChainType, HttpMethod.Post, "GetEtherBalance", args);

            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            eventResponse?.Invoke(BuildResponse<GetEtherBalanceReturn>(responseBody));
        }
        catch (HttpRequestException e)
        {
            Debug.Log($"[ Exception Caught ] Message : {e.Message}");
            eventResponse?.Invoke(null);
        }
    }

    /// <summary>
    /// Retrieve <see cref="GetTokenBalancesReturn"/> via <paramref name="eventResponse"/> return value
    /// </summary>
    /// <param name="args"></param>
    /// <param name="eventResponse"></param>
    public static async void GetTokenBalances(BlockChainType blockChainType, GetTokenBalancesArgs args, GetTokenBalancesCallback eventResponse)
    {
        try
        {
            InitClient(HostName(blockChainType));

            HttpRequestMessage request = CreateHTTPRequest(blockChainType, HttpMethod.Post, "GetTokenBalances", args);
            Debug.Log("from indexer gettokenbalance request:"+ request);

            HttpResponseMessage response = await client.SendAsync(request);
            Debug.Log("from indexer gettokenbalance response:" + response);

            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            eventResponse?.Invoke(BuildResponse<GetTokenBalancesReturn>(responseBody));
        }
        catch (HttpRequestException e)
        {
            Debug.Log($"[ Exception Caught ] Message : {e.Message}");
            eventResponse?.Invoke(null);
        }
    }

    /// <summary>
    /// Retrieve <see cref="GetTokenSuppliesReturn"/> via <paramref name="eventResponse"/> return value
    /// </summary>
    /// <param name="args"></param>
    /// <param name="eventResponse"></param>
    public static async void GetTokenSupplies(BlockChainType blockChainType, GetTokenSuppliesArgs args, GetTokenSuppliesCallback eventResponse)
    {
        try
        {
            InitClient(HostName(blockChainType));

            HttpRequestMessage request = CreateHTTPRequest(blockChainType, HttpMethod.Post, "GetTokenSupplies", args);

            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            eventResponse?.Invoke(BuildResponse<GetTokenSuppliesReturn>(responseBody));
        }
        catch (HttpRequestException e)
        {
            Debug.Log($"[ Exception Caught ] Message : {e.Message}");
            eventResponse?.Invoke(null);
        }
    }

    /// <summary>
    /// Retrieve <see cref="GetTokenSuppliesMapReturn"/> via <paramref name="eventResponse"/> return value
    /// </summary>
    /// <param name="args"></param>
    /// <param name="eventResponse"></param>
    public static async void GetTokenSuppliesMap(BlockChainType blockChainType, GetTokenSuppliesMapArgs args, GetTokenSuppliesMapCallback eventResponse)
    {
        try
        {
            InitClient(HostName(blockChainType));

            HttpRequestMessage request = CreateHTTPRequest(blockChainType, HttpMethod.Post, "GetTokenSuppliesMap", args);

            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            eventResponse?.Invoke(BuildResponse<GetTokenSuppliesMapReturn>(responseBody));
        }
        catch (HttpRequestException e)
        {
            Debug.Log($"[ Exception Caught ] Message : {e.Message}");
            eventResponse?.Invoke(null);
        }
    }

    /// <summary>
    /// Retrieve <see cref="GetBalanceUpdatesReturn"/> via <paramref name="eventResponse"/> return value
    /// </summary>
    /// <param name="args"></param>
    /// <param name="eventResponse"></param>
    public static async void GetBalanceUpdates(BlockChainType blockChainType, GetBalanceUpdatesArgs args, GetBalanceUpdatesCallback eventResponse)
    {
        try
        {
            InitClient(HostName(blockChainType));

            HttpRequestMessage request = CreateHTTPRequest(blockChainType, HttpMethod.Post, "GetBalanceUpdates", args);

            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            eventResponse?.Invoke(BuildResponse<GetBalanceUpdatesReturn>(responseBody));
        }
        catch (HttpRequestException e)
        {
            Debug.Log($"[ Exception Caught ] Message : {e.Message}");
            eventResponse?.Invoke(null);
        }
    }

    /// <summary>
    /// Retrieve <see cref="GetTransactionHistoryReturn"/> via <paramref name="eventResponse"/> return value
    /// </summary>
    /// <param name="args"></param>
    /// <param name="eventResponse"></param>
    public static async void GetTransactionHistory(BlockChainType blockChainType, GetTransactionHistoryArgs args, GetTransactionHistoryCallback eventResponse)
    {
        try
        {
            InitClient(HostName(blockChainType));

            HttpRequestMessage request = CreateHTTPRequest(blockChainType, HttpMethod.Post, "GetTransactionHistory", args);

            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            eventResponse?.Invoke(BuildResponse<GetTransactionHistoryReturn>(responseBody));
        }
        catch (HttpRequestException e)
        {
            Debug.Log($"[ Exception Caught ] Message : {e.Message}");
            eventResponse?.Invoke(null);
        }
    }

    /// <summary>
    /// Need example data input to test
    /// </summary>
    /// <param name="args"></param>
    /// <param name="eventResponse"></param>
    public static async void SyncBalance(BlockChainType blockChainType, SyncBalanceArgs args, SyncBalanceCallback eventResponse)
    {
        try
        {
            InitClient(HostName(blockChainType));

            HttpRequestMessage request = CreateHTTPRequest(blockChainType, HttpMethod.Post, "SyncBalance", args);

            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            eventResponse?.Invoke(BuildResponse<SyncBalanceReturn>(responseBody));
        }
        catch (HttpRequestException e)
        {
            Debug.Log($"[ Exception Caught ] Message : {e.Message}");
            eventResponse?.Invoke(null);
        }
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
        Debug.Log("Path: " + Url(blockChainType, endPoint));

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
        try
        {
            T data = JsonConvert.DeserializeObject<T>(text);
            Debug.Log("Data returned from response: \n" + text);

            return data;
        }
        catch (Exception e)
        {
            Debug.Log($"{e}");
            Debug.Log($"[Exception Caught] Expecting JSON, got: '{text}'");

            return default;
        }
    }
}
