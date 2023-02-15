using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SequenceSharp
{
    public interface IEthApiService
    {
        IEthApiTransactionsService Transactions { get; }
        ITransactionManager TransactionManager { get; set; }
        IEthSign Sign { get; }
        IEthGetBalance GetBalance { get; }
        IEthAccounts Accounts { get; }
        IEthChainId ChainId { get; }
    }

    public interface IEthApiTransactionsService
    {
        IEthSendTransaction SendTransaction { get; }
    }
    public interface ITransactionManager { }
    public interface IEthSign { }
    public interface IEthGetBalance { }
    public interface IEthAccounts { }
    public interface IEthChainId { }
}
