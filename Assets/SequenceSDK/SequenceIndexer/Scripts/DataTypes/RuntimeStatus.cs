[System.Serializable]
public class RuntimeStatus
{
    public bool healthOK;
    public bool indexerEnabled;
    public string startTime;
    public double uptime;
    public string ver;
    public string branch;
    public string commitHash;
    public int chainID;
    public RuntimeChecks checks;
}
