namespace SequenceSharp
{
    [System.Serializable]
    public class Transaction
    {
        public string txnHash;
        public int blockNumber;
        public string blockHash;
        public int chainId;
        public string metaTxnID;
        public TxnTransfer[] transfers;
        public string timestamp;
    }
}