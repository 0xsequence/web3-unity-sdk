using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used for testing each function in the <see cref="Indexer"/> static class
/// </summary>
public class IndexerFunctionTest : MonoBehaviour
{
    private const BlockChainType BLOCKCHAIN_TYPE = BlockChainType.Polygon;

    private void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestGetEtherBalances();
        }
    }

    private async void TestPing()
    {
        var status = await Indexer.Ping(BlockChainType.Polygon);
        Debug.Log("Ping Status: " + status);
    }

    private async void TestVersion()
    {
        var version = await Indexer.Version(BLOCKCHAIN_TYPE);
        Debug.Log("App Version: " + version.appVersion);
    }

    private async void TestRuntimeStatus()
    {
        var status = await Indexer.RuntimeStatus(BLOCKCHAIN_TYPE);
        Debug.Log("Runtime health ok: " + status.healthOK);
    }


    private async void TestGetChainID()
    {
        var chainID = await Indexer.GetChainID(BLOCKCHAIN_TYPE);

        Debug.Log("ChainID: " + chainID);
    }

    // Returns "Response status code does not indicate success: 401 (Unauthorized)."
    private async void TestGetEtherBalances()
    {
        var etherBalance = await Indexer.GetEtherBalance(BLOCKCHAIN_TYPE, "0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9");

        if (etherBalance != null)
        {
            Debug.Log("Ether Balance: " + etherBalance);
        }
    }

    private async void TestGetTokenBalances()
    {
        GetTokenBalancesArgs balanceArgs = new GetTokenBalancesArgs
            ("0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9",
            "0x631998e91476DA5B870D741192fc5Cbc55F5a52E",
            true);

        var tokenBalances = await Indexer.GetTokenBalances(BLOCKCHAIN_TYPE, balanceArgs);

        Debug.Log("Balances found: " + tokenBalances.balances.Length);
    }

    private async void TestGetTokenSupplies()
    {
        GetTokenSuppliesArgs tokenSuppliesArgs = new GetTokenSuppliesArgs("0x631998e91476DA5B870D741192fc5Cbc55F5a52E", true);

        var tokenSuppliesReturn = await Indexer.GetTokenSupplies(BLOCKCHAIN_TYPE, tokenSuppliesArgs);

        if (tokenSuppliesReturn != null)
        {
            Debug.Log("Token Supplies contract type: " + tokenSuppliesReturn.contractType);
        }
    }


    private async void TestGetTokenSuppliesMap()
    {
        Dictionary<string, string[]> tokenSuppliesMap = new Dictionary<string, string[]>();
        tokenSuppliesMap.Add("0x631998e91476da5b870d741192fc5cbc55f5a52e", new string[]
        {
            "65539",
            "65543"
        });

        GetTokenSuppliesMapArgs tokenSuppliesMapArgs = new GetTokenSuppliesMapArgs(tokenSuppliesMap, true);

        var tokenSuppliesMapReturn = await Indexer.GetTokenSuppliesMap(BLOCKCHAIN_TYPE, tokenSuppliesMapArgs);

        if (tokenSuppliesMapReturn != null)
        {
            Debug.Log("Token Supplies Map Count: " + tokenSuppliesMapReturn.supplies.Count);
        }
    }


    private async void TestGetBalanceUpdates()
    {
        GetBalanceUpdatesArgs balanceUpdatesArgs = new GetBalanceUpdatesArgs("0x631998e91476DA5B870D741192fc5Cbc55F5a52E");
        var balanceUpdateReturn = await Indexer.GetBalanceUpdates(BLOCKCHAIN_TYPE, balanceUpdatesArgs);

        Debug.Log("Balances returned: " + balanceUpdateReturn.balances.Length);
    }

    private async void TestGetTransactionHistory()
    {
        TransactionHistoryFilter transactionFilter = new TransactionHistoryFilter();
        transactionFilter.accountAddress = "0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9";
        transactionFilter.contractAddress = "0x631998e91476DA5B870D741192fc5Cbc55F5a52E";

        GetTransactionHistoryArgs transactionHistoryArgs = new GetTransactionHistoryArgs(transactionFilter);
        var transactionHistoryReturn = await Indexer.GetTransactionHistory(BLOCKCHAIN_TYPE, transactionHistoryArgs);

        if (transactionHistoryReturn != null)
        {
            Debug.Log("Transactions returned: " + transactionHistoryReturn.transactions.Length);
        }
    }
}
