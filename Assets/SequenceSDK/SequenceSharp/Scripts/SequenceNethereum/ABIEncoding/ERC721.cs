using System.Numerics;
using System.Threading.Tasks;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace SequenceSharp
{

    public class ERC721
    {
        private Web3 _web3 = null;
        private string _contractAddress = "";
        private Nethereum.Contracts.Contract _contract;

        public ERC721(Web3 web3, string contractAddress)
        {
            _web3 = web3;
            _contractAddress = contractAddress;
            _contract = _web3.Eth.GetContract(abi, _contractAddress);
        }

        /// <summary>
        /// Returns the token collection name.
        /// </summary>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public Task<string> Name()
        {
            return _contract.GetFunction("name").CallAsync<string>();
        }

        /// <summary>
        /// Returns the token collection symbol.
        /// </summary>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public Task<string> Symbol()
        {
            return _contract.GetFunction("symbol").CallAsync<string>();
        }

        /// <summary>
        /// Returns the Uniform Resource Identifier (URI) for tokenId token.
        /// </summary>
        /// <param name="tokenId"></param>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public Task<string> TokenURI(BigInteger tokenId)
        {
            return _contract.GetFunction("tokenURI").CallAsync<string>(tokenId);
        }

        /// <summary>
        /// Returns the number of tokens in owner's account.
        /// </summary>
        /// <param name="owner">Account address, if not provided, it will be the account address from sequence wallet</param>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public Task<BigInteger> BalanceOf(string owner)
        {
            return _contract.GetFunction("balanceOf").CallAsync<BigInteger>(owner);
        }

        /// <summary>
        /// Returns the owner of the tokenId token.
        /// </summary>
        /// <param name="tokenId">tokenId must exist.</param>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public Task<string> OwnerOf(BigInteger tokenId)
        {
            return _contract.GetFunction("ownerOf").CallAsync<string>(tokenId);
        }

        public async Task<TransactionReceipt> SafeTransferFrom(string from, string to, BigInteger tokenId)
        {
            var address = await this._web3.GetAddress();
            return await _contract
                .GetFunction("safeTransferFrom")
                .SendTransactionAndWaitForReceiptAsync(
                    address,
                    new HexBigInteger(BigInteger.Zero),
                    new HexBigInteger(BigInteger.Zero),
                    null,
                    from,
                    to,
                    tokenId
                );
        }

        public async Task<TransactionReceipt> TransferFrom(string from, string to, BigInteger tokenId)
        {
            var address = await this._web3.GetAddress();
            return await _contract
                .GetFunction("transferFrom")
                .SendTransactionAndWaitForReceiptAsync(
                    address,
                    new HexBigInteger(BigInteger.Zero),
                    new HexBigInteger(BigInteger.Zero),
                    null,
                    from,
                    to,
                    tokenId
                );
        }

        public async Task<TransactionReceipt> Approve(string to, BigInteger tokenId)
        {
            var address = await this._web3.GetAddress();
            return await _contract
                .GetFunction("approve")
                .SendTransactionAndWaitForReceiptAsync(
                    address,
                    new HexBigInteger(BigInteger.Zero),
                    new HexBigInteger(BigInteger.Zero),
                    null,
                    to,
                    tokenId
                );
        }

        public Task<string> GetApproved(BigInteger tokenId)
        {
            return _contract
                 .GetFunction("getApproved")
                 .CallAsync<string>(
                     tokenId
                 );
        }

        public async Task<TransactionReceipt> SetApprovalForAll(string operatorAddress, bool approved)
        {
            var address = await this._web3.GetAddress();
            return await _contract
                .GetFunction("setApprovalForAll")
                .SendTransactionAndWaitForReceiptAsync(
                    address,
                    new HexBigInteger(BigInteger.Zero),
                    new HexBigInteger(BigInteger.Zero),
                    null,
                    operatorAddress,
                    approved
                );
        }

        public Task<bool> IsApprovedForAll(string owner, string operatorAddress)
        {
            return _contract
                .GetFunction("isApprovedForAll")
                .CallAsync<bool>(
                    owner,
                    operatorAddress
                );
        }

        private static readonly string abi =
            @"
            [
                {
                    ""anonymous"": false,
                    ""inputs"": [{""indexed"": true, ""internalType"": ""address"", ""name"": ""owner"", ""type"": ""address""}, {
                    ""indexed"": true,
                    ""internalType"": ""address"",
                    ""name"": ""approved"",
                    ""type"": ""address""
                    }, {""indexed"": true, ""internalType"": ""uint256"", ""name"": ""tokenId"", ""type"": ""uint256""}],
                    ""name"": ""Approval"",
                    ""type"": ""event""
                },
                {
                    ""anonymous"": false,
                    ""inputs"": [{""indexed"": true, ""internalType"": ""address"", ""name"": ""owner"", ""type"": ""address""}, {
                    ""indexed"": true,
                    ""internalType"": ""address"",
                    ""name"": ""operator"",
                    ""type"": ""address""
                    }, {""indexed"": false, ""internalType"": ""bool"", ""name"": ""approved"", ""type"": ""bool""}],
                    ""name"": ""ApprovalForAll"",
                    ""type"": ""event""
                },
                {
                    ""anonymous"": false,
                    ""inputs"": [{""indexed"": true, ""internalType"": ""address"", ""name"": ""from"", ""type"": ""address""}, {
                    ""indexed"": true,
                    ""internalType"": ""address"",
                    ""name"": ""to"",
                    ""type"": ""address""
                    }, {""indexed"": true, ""internalType"": ""uint256"", ""name"": ""tokenId"", ""type"": ""uint256""}],
                    ""name"": ""Transfer"",
                    ""type"": ""event""
                },
                {
                    ""inputs"": [{""internalType"": ""address"", ""name"": ""to"", ""type"": ""address""}, {
                    ""internalType"": ""uint256"",
                    ""name"": ""tokenId"",
                    ""type"": ""uint256""
                    }], ""name"": ""approve"", ""outputs"": [], ""stateMutability"": ""nonpayable"", ""type"": ""function""
                },
                {
                    ""constant"": true,
                    ""inputs"": [],
                    ""name"": ""totalSupply"",
                    ""outputs"": [
                    {
                        ""name"": """",
                        ""type"": ""uint256""
                    }
                    ],
                    ""payable"": false,
                    ""stateMutability"": ""view"",
                    ""type"": ""function""
                },
                {
                    ""inputs"": [{""internalType"": ""address"", ""name"": ""owner"", ""type"": ""address""}],
                    ""name"": ""balanceOf"",
                    ""outputs"": [{""internalType"": ""uint256"", ""name"": ""balance"", ""type"": ""uint256""}],
                    ""stateMutability"": ""view"",
                    ""type"": ""function""
                },
                {
                    ""inputs"": [{""internalType"": ""uint256"", ""name"": ""tokenId"", ""type"": ""uint256""}],
                    ""name"": ""getApproved"",
                    ""outputs"": [{""internalType"": ""address"", ""name"": ""operator"", ""type"": ""address""}],
                    ""stateMutability"": ""view"",
                    ""type"": ""function""
                },
                {
                    ""inputs"": [{""internalType"": ""address"", ""name"": ""owner"", ""type"": ""address""}, {
                    ""internalType"": ""address"",
                    ""name"": ""operator"",
                    ""type"": ""address""
                    }],
                    ""name"": ""isApprovedForAll"",
                    ""outputs"": [{""internalType"": ""bool"", ""name"": """", ""type"": ""bool""}],
                    ""stateMutability"": ""view"",
                    ""type"": ""function""
                },
                {
                    ""inputs"": [],
                    ""name"": ""name"",
                    ""outputs"": [{""internalType"": ""string"", ""name"": """", ""type"": ""string""}],
                    ""stateMutability"": ""view"",
                    ""type"": ""function""
                },
                {
                    ""inputs"": [{""internalType"": ""uint256"", ""name"": ""tokenId"", ""type"": ""uint256""}],
                    ""name"": ""ownerOf"",
                    ""outputs"": [{""internalType"": ""address"", ""name"": ""owner"", ""type"": ""address""}],
                    ""stateMutability"": ""view"",
                    ""type"": ""function""
                },
                {
                    ""inputs"": [{""internalType"": ""address"", ""name"": ""from"", ""type"": ""address""}, {
                    ""internalType"": ""address"",
                    ""name"": ""to"",
                    ""type"": ""address""
                    },
                    {""internalType"": ""uint256"", ""name"": ""tokenId"", ""type"": ""uint256""}],
                    ""name"": ""safeTransferFrom"",
                    ""outputs"": [],
                    ""stateMutability"": ""nonpayable"",
                    ""type"": ""function""
                },
                {
                    ""inputs"": [{""internalType"": ""address"", ""name"": ""from"", ""type"": ""address""}, {
                    ""internalType"": ""address"",
                    ""name"": ""to"",
                    ""type"": ""address""
                    },
                    {""internalType"": ""uint256"", ""name"": ""tokenId"", ""type"": ""uint256""}, {
                        ""internalType"": ""bytes"",
                        ""name"": ""data"",
                        ""type"": ""bytes""
                    }], ""name"": ""safeTransferFrom"", ""outputs"": [], ""stateMutability"": ""nonpayable"", ""type"": ""function""
                },
                {
                    ""inputs"": [{""internalType"": ""address"", ""name"": ""operator"", ""type"": ""address""}, {
                    ""internalType"": ""bool"",
                    ""name"": ""_approved"",
                    ""type"": ""bool""
                    }], ""name"": ""setApprovalForAll"", ""outputs"": [], ""stateMutability"": ""nonpayable"", ""type"": ""function""
                },
                {
                    ""inputs"": [{""internalType"": ""bytes4"", ""name"": ""interfaceId"", ""type"": ""bytes4""}],
                    ""name"": ""supportsInterface"",
                    ""outputs"": [{""internalType"": ""bool"", ""name"": """", ""type"": ""bool""}],
                    ""stateMutability"": ""view"",
                    ""type"": ""function""
                },
                {
                    ""inputs"": [],
                    ""name"": ""symbol"",
                    ""outputs"": [{""internalType"": ""string"", ""name"": """", ""type"": ""string""}],
                    ""stateMutability"": ""view"",
                    ""type"": ""function""
                },
                {
                    ""inputs"": [{""internalType"": ""uint256"", ""name"": ""tokenId"", ""type"": ""uint256""}],
                    ""name"": ""tokenURI"",
                    ""outputs"": [{""internalType"": ""string"", ""name"": """", ""type"": ""string""}],
                    ""stateMutability"": ""view"",
                    ""type"": ""function""
                },
                {
                    ""inputs"": [{""internalType"": ""address"", ""name"": ""from"", ""type"": ""address""}, {
                    ""internalType"": ""address"",
                    ""name"": ""to"",
                    ""type"": ""address""
                    }, {""internalType"": ""uint256"", ""name"": ""tokenId"", ""type"": ""uint256""}],
                    ""name"": ""transferFrom"",
                    ""outputs"": [],
                    ""stateMutability"": ""nonpayable"",
                    ""type"": ""function""
                }
            ]
        ";
    }
}
