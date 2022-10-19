namespace SequenceSharp
{
    [System.Serializable]
    public class ContractInfo
    {
        public int chainId;
        public string address;
        public string name;
        public string type;
        public string symbol;
        public int decimals;
        public string logoURI;
        public ContractInfoExtensions extensions;
    }
}