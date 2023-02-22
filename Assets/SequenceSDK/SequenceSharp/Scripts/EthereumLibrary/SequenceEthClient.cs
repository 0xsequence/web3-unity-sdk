using System.Collections.Generic;
using System.Numerics;

namespace SequenceSharp
{
    public class SequenceEthClient : IEthClient
    {
        public BigInteger BalanceAt(string address, string blockNumber)
        {
            //[FOCUS IMPLEMENTATION]
            throw new System.NotImplementedException();
        }

        public Block BlockByHash(string blockHash)
        {
            //[FOCUS IMPLEMENTATION]
            throw new System.NotImplementedException();
        }

        public Block BlockByNumber(string blockNumber)
        {
            //[FOCUS IMPLEMENTATION]
            throw new System.NotImplementedException();
        }

        public string BlockNumber()
        {
            //[FOCUS IMPLEMENTATION]
            throw new System.NotImplementedException();
        }

        public List<Block> BlockRange(string start = "earliest", string end = "earliest", bool? full = true)
        {
            throw new System.NotImplementedException();
        }

        public string CallContract()
        {
            //[FOCUS IMPLEMENTATION]
            throw new System.NotImplementedException();
        }

        public string CallContractAtHash()
        {
            throw new System.NotImplementedException();
        }

        public string ChainID()
        {
            //[FOCUS IMPLEMENTATION]
            throw new System.NotImplementedException();
        }

        public string CodeAt(string address, string blockNumber)
        {
            //[FOCUS IMPLEMENTATION]
            throw new System.NotImplementedException();
        }

        public BigInteger EstimateGas(TransactionCall transactionCall, string blockNumber)
        {
            throw new System.NotImplementedException();
        }

        public FeeHistoryResult FeeHistory(string blockCount, string newestBlock, int? REWARDPERCENTILES)
        {
            //[FOCUS IMPLEMENTATION]
            throw new System.NotImplementedException();
        }

        public List<Log> FilterLogs(Filter filter)
        {
            throw new System.NotImplementedException();
        }

        public void HeaderByHash(string blockHash)
        {
            //[FOCUS IMPLEMENTATION]
            throw new System.NotImplementedException();
        }

        public void HeaderByNumber(string blockNumber)
        {
            //[FOCUS IMPLEMENTATION]
            throw new System.NotImplementedException();
        }

        public string NetworkID()
        {
            throw new System.NotImplementedException();
        }

        public BigInteger NonceAt(string address, string blockNumber)
        {
            //[FOCUS IMPLEMENTATION]
            throw new System.NotImplementedException();
        }

        public BigInteger PeerCount()
        {
            throw new System.NotImplementedException();
        }

        public BigInteger PendingBalanceAt(string address)
        {
            throw new System.NotImplementedException();
        }

        public string PendingCallContract(TransactionCall transactionCall)
        {
            throw new System.NotImplementedException();
        }

        public string PendingCodeAt(string address)
        {
            throw new System.NotImplementedException();
        }

        public BigInteger PendingNonceAt(string address)
        {
            throw new System.NotImplementedException();
        }

        public string PendingStorageAt(string address, BigInteger storagePosition)
        {
            throw new System.NotImplementedException();
        }

        public BigInteger PendingTransactionCount()
        {
            throw new System.NotImplementedException();
        }

        public string SendRawTransaction(string signedTransactionData)
        {
            //[FOCUS IMPLEMENTATION]
            throw new System.NotImplementedException();
        }

        public string SendTransaction(Transaction transaction)
        {
            throw new System.NotImplementedException();
        }

        public string StorageAt(string address, BigInteger storagePosition, string blockNumber)
        {
            throw new System.NotImplementedException();
        }

        public BigInteger SuggestGasPrice()
        {
            //[FOCUS IMPLEMENTATION]
            throw new System.NotImplementedException();
        }

        public string SuggestGasTipCap()
        {
            //[FOCUS IMPLEMENTATION]
            throw new System.NotImplementedException();
        }

        public SyncStatus SyncProgress()
        {
            throw new System.NotImplementedException();
        }

        public Transaction TransactionByHash(string transactionHash)
        {
            //[FOCUS IMPLEMENTATION]
            throw new System.NotImplementedException();
        }

        public BigInteger TransactionCount(string blockHash)
        {
            //[FOCUS IMPLEMENTATION]
            throw new System.NotImplementedException();
        }

        public Transaction TransactionInBlock(string blockHash, BigInteger transactionIndex)
        {
            throw new System.NotImplementedException();
        }

        public TransactionReceipt TransactionReceipt(string transactionHash)
        {
            //[FOCUS IMPLEMENTATION]
            throw new System.NotImplementedException();
        }

        public Transaction TransactionSender(string blockHash, BigInteger transactionIndex)
        {
            throw new System.NotImplementedException();
        }
    }
}