using System.Numerics;

namespace SequenceSharp
{
    [System.Serializable]
    public class Transaction
    {
        public string txnHash;
        public BigInteger blockNumber;
        public string blockHash;
        public BigInteger chainId;
        public string metaTxnID;
        public TxnTransfer[] transfers;
        public string timestamp;
    }
}