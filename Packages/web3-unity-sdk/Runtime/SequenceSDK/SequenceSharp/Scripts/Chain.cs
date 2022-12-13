using System.Numerics;

namespace SequenceSharp
{
    public static class Chain
    {
        // Mainnets
        public static readonly BigInteger Ethereum = BigInteger.Parse("1");
        public static readonly BigInteger Polygon = BigInteger.Parse("137");
        public static readonly BigInteger BNBSmartChain = BigInteger.Parse("56");
        public static readonly BigInteger ArbitrumOne = BigInteger.Parse("42161");
        public static readonly BigInteger ArbitrumNova = BigInteger.Parse("42170");
        public static readonly BigInteger Optimism = BigInteger.Parse("10");
        public static readonly BigInteger Avalanche = BigInteger.Parse("43114");
        public static readonly BigInteger Gnosis = BigInteger.Parse("100");

        // Testnets
        public static readonly BigInteger TestnetGoerli = BigInteger.Parse("5");
        public static readonly BigInteger TestnetPolygonMumbai = BigInteger.Parse("80001");
        public static readonly BigInteger TestnetBNBSmartChain = BigInteger.Parse("97");
        public static readonly BigInteger TestnetAvalancheFuji = BigInteger.Parse("43113");
    }
}