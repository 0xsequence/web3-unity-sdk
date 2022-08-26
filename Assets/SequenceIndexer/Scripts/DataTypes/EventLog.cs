[System.Serializable]
public class EventLog
{
    public int id;
    public EventLogType type;
    public int blockNumber;
    public string blockHash;
    public string contractAddress;
    public ContractType contractType;
    public string txnHash;
    public int txnIndex;
    public int txnLogIndex;
    public EventLogDataType logDataType;
    public string ts;
    public string logData;
}
