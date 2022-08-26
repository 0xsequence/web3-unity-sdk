[System.Serializable]
public class SyncBalanceArgs
{
    public string accountAddress;
    public string contractAddress;
    public string tokenID;

    public SyncBalanceArgs()
    {

    }

    public SyncBalanceArgs(string accountAddress)
    {
        this.accountAddress = accountAddress;
    }

    public SyncBalanceArgs(string accountAddress, string contractAddress)
    {
        this.accountAddress = accountAddress;
        this.contractAddress = contractAddress;
    }

    public SyncBalanceArgs(string accountAddress, string contractAddress, string tokenID)
    {
        this.accountAddress = accountAddress;
        this.contractAddress = contractAddress;
        this.tokenID = tokenID;
    }
}
