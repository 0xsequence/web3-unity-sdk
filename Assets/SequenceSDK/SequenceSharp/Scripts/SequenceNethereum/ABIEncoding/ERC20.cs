using System.Numerics;
using System.Threading.Tasks;
using System;

public class ERC20
{
    public static async Task<string> Name()
    {
        throw new NotImplementedException();
    }

    public static async Task<string> Symbol()
    {
        throw new NotImplementedException();
    }

    public static async Task<uint> Decimals()
    {
        throw new NotImplementedException();
    }
    public static async Task<BigInteger> TotalSupply()
    {
        throw new NotImplementedException();
    }

    public static async Task<BigInteger> BalanceOf(string account)
    {
        throw new NotImplementedException();
    }

    public static async Task<int> Transfer(string recipient, string amount)
    {
        throw new NotImplementedException();
    }

    public static async Task<BigInteger> Allowance(string owner, string spender)
    {
        throw new NotImplementedException();
    }

    public static async Task<bool> Approve(string spender, string amount)
    {
        throw new NotImplementedException();
    }

    public static async Task<bool> TransferFrom(string sender, string recipient,string amount)
    {
        throw new NotImplementedException();
    }

    public static async Task<bool> IncreaseAllowance(string spender, BigInteger addedValue)
    {
        throw new NotImplementedException();
    }

    public static async Task<bool> DecreaseAllowance(string spender, BigInteger subtractedValue)
    {
        throw new NotImplementedException();
    }
}
