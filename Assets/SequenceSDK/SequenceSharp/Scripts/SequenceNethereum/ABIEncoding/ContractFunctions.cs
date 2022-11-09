
using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Numerics;
using Nethereum.Contracts;

namespace SequenceSharp
{
    //Inherits from Nethereum FunctionMessage for ERC20 Token Functions

    [Function("name", "string")]
    public class NameFunction : FunctionMessage { }
    [Function("symbol", "string")]
    public class SymbolFunction : FunctionMessage { }
    [Function("decimals", "string")]
    public class DecimalsFunction : FunctionMessage { }

    [Function("totalSupply", "uint256")]
    public class TotalSupplyFunction : FunctionMessage { }

    [Function("balanceOf", "uint256")]
    public class BalanceOfFunction : FunctionMessage
    {
        [Parameter("address", "_owner", 1)]
        public string Owner { get; set; }
    }
    [Function("transfer", "bool")]
    public class TransferFunction : FunctionMessage
    {
        [Parameter("address", "_to", 1)]
        public string To { get; set; }

        [Parameter("uint256", "_value", 2)]
        public BigInteger TokenAmount { get; set; }
    }
}
