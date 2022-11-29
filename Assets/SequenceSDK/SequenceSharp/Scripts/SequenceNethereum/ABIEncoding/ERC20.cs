using System.Numerics;
using System.Threading.Tasks;
using System;
using UnityEngine;
using Nethereum.Web3;


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
      
      private readonly static string abi = "[ { \"inputs\": [ { \"internalType\": \"string\", \"name\": \"name_\", \"type\": \"string\" }, { \"internalType\": \"string\", \"name\": \"symbol_\", \"type\": \"string\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"constructor\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"value\", \"type\": \"uint256\" } ], \"name\": \"Approval\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"from\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"value\", \"type\": \"uint256\" } ], \"name\": \"Transfer\", \"type\": \"event\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" } ], \"name\": \"allowance\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"approve\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"account\", \"type\": \"address\" } ], \"name\": \"balanceOf\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"decimals\", \"outputs\": [ { \"internalType\": \"uint8\", \"name\": \"\", \"type\": \"uint8\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"subtractedValue\", \"type\": \"uint256\" } ], \"name\": \"decreaseAllowance\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"addedValue\", \"type\": \"uint256\" } ], \"name\": \"increaseAllowance\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"name\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"symbol\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"totalSupply\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"recipient\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"transfer\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"sender\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"recipient\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"transferFrom\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" } ]";

        private static Web3 _web3 = null;

        public static void SetWeb3(Web3 web3)
        {
            _web3 = web3;
        }

        /// <summary>
        /// Returns the name of the token
        /// </summary>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public static async Task<string> Name(string address)
        {
            try
            {
                var contract = _web3.Eth.GetContract(abi, address);
                var nameFunction = contract.GetFunction("name");
                var name = await nameFunction.CallAsync<string>();


                return name;
            }catch(Exception e)
            {
                Debug.Log(e);
                return "";
            }
            
        }

        /// <summary>
        /// Returns the symbol of the token, usually a shorter version of the name
        /// </summary>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public static async Task<string> Symbol(string address)
        {
            try
            {
                var contract = _web3.Eth.GetContract(abi, address);
                var symbolFunction = contract.GetFunction("symbol");
                var symbol = await symbolFunction.CallAsync<string>();


                return symbol;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return "";
            }
        }


        /// <summary>
        /// Returns the number of decimals used to get its user representation. 
        /// Tokens usually opt for a value of 18, imitating the relationship between Ether and Wei. 
        /// </summary>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public static async Task<BigInteger> Decimals(string address)
        {
            try
            {
                var contract = _web3.Eth.GetContract(abi, address);
                var decimalsFunction = contract.GetFunction("decimals");
                var decimals = await decimalsFunction.CallAsync<BigInteger>();

                return decimals;
            }
            catch (Exception e)
            {
                Debug.Log(e);

                return -1;
            }
        }

        /// <summary>
        /// Returns the amount of tokens in existence.
        /// </summary>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public static async Task<BigInteger> TotalSupply(string address)
        {
            try
            {
                var contract = _web3.Eth.GetContract(abi, address);
                var totalSupplyFunction = contract.GetFunction("totalSupply");
                var totalSupply = await totalSupplyFunction.CallAsync<BigInteger>();

                return totalSupply;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return -1;
            }
        }

    

        /// <summary>
        /// Returns the amount of tokens owned by account.
        /// </summary>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// /// <param name="account">Account address, if not provided, it will be the account address from sequence wallet </param>
        /// <returns></returns>
        public static async Task<BigInteger> BalanceOf(string address,  string account)
        {
            try
            {               
                var contract = _web3.Eth.GetContract(abi, address);
                var balanceOfFunction = contract.GetFunction("balanceOf");
                var balanceOf = await balanceOfFunction.CallAsync<BigInteger>(account);

                return balanceOf;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return -1;
            }

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