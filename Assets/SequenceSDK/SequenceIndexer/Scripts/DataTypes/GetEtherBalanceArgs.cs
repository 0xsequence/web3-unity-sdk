[System.Serializable]
public class GetEtherBalanceArgs
{
    public string accountAddress;

    public GetEtherBalanceArgs()
    {

    }

    public GetEtherBalanceArgs(string accountAddress)
    {
        this.accountAddress = accountAddress;
    }
}
