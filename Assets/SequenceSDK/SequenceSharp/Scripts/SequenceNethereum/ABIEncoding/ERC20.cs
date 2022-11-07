using System.Numerics;
using System.Threading.Tasks;
using System;
using UnityEngine;
namespace SequenceSharp
{
    public class ERC20 : MonoBehaviour
    {
        private static string abi = "[ { \"inputs\": [ { \"internalType\": \"string\", \"name\": \"name_\", \"type\": \"string\" }, { \"internalType\": \"string\", \"name\": \"symbol_\", \"type\": \"string\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"constructor\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"value\", \"type\": \"uint256\" } ], \"name\": \"Approval\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"from\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"value\", \"type\": \"uint256\" } ], \"name\": \"Transfer\", \"type\": \"event\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" } ], \"name\": \"allowance\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"approve\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"account\", \"type\": \"address\" } ], \"name\": \"balanceOf\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"decimals\", \"outputs\": [ { \"internalType\": \"uint8\", \"name\": \"\", \"type\": \"uint8\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"subtractedValue\", \"type\": \"uint256\" } ], \"name\": \"decreaseAllowance\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"addedValue\", \"type\": \"uint256\" } ], \"name\": \"increaseAllowance\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"name\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"symbol\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"totalSupply\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"recipient\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"transfer\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"sender\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"recipient\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"transferFrom\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" } ]";

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
                console.log('provider: ',provider);
                const erc20 = new ethers.Contract(" + address+@", "+abi+@", provider);
                console.log('erc20 : ',erc20);
                var name = await erc20.name();
                console.log('js name:',name);
                return name;

            ");

            return name;
        }

        public static async Task<string> Symbol(string address, string abi)
        {
            //throw new NotImplementedException();
            string symbol = await _wallet.ExecuteSequenceJS(@"
                const provider = seq.getWallet().getProvider();
                const erc20 = new ethers.Contract(" + address + @", " + abi + @", provider);
                var symbol = await erc20.symbol();
                return symbol;
            ");
            return symbol;
        }

        public static async Task<BigInteger> Decimals(string address, string abi)
        {
            //throw new NotImplementedException();
            var decimals = BigInteger.Parse(await _wallet.ExecuteSequenceJS(@"
                const provider = seq.getWallet().getProvider();
                const erc20 = new ethers.Contract(" + address + @", " + abi + @", provider);
                var decimals = await erc20.decimals();
                return decimals;
            "));
            return decimals;
        }
        public static async Task<BigInteger> TotalSupply(string address, string abi)
        {
            //throw new NotImplementedException();
            var totalSupply = BigInteger.Parse(await _wallet.ExecuteSequenceJS(@"
                const provider = seq.getWallet().getProvider();
                const erc20 = new ethers.Contract(" + address + @", " + abi + @", provider);
                var totalSupply = await erc20.totalSupply();
                return totalSupply;
            "));
            return totalSupply;
        }

        public static async Task<BigInteger> BalanceOf(string account, string address, string abi)
        {
            //throw new NotImplementedException();
            var balanceOf = BigInteger.Parse(await _wallet.ExecuteSequenceJS(@"
                const provider = seq.getWallet().getProvider();
                const signer = wallet.getSigner();
                const erc20 = new ethers.Contract(" + address + @", " + abi + @", provider);
                var balanceOf = await erc20.balanceOf(signer.getAddress()||"+account+@");
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