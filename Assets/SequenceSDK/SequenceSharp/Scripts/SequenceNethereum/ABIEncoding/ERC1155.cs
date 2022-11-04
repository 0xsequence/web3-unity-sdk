using System.Numerics;
using System.Threading.Tasks;
using System;

public class ERC1155
{
    public static async Task<string> URI(BigInteger _)
    {
        throw new NotImplementedException();
    }

    public static async Task BalanceOf<BigInteger>(string account, BigInteger id)
    {
        throw new NotImplementedException();
    }

    public static async Task<BigInteger[]> BalanceOfBatch(string[] accounts, BigInteger[] ids)
    {
        throw new NotImplementedException();
    }

    public static async Task SetApprovalForAll(string operatorAddress, bool approved)
    {
        throw new NotImplementedException();
    }
    public static async Task<bool> IsApprovedForAll(string account, string operatorAddress)
    {
        throw new NotImplementedException();
    }
    public static async Task SafeTransferFrom(string from, string to, BigInteger id, BigInteger amount, string data)
    {
        throw new NotImplementedException();
    }
    public static async Task SafeBatchTransferFrom(string from, string to, BigInteger[] ids, BigInteger[] amounts, string data)
    {
        throw new NotImplementedException();
    }
}