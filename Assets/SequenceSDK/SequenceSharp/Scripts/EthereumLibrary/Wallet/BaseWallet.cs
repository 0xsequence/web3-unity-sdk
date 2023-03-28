using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SequenceSharp.WALLET
{
    public abstract class BaseWallet
    {
        private readonly string address;
        private readonly SigningKey signingKey;

        public SigningKey GetSigningKey()
        {
            return signingKey;
        }
        public string GetAddress()
        {
            return address;
        }
        public abstract void SignMessage();
        public abstract void SendTransaction();

        public abstract void SignTypedData();

    }
}
