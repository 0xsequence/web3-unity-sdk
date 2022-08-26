using System.Numerics;

[System.Serializable]
public class TokenBalance
{
    public int id;
    public string contractAddress;
    public ContractType contractType;
    public string accountAddress;
    public string tokenID;
    public string balance;
    public string blockHash;
    public int blockNumber;
    // public int updateId;
    public BigInteger updateID;
    public int chainId;
    public ContractInfo contractInfo;
    public TokenMetadata tokenMetadata;
}
