using System.Numerics;
using System.Threading.Tasks;
using System;
using UnityEngine;
using Newtonsoft.Json;
using Nethereum.Web3;
using Nethereum.JsonRpc.Client.RpcMessages;
using Nethereum.ABI;
using Nethereum.Hex.HexConvertors.Extensions;

namespace SequenceSharp
{

    public struct ERC20Supply
    {
        public string type;
        public string hex;
    }
    public struct ERC20Balance
    {
        public string type;
        public string hex;
    }

    public class ERC20 : MonoBehaviour
    {
      /*  private readonly static string abi = @"[
    // Read-Only Functions
    'function name() view returns (string)',
    'function balanceOf(address owner) view returns (uint256)',
    'function decimals() view returns (uint8)',
    'function symbol() view returns (string)',
    'function totalSupply() view returns (uint256)',

    // Authenticated Functions
    'function transfer(address to, uint amount) returns (bool)',

    
]";*/
      private readonly static string abi = "[ { \"inputs\": [ { \"internalType\": \"string\", \"name\": \"name_\", \"type\": \"string\" }, { \"internalType\": \"string\", \"name\": \"symbol_\", \"type\": \"string\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"constructor\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"value\", \"type\": \"uint256\" } ], \"name\": \"Approval\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"from\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"value\", \"type\": \"uint256\" } ], \"name\": \"Transfer\", \"type\": \"event\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" } ], \"name\": \"allowance\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"approve\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"account\", \"type\": \"address\" } ], \"name\": \"balanceOf\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"decimals\", \"outputs\": [ { \"internalType\": \"uint8\", \"name\": \"\", \"type\": \"uint8\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"subtractedValue\", \"type\": \"uint256\" } ], \"name\": \"decreaseAllowance\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"addedValue\", \"type\": \"uint256\" } ], \"name\": \"increaseAllowance\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"name\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"symbol\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"totalSupply\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"recipient\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"transfer\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"sender\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"recipient\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"transferFrom\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" } ]";
        private static bool usingSequence = false;
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

            
            if (usingSequence)
            {
                string name = await _wallet.ExecuteSequenceJS(@"
                
                const wallet = seq.getWallet();           
                const networks = await wallet.getNetworks();
                const n = networks.find(n => n['chainId']==" + chainId + @");
                const signer = wallet.getSigner(n);
                const abi =" + abi + @";
                const erc20 = new ethers.Contract('" + address + @"', abi, signer);               
                var name = await erc20.name();
                return name;"

                );

                return name;
            }
            else
            {
                var web3 = new Web3();

                web3.Client.OverridingRequestInterceptor = new SequenceInterceptor(_wallet);
                var nameOfFunctionMessage = new NameFunction() { };
                var contract = web3.Eth.GetContract(abi, address);
                var nameFunction = contract.GetFunction("name");
                var name = await nameFunction.CallAsync<string>();
                Debug.Log("result : "+ name);
                
                return name;
            }
        }

        /// <summary>
        /// Returns the symbol of the token, usually a shorter version of the name
        /// </summary>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public static async Task<string> Symbol(string address, int chainId)
        {
            if (usingSequence)
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
            else
            {
                /*var web3 = new Web3();

                web3.Client.OverridingRequestInterceptor = new SequenceInterceptor(_wallet);
                var symbolFunctionMessage = new SymbolFunction() { };
                *//*var abiEncode = new ABIEncode();
                var result = abiEncode.GetSha3ABIParamsEncodedPacked(nameOfFunctionMessage);
                Debug.Log("from abiEncode: " + result.ToHex());*//*
                var symbolHandler = web3.Eth.GetContractQueryHandler<SymbolFunction>();
                Debug.Log(symbolHandler.ToString());
                var symbol_ = await symbolHandler.QueryAsync<string>(address, symbolFunctionMessage);
                Debug.Log(symbol_);
                //return name_;
                return null;*/
                return null;
            }
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
            var totalSupply = await _wallet.ExecuteSequenceJS(@"
                const wallet = seq.getWallet();           
                const networks = await wallet.getNetworks();
                const n = networks.find(n => n['chainId']==" + chainId + @");
                const signer = wallet.getSigner(n);
                const abi =" + abi + @";
                const erc20 = new ethers.Contract('" + address + @"', abi, signer);  

                var totalSupply = await erc20.totalSupply();
                console.log(totalSupply);
                return totalSupply;
            ");
            ERC20Supply totalSupplyParsed = JsonConvert.DeserializeObject<ERC20Supply>(totalSupply);
            return BigInteger.Parse(totalSupplyParsed.hex.Substring(2), System.Globalization.NumberStyles.HexNumber); ;
        }

        /// <summary>
        /// Returns the amount of tokens owned by account.
        /// </summary>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// /// <param name="account">Account address, if not provided, it will be the account address from sequence wallet </param>
        /// <returns></returns>
        public static async Task<BigInteger> BalanceOf(string address, int chainId, string account = null)
        {
            if(account == null)
            {
                //account address not provided

                account = await _wallet.GetAddress();
            }
            //throw new NotImplementedException();
            var balanceOf = await _wallet.ExecuteSequenceJS(@"
                const wallet = seq.getWallet();           
                const networks = await wallet.getNetworks();
                const n = networks.find(n => n['chainId']==" + chainId + @");
                const signer = wallet.getSigner(n);
                const abi =" + abi + @";
                const erc20 = new ethers.Contract('" + address + @"', abi, signer);  

                var balanceOf = await erc20.balanceOf('" + account+@"');
                console.log(balanceOf);
                return balanceOf;
            ");
            ERC20Balance balanceOfParsed = JsonConvert.DeserializeObject<ERC20Balance>(balanceOf);

            return BigInteger.Parse(balanceOfParsed.hex.Substring(2), System.Globalization.NumberStyles.HexNumber);
            
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