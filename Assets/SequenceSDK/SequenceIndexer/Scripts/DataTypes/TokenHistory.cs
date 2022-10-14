[System.Serializable]
public class TokenHistory
{
    public int id;
    public int blockNumber;
    public string blockHash;
    public string contractAddress;
    public ContractType contractType;
    public string fromAddress;
    public string toAddress;
    public string txnHash;
    public int txnIndex;
    public int txnLogIndex;
    public string logData;
    public string ts;
}
