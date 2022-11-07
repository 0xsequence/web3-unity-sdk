using System.Numerics;
using System.Threading.Tasks;
using System;
using UnityEngine;
namespace SequenceSharp
{
    public class ERC20 : MonoBehaviour
    {
        private readonly static string abi = @"[
    // Read-Only Functions
    'function name() view returns (string)',
    'function balanceOf(address owner) view returns (uint256)',
    'function decimals() view returns (uint8)',
    'function symbol() view returns (string)',
    'function totalSupply() view returns (uint256)',

    // Authenticated Functions
    'function transfer(address to, uint amount) returns (bool)',

    
]";

        private static Wallet _wallet;

        private void Awake()
        {
            _wallet = FindObjectOfType<Wallet>();
        }

        public static async Task<string> Name(string address)
        {
            //throw new NotImplementedException();
            Debug.Log("erc20 name: "+ _wallet);
            string name = await _wallet.ExecuteSequenceJS(@"
                const provider = seq.getWallet().getProvider();
                const network = provider.getNetwork();
                const abi =" + abi + @";
                const erc20 = new ethers.Contract('" + address+ @"', abi, provider);               
                var name = await erc20.name();
                return name;

            ");

            return name;
        }

        public static async Task<string> Symbol(string address)
        {
            //throw new NotImplementedException();
            string symbol = await _wallet.ExecuteSequenceJS(@"
                const provider = seq.getWallet().getProvider();
                const network = provider.getNetwork();
                const abi =" + abi + @";
                const erc20 = new ethers.Contract('" + address + @"', abi, provider);

                var symbol = await erc20.symbol();
                return symbol;
            ");
            return symbol;
        }

        public static async Task<BigInteger> Decimals(string address)
        {
            //throw new NotImplementedException();
            var decimals = BigInteger.Parse(await _wallet.ExecuteSequenceJS(@"
                const provider = seq.getWallet().getProvider();
                const network = provider.getNetwork();
                const abi =" + abi + @";
                const erc20 = new ethers.Contract('" + address + @"', abi, provider);

                var decimals = await erc20.decimals();
                return decimals;
            "));
            return decimals;
        }
        public static async Task<BigInteger> TotalSupply(string address)
        {
            //throw new NotImplementedException();
            var totalSupply = BigInteger.Parse(await _wallet.ExecuteSequenceJS(@"
                const provider = seq.getWallet().getProvider();
                const network = provider.getNetwork();
                const abi =" + abi + @";
                const erc20 = new ethers.Contract('" + address + @"', abi, provider);

                var totalSupply = await erc20.totalSupply();
                return totalSupply;
            "));
            return totalSupply;
        }

        public static async Task<BigInteger> BalanceOf(string account, string address)
        {
            //throw new NotImplementedException();
            var balanceOf = BigInteger.Parse(await _wallet.ExecuteSequenceJS(@"
                const provider = seq.getWallet().getProvider();
                const signer = seq.getWallet().getSigner();
                const network = provider.getNetwork();
                const abi =" + abi + @";
                const erc20 = new ethers.Contract('" + address + @"', abi, provider);
                var balanceOf = await erc20.balanceOf(signer.getAddress()||" + account+@");
            "));
            return balanceOf;
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

        public static async Task<bool> TransferFrom(string sender, string recipient, string amount)
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

}