using System.Collections.Generic;

[System.Serializable]
public class TxnTransfer
{
    public TxnTransferType transferType;
    public string contractAddress;
    public ContractType contractType;
    public string from;
    public string to;
    public string[] tokenIds;
    public string[] amounts;
    public int logIndex;
    public ContractInfo contractInfo;
    public Dictionary<string, TokenMetadata> tokenMetaData;
}
