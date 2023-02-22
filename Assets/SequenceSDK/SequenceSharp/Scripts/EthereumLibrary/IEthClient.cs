using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

namespace SequenceSharp
{
	// Standard Ethereum JSON-RPC methods:
	// https://ethereum.org/en/developers/docs/apis/json-rpc/
	//
	// web3_clientVersion
	// web3_sha3
	// net_version
	// net_listening
	// net_peerCount
	// eth_protocolVersion
	// eth_syncing
	// eth_coinbase
	// eth_mining
	// eth_hashrate
	// eth_gasPrice
	// eth_accounts
	// eth_blockNumber
	// eth_getBalance
	// eth_getStorageAt
	// eth_getTransactionCount
	// eth_getBlockTransactionCountByHash
	// eth_getBlockTransactionCountByNumber
	// eth_getUncleCountByBlockHash
	// eth_getUncleCountByBlockNumber
	// eth_getCode
	// eth_sign
	// eth_signTransaction
	// eth_sendTransaction
	// eth_sendRawTransaction
	// eth_call
	// eth_estimateGas
	// eth_getBlockByHash
	// eth_getBlockByNumber
	// eth_getTransactionByHash
	// eth_getTransactionByBlockHashAndIndex
	// eth_getTransactionByBlockNumberAndIndex
	// eth_getTransactionReceipt
	// eth_getUncleByBlockHashAndIndex
	// eth_getUncleByBlockNumberAndIndex
	// eth_getCompilers
	// eth_compileSolidity
	// eth_compileLLL
	// eth_compileSerpent
	// eth_newFilter
	// eth_newBlockFilter
	// eth_newPendingTransactionFilter
	// eth_uninstallFilter
	// eth_getFilterChanges
	// eth_getFilterLogs
	// eth_getLogs
	// eth_getWork
	// eth_submitWork
	// eth_submitHashrate
	//
	//
	// Unstandard JSON-RPC Methods:
	//
	// eth_getBlockRange -- Optimism
	public interface IEthClient
	{

		// ChainID = eth_chainId
		string ChainID();

		// BlockByHash = eth_getBlockByHash (true)
		Block BlockByHash(string blockHash);

		// BlockByNumber = eth_getBlockByNumber (true)
		Block BlockByNumber(string blockNumber);

		// BlockNumber = eth_blockNumber
		string BlockNumber();

		// BlockRange = eth_getBlockRange
		// https://community.optimism.io/docs/developers/build/json-rpc/#eth-getblockrange
		List<Block> BlockRange(string start = "earliest", string end = "earliest", bool? full= true );

		// PeerCount = net_peerCount
		BigInteger PeerCount();

		// HeaderByHash = eth_getBlockByHash (false)
		void HeaderByHash(string blockHash);

		// HeaderByNumber = eth_getBlockByHash (true)
		void HeaderByNumber(string blockNumber);

		// TransactionByHash = eth_getTransactionByHash
		Transaction TransactionByHash(string transactionHash);

		// TransactionSender is a wrapper for eth_getTransactionByBlockHashAndIndex
		Transaction TransactionSender(string blockHash, BigInteger transactionIndex);

		// TransactionCount = eth_getBlockTransactionCountByHash
		BigInteger TransactionCount(string blockHash);

		// TransactionInBlock = eth_getTransactionByBlockHashAndIndex
		Transaction TransactionInBlock(string blockHash, BigInteger transactionIndex);
		// TransactionReceipt = eth_getTransactionReceipt
		TransactionReceipt TransactionReceipt(string transactionHash);

		// SyncProgress = eth_syncing
		SyncStatus SyncProgress();

		// NetworkID = net_version
		string NetworkID();

		// BalanceAt = eth_getBalance
		BigInteger BalanceAt(string address, string blockNumber);

		// StorageAt = eth_getStorageAt
		string StorageAt(string address, BigInteger storagePosition, string blockNumber);

		// CodeAt = eth_getCode
		string CodeAt(string address, string blockNumber);

		// NonceAt = eth_getTransactionCount
		BigInteger NonceAt(string address, string blockNumber);

		// FilterLogs = eth_getLogs
		List<Log> FilterLogs(Filter filter);

		// PendingBalanceAt = eth_getBalance ("pending")
		BigInteger PendingBalanceAt(string address);

		// PendingStorageAt = eth_getStorageAt ("pending")
		string PendingStorageAt(string address, BigInteger storagePosition);

		// PendingCodeAt = eth_getCode ("pending")
		string PendingCodeAt(string address);

		// PendingNonceAt = eth_getTransactionCount ("pending")
		BigInteger PendingNonceAt(string address);

		// PendingTransactionCount = eth_getBlockTransactionCountByNumber ("pending")
		BigInteger PendingTransactionCount();

		// CallContract = eth_call (blockNumber)
		//TODO: ?
		string CallContract();

		// CallContractAtHash = eth_call (blockHash)
		//TODO: ?
		string CallContractAtHash();

		// PendingCallContract = eth_call ("pending")
		string PendingCallContract(TransactionCall transactionCall);

		// SuggestGasPrice = eth_gasPrice
		BigInteger SuggestGasPrice();

		// SuggestGasTipCap = eth_maxPriorityFeePerGas
		string SuggestGasTipCap();

		// FeeHistory = eth_feeHistory
		FeeHistoryResult FeeHistory(string blockCount, string newestBlock, int? REWARDPERCENTILES);
		// EstimateGas = eth_estimateGas
		BigInteger EstimateGas(TransactionCall transactionCall, string blockNumber);

		// SendTransaction = eth_sendRawTransaction
		//return the transaction hash
		string SendTransaction(Transaction transaction);

		// SendRawTransaction = eth_sendRawTransaction
		string SendRawTransaction(string signedTransactionData);
	}
}