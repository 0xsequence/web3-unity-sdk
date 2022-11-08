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

        /// <summary>
        /// Returns the name of the token
        /// </summary>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public static async Task<string> Name(string address, int chainId)
        {
            //throw new NotImplementedException();
            
            string name = await _wallet.ExecuteSequenceJS(@"
                
                const wallet = seq.getWallet();           
                const networks = await wallet.getNetworks();
                const n = networks.find(n => n['chainId']=="+chainId+@");
                const signer = wallet.getSigner(n);
                const abi =" + abi + @";
                const erc20 = new ethers.Contract('" + address+ @"', abi, signer);               
                var name = await erc20.name();
                return name;"

            );

            return name;
        }

        /// <summary>
        /// Returns the symbol of the token, usually a shorter version of the name
        /// </summary>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public static async Task<string> Symbol(string address, int chainId)
        {
            //throw new NotImplementedException();
            string symbol = await _wallet.ExecuteSequenceJS(@"
                const wallet = seq.getWallet();           
                const networks = await wallet.getNetworks();
                const n = networks.find(n => n['chainId']==" + chainId + @");
                const signer = wallet.getSigner(n);
                const abi =" + abi + @";
                const erc20 = new ethers.Contract('" + address + @"', abi, signer);  

                var symbol = await erc20.symbol();
                return symbol;
            ");
            return symbol;
        }


        /// <summary>
        /// Returns the number of decimals used to get its user representation. 
        /// Tokens usually opt for a value of 18, imitating the relationship between Ether and Wei. 
        /// </summary>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public static async Task<BigInteger> Decimals(string address, int chainId)
        {
            //throw new NotImplementedException();
            var decimals = BigInteger.Parse(await _wallet.ExecuteSequenceJS(@"
                const wallet = seq.getWallet();           
                const networks = await wallet.getNetworks();
                const n = networks.find(n => n['chainId']==" + chainId + @");
                const signer = wallet.getSigner(n);
                const abi =" + abi + @";
                const erc20 = new ethers.Contract('" + address + @"', abi, signer);  

                var decimals = await erc20.decimals();
                return decimals;
            "));
            return decimals;
        }

        /// <summary>
        /// Returns the amount of tokens in existence.
        /// </summary>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public static async Task<BigInteger> TotalSupply(string address, int chainId)
        {
            //throw new NotImplementedException();
            var totalSupply = BigInteger.Parse(await _wallet.ExecuteSequenceJS(@"
                const wallet = seq.getWallet();           
                const networks = await wallet.getNetworks();
                const n = networks.find(n => n['chainId']==" + chainId + @");
                const signer = wallet.getSigner(n);
                const abi =" + abi + @";
                const erc20 = new ethers.Contract('" + address + @"', abi, signer);  

                var totalSupply = await erc20.totalSupply();
                return totalSupply;
            "));
            return totalSupply;
        }

        /// <summary>
        /// Returns the amount of tokens owned by account.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public static async Task<BigInteger> BalanceOf(string account, string address, int chainId)
        {
            //throw new NotImplementedException();
            var balanceOf = BigInteger.Parse(await _wallet.ExecuteSequenceJS(@"
                const wallet = seq.getWallet();           
                const networks = await wallet.getNetworks();
                const n = networks.find(n => n['chainId']==" + chainId + @");
                const signer = wallet.getSigner(n);
                const abi =" + abi + @";
                const erc20 = new ethers.Contract('" + address + @"', abi, signer);  

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