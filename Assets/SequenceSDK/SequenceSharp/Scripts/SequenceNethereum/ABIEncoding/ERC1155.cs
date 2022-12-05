using System.Numerics;
using System.Threading.Tasks;
using System;
using UnityEngine;
using System.Collections.Generic;
using Nethereum.Web3;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace SequenceSharp
{
    public struct ERC1155Balance
    {
        public string type;
        public string hex;
    }

    public class ERC1155
    {
        private Web3 _web3 = null;
        private string _contractAddress = "";
        private Nethereum.Contracts.Contract _contract;

        private TransactionReceipt receipt;

        public ERC1155(Web3 web3, string contractAddress)
        {
            _web3 = web3;
            _contractAddress = contractAddress;
            _contract = _web3.Eth.GetContract(abi, _contractAddress);
        }

        /// <summary>
        /// Returns the URI for token type id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public Task<string> URI(BigInteger id)
        {
            return _contract.GetFunction("uri").CallAsync<string>(id);
        }

        /// <summary>
        /// Returns the amount of tokens of token type id owned by account
        /// </summary>
        /// <param name="account"> cannot be zero address </param>
        /// <param name="id"></param>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public Task<BigInteger> BalanceOf(BigInteger id, string account)
        {
            return _contract.GetFunction("balanceOf").CallAsync<BigInteger>(account, id);
        }

        /// <summary>
        /// Batched version of balanceOf, accounts and ids must have same length
        /// </summary>
        /// <param name="accounts"> must have same length as ids</param>
        /// <param name="ids">must have same length as accounts</param>
        /// <param name="address">Contract address</param>
        /// <param name="chainId"></param>
        /// <returns></returns>
        public Task<List<BigInteger>> BalanceOfBatch(List<string> accounts, List<BigInteger> ids)
        {
            return _contract
                .GetFunction("balanceOfBatch")
                .CallAsync<List<BigInteger>>(accounts, ids);
        }

        public async Task SetApprovalForAll(string operatorAddress, bool approved)
        {
            var address = await this._web3.GetAddress();
            var receipt = await _contract
                .GetFunction("setApprovalForAll")
                .SendTransactionAndWaitForReceiptAsync(
                    address,
                    new HexBigInteger(BigInteger.Zero),
                    new HexBigInteger(BigInteger.Zero),
                    null,
                    operatorAddress,
                    approved
                );
            Debug.Log("[Sequence] receipt form function SetApprovalForAll: " + receipt);
            // TODO output
        }

        public async Task<bool> IsApprovedForAll(string account, string operatorAddress)
        {
            var address = await this._web3.GetAddress();
            var receipt = await _contract
                .GetFunction("isApprovedForAll")
                .SendTransactionAndWaitForReceiptAsync(
                    address,
                    new HexBigInteger(BigInteger.Zero),
                    new HexBigInteger(BigInteger.Zero),
                    null,
                    account,
                    operatorAddress
                )
                .ConfigureAwait(false);
            Debug.Log("[Sequence] receipt form function IsApprovedForAll: " + receipt.BlockHash);
            return false;
            // TODO output
        }

        public async Task SafeTransferFrom(
            string from,
            string to,
            BigInteger id,
            BigInteger amount,
            Byte[] data
        )
        {
            var address = await this._web3.GetAddress();
            var receipt = await _contract
                .GetFunction("safeTransferFrom")
                .SendTransactionAndWaitForReceiptAsync(
                    address,
                    new HexBigInteger(BigInteger.Zero),
                    new HexBigInteger(BigInteger.Zero),
                    null,
                    from,
                    to,
                    id,
                    amount,
                    data
                );
            Debug.Log("[Sequence] receipt form function SafeTransferFrom: " + receipt);
            // TODO output
        }

        public async Task SafeBatchTransferFrom(
            string from,
            string to,
            List<BigInteger> ids,
            List<BigInteger> amounts,
            Byte[] data
        )
        {
            var address = await this._web3.GetAddress();
            var receipt = await _contract
                .GetFunction("safeBatchTransferFrom")
                .SendTransactionAndWaitForReceiptAsync(
                    address,
                    new HexBigInteger(BigInteger.Zero),
                    new HexBigInteger(BigInteger.Zero),
                    null,
                    from,
                    to,
                    ids,
                    amounts,
                    data
                );
            Debug.Log("[Sequence] receipt form function SafeBatchTransferFrom: " + receipt);
            // TODO output
        }

        private static readonly string abi =
            @"
            [
                {
                    ""anonymous"": false,
                    ""inputs"": [
                    {
                        ""indexed"": true,
                        ""internalType"": ""address"",
                        ""name"": ""account"",
                        ""type"": ""address""
                    },
                    {
                        ""indexed"": true,
                        ""internalType"": ""address"",
                        ""name"": ""operator"",
                        ""type"": ""address""
                    },
                    {
                        ""indexed"": false,
                        ""internalType"": ""bool"",
                        ""name"": ""approved"",
                        ""type"": ""bool""
                    }
                    ],
                    ""name"": ""ApprovalForAll"",
                    ""type"": ""event""
                },
                {
                    ""anonymous"": false,
                    ""inputs"": [
                    {
                        ""indexed"": true,
                        ""internalType"": ""address"",
                        ""name"": ""operator"",
                        ""type"": ""address""
                    },
                    {
                        ""indexed"": true,
                        ""internalType"": ""address"",
                        ""name"": ""from"",
                        ""type"": ""address""
                    },
                    {
                        ""indexed"": true,
                        ""internalType"": ""address"",
                        ""name"": ""to"",
                        ""type"": ""address""
                    },
                    {
                        ""indexed"": false,
                        ""internalType"": ""uint256[]"",
                        ""name"": ""ids"",
                        ""type"": ""uint256[]""
                    },
                    {
                        ""indexed"": false,
                        ""internalType"": ""uint256[]"",
                        ""name"": ""values"",
                        ""type"": ""uint256[]""
                    }
                    ],
                    ""name"": ""TransferBatch"",
                    ""type"": ""event""
                },
                {
                    ""anonymous"": false,
                    ""inputs"": [
                    {
                        ""indexed"": true,
                        ""internalType"": ""address"",
                        ""name"": ""operator"",
                        ""type"": ""address""
                    },
                    {
                        ""indexed"": true,
                        ""internalType"": ""address"",
                        ""name"": ""from"",
                        ""type"": ""address""
                    },
                    {
                        ""indexed"": true,
                        ""internalType"": ""address"",
                        ""name"": ""to"",
                        ""type"": ""address""
                    },
                    {
                        ""indexed"": false,
                        ""internalType"": ""uint256"",
                        ""name"": ""id"",
                        ""type"": ""uint256""
                    },
                    {
                        ""indexed"": false,
                        ""internalType"": ""uint256"",
                        ""name"": ""value"",
                        ""type"": ""uint256""
                    }
                    ],
                    ""name"": ""TransferSingle"",
                    ""type"": ""event""
                },
                {
                    ""anonymous"": false,
                    ""inputs"": [
                    {
                        ""indexed"": false,
                        ""internalType"": ""string"",
                        ""name"": ""value"",
                        ""type"": ""string""
                    },
                    {
                        ""indexed"": true,
                        ""internalType"": ""uint256"",
                        ""name"": ""id"",
                        ""type"": ""uint256""
                    }
                    ],
                    ""name"": ""URI"",
                    ""type"": ""event""
                },
                {
                    ""inputs"": [
                    {
                        ""internalType"": ""address"",
                        ""name"": ""account"",
                        ""type"": ""address""
                    },
                    {
                        ""internalType"": ""uint256"",
                        ""name"": ""id"",
                        ""type"": ""uint256""
                    }
                    ],
                    ""name"": ""balanceOf"",
                    ""outputs"": [
                    {
                        ""internalType"": ""uint256"",
                        ""name"": """",
                        ""type"": ""uint256""
                    }
                    ],
                    ""stateMutability"": ""view"",
                    ""type"": ""function""
                },
                {
                    ""inputs"": [
                    {
                        ""internalType"": ""address[]"",
                        ""name"": ""accounts"",
                        ""type"": ""address[]""
                    },
                    {
                        ""internalType"": ""uint256[]"",
                        ""name"": ""ids"",
                        ""type"": ""uint256[]""
                    }
                    ],
                    ""name"": ""balanceOfBatch"",
                    ""outputs"": [
                    {
                        ""internalType"": ""uint256[]"",
                        ""name"": """",
                        ""type"": ""uint256[]""
                    }
                    ],
                    ""stateMutability"": ""view"",
                    ""type"": ""function""
                },
                {
                    ""inputs"": [
                    {
                        ""internalType"": ""address"",
                        ""name"": ""account"",
                        ""type"": ""address""
                    },
                    {
                        ""internalType"": ""address"",
                        ""name"": ""operator"",
                        ""type"": ""address""
                    }
                    ],
                    ""name"": ""isApprovedForAll"",
                    ""outputs"": [
                    {
                        ""internalType"": ""bool"",
                        ""name"": """",
                        ""type"": ""bool""
                    }
                    ],
                    ""stateMutability"": ""view"",
                    ""type"": ""function""
                },
                {
                    ""inputs"": [
                    {
                        ""internalType"": ""address"",
                        ""name"": ""from"",
                        ""type"": ""address""
                    },
                    {
                        ""internalType"": ""address"",
                        ""name"": ""to"",
                        ""type"": ""address""
                    },
                    {
                        ""internalType"": ""uint256[]"",
                        ""name"": ""ids"",
                        ""type"": ""uint256[]""
                    },
                    {
                        ""internalType"": ""uint256[]"",
                        ""name"": ""amounts"",
                        ""type"": ""uint256[]""
                    },
                    {
                        ""internalType"": ""bytes"",
                        ""name"": ""data"",
                        ""type"": ""bytes""
                    }
                    ],
                    ""name"": ""safeBatchTransferFrom"",
                    ""outputs"": [],
                    ""stateMutability"": ""nonpayable"",
                    ""type"": ""function""
                },
                {
                    ""inputs"": [
                    {
                        ""internalType"": ""address"",
                        ""name"": ""from"",
                        ""type"": ""address""
                    },
                    {
                        ""internalType"": ""address"",
                        ""name"": ""to"",
                        ""type"": ""address""
                    },
                    {
                        ""internalType"": ""uint256"",
                        ""name"": ""id"",
                        ""type"": ""uint256""
                    },
                    {
                        ""internalType"": ""uint256"",
                        ""name"": ""amount"",
                        ""type"": ""uint256""
                    },
                    {
                        ""internalType"": ""bytes"",
                        ""name"": ""data"",
                        ""type"": ""bytes""
                    }
                    ],
                    ""name"": ""safeTransferFrom"",
                    ""outputs"": [],
                    ""stateMutability"": ""nonpayable"",
                    ""type"": ""function""
                },
                {
                    ""inputs"": [
                    {
                        ""internalType"": ""address"",
                        ""name"": ""operator"",
                        ""type"": ""address""
                    },
                    {
                        ""internalType"": ""bool"",
                        ""name"": ""approved"",
                        ""type"": ""bool""
                    }
                    ],
                    ""name"": ""setApprovalForAll"",
                    ""outputs"": [],
                    ""stateMutability"": ""nonpayable"",
                    ""type"": ""function""
                },
                {
                    ""inputs"": [
                    {
                        ""internalType"": ""bytes4"",
                        ""name"": ""interfaceId"",
                        ""type"": ""bytes4""
                    }
                    ],
                    ""name"": ""supportsInterface"",
                    ""outputs"": [
                    {
                        ""internalType"": ""bool"",
                        ""name"": """",
                        ""type"": ""bool""
                    }
                    ],
                    ""stateMutability"": ""view"",
                    ""type"": ""function""
                },
                {
                    ""inputs"": [
                    {
                        ""internalType"": ""uint256"",
                        ""name"": ""id"",
                        ""type"": ""uint256""
                    }
                    ],
                    ""name"": ""uri"",
                    ""outputs"": [
                    {
                        ""internalType"": ""string"",
                        ""name"": """",
                        ""type"": ""string""
                    }
                    ],
                    ""stateMutability"": ""view"",
                    ""type"": ""function""
                }
            ]";
    }
}
