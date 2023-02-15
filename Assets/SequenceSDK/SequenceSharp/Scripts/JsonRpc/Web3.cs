using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SequenceSharp
{
    public class Web3
    {
        public Web3(IClient client) { }
        public IClient Client { get; }

        public IEthApiContractService Eth { get; }
    }
}
