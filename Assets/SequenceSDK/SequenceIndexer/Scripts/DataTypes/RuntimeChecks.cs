using System.Numerics;

namespace SequenceSharp
{
    [System.Serializable]
    public class RuntimeChecks
    {
        public bool running;
        public string syncMode;
        public BigInteger lastBlockNum;
    }
}