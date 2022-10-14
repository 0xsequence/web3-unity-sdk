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
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TestGetEtherBalances();
        }
    }

    private void TestPing()
    {
        Indexer.Ping(BlockChainType.Polygon, (pingReturn) =>
        {
            Debug.Log("Ping Status: " + pingReturn.status);
        });
    }

    private void TestVersion()
    {
        Indexer.Version(BLOCKCHAIN_TYPE, (version) =>
        {
            Debug.Log("App Version: " + version.version.appVersion);
        });
    }

    private void TestRuntimeStatus()
    {
        Indexer.RuntimeStatus(BLOCKCHAIN_TYPE, (runtimeStatus) =>
        {
            Debug.Log("Runtime health ok: " + runtimeStatus.status.healthOK);
        });
    }

    private void TestGetChainID()
    {
        Indexer.GetChainID(BLOCKCHAIN_TYPE, (chainID) =>
        {
            Debug.Log("ChainID: " + chainID.chainID);
        });
    }

    // Returns "Response status code does not indicate success: 401 (Unauthorized)."
    private void TestGetEtherBalances()
    {
        GetEtherBalanceArgs etherBalanceArgs = new GetEtherBalanceArgs("0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9");

        Indexer.GetEtherBalance(BLOCKCHAIN_TYPE, etherBalanceArgs, (etherBalance) =>
        {
            if (etherBalance != null)
            {
                Debug.Log("Ether Balance: " + etherBalance.balance);
            }
        });
    }

    private void TestGetTokenBalances()
    {
        GetTokenBalancesArgs balanceArgs = new GetTokenBalancesArgs
            ("0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9",
            "0x631998e91476DA5B870D741192fc5Cbc55F5a52E",
            true);

        Indexer.GetTokenBalances(BLOCKCHAIN_TYPE, balanceArgs, (tokenBalances) =>
        {
            Debug.Log("Balances found: " + tokenBalances.balances.Length);
        });
    }

    private void TestGetTokenSupplies()
    {
        GetTokenSuppliesArgs tokenSuppliesArgs = new GetTokenSuppliesArgs("0x631998e91476DA5B870D741192fc5Cbc55F5a52E", true);

        Indexer.GetTokenSupplies(BLOCKCHAIN_TYPE, tokenSuppliesArgs, (tokenSuppliesReturn) =>
        {
            if (tokenSuppliesReturn != null)
            {
                Debug.Log("Token Supplies contract type: " + tokenSuppliesReturn.contractType);
            }
        });
    }

    
    private void TestGetTokenSuppliesMap()
    {
        Dictionary<string, string[]> tokenSuppliesMap = new Dictionary<string, string[]>();
        tokenSuppliesMap.Add("0x631998e91476da5b870d741192fc5cbc55f5a52e", new string[]
        {
            "65539",
            "65543"
        });

        GetTokenSuppliesMapArgs tokenSuppliesMapArgs = new GetTokenSuppliesMapArgs(tokenSuppliesMap, true);

        Indexer.GetTokenSuppliesMap(BLOCKCHAIN_TYPE, tokenSuppliesMapArgs, (tokenSuppliesMapReturn) =>
        {
            if (tokenSuppliesMapReturn != null)
            {
                Debug.Log("Token Supplies Map Count: " + tokenSuppliesMapReturn.supplies.Count);
            }
        });
    }

    
    private void TestGetBalanceUpdates()
    {
        GetBalanceUpdatesArgs balanceUpdatesArgs = new GetBalanceUpdatesArgs("0x631998e91476DA5B870D741192fc5Cbc55F5a52E");
        Indexer.GetBalanceUpdates(BLOCKCHAIN_TYPE, balanceUpdatesArgs, (balanceUpdateReturn) =>
        {
            Debug.Log("Balances returned: " + balanceUpdateReturn.balances.Length);
        });
    }

    private void TestGetTransactionHistory()
    {
        TransactionHistoryFilter transactionFilter = new TransactionHistoryFilter();
        transactionFilter.accountAddress = "0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9";
        transactionFilter.contractAddress = "0x631998e91476DA5B870D741192fc5Cbc55F5a52E";

        GetTransactionHistoryArgs transactionHistoryArgs = new GetTransactionHistoryArgs(transactionFilter);
        Indexer.GetTransactionHistory(BLOCKCHAIN_TYPE, transactionHistoryArgs, (transactionHistoryReturn) =>
        {
            if (transactionHistoryReturn != null)
            {
                Debug.Log("Transactions returned: " + transactionHistoryReturn.transactions.Length);
            }
        });
    }

    // Currently returning "Response status code does not indicate success: 401 (Unauthorized)."
    private void TestSyncBalance()
    {
        SyncBalanceArgs syncBalanceArgs = new SyncBalanceArgs(
            "0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9",
            "0x631998e91476DA5B870D741192fc5Cbc55F5a52E",
            "65539"
            );
        Indexer.SyncBalance(BLOCKCHAIN_TYPE, syncBalanceArgs, (syncBalance) =>
        {
            if (syncBalance != null)
            {
                Debug.Log("Balance Synced");
            }
        });
    }
}
