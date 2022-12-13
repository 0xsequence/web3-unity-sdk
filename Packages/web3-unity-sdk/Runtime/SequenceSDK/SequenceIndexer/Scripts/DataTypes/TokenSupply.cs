using System.Numerics;

namespace SequenceSharp
{
    [System.Serializable]
    public class TokenSupply
    {
        public BigInteger tokenID;
        public string supply;
        public BigInteger chainId;
        public ContractInfo contractInfo;
        public TokenMetadata tokenMetadata;
    }
}