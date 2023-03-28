using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using SequenceSharp.RPC;

namespace SequenceSharp.WALLET
{
    public class SequenceWallet : BaseWallet
    {
        Provider provider;
        WalletProvider wallet;
       public Task Encrypt()
        {
            throw new System.NotImplementedException();
        }

        public void EncryptSync()
        {
            throw new System.NotImplementedException();
        }

        public static Task<Wallet> FromEncryptedJson()
        {
            throw new System.NotImplementedException();
        }

        public static Wallet FromEncryptedJsonSync()
        {
            throw new System.NotImplementedException();
        }
/*
        public static HDWallet CreatRandom()
        {
            throw new System.NotImplementedException();
        }

        public static HDWallet FromPrase()
        {
            throw new System.NotImplementedException();
        }
*/

        public void Transactor()
        {
            throw new System.NotImplementedException();
        }

        public void TransactorForChainID()
        {
            throw new System.NotImplementedException();
        }

        public void GetProvider()
        {
            throw new System.NotImplementedException();
        }

        public void SetProvider()
        {
            throw new System.NotImplementedException();
        }                                                                                                                                        
        public void Provider()
        {
            throw new System.NotImplementedException();
        }

        public BigInteger GetBalance()
        {
            throw new System.NotImplementedException();
        }

        public BigInteger GetNonce()
        {
            throw new System.NotImplementedException();
        }

        public void SignTx()
        {
            throw new System.NotImplementedException();
        }

        public bool IsValidSignature()
        {
            throw new System.NotImplementedException();
        }
        public override void SignMessage()
        {
            throw new System.NotImplementedException();
        }

        public override void SendTransaction()
        {
            throw new System.NotImplementedException();
        }

        public override void SignTypedData()
        {
            throw new System.NotImplementedException();
        }
    }
}