using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SequenceSharp
{
    public class EthApiService : IEthApiService
    {
        public EthApiService(IClient client)
        {

        }
        public EthApiService(IClient client, ITransactionManager transactionManager)
        {

        }


        public IEthApiTransactionsService Transactions { get; private set; }

        public ITransactionManager TransactionManager { get; private set; }

        public IEthSign Sign { get; private set; }

        public IEthGetBalance GetBalance { get; private set; }

        public IEthAccounts Accounts { get; private set; }

        public IEthChainId ChainId { get; private set; }
    }
}
