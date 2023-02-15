using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SequenceSharp
{
    public interface IEthApiContractService
    {
        Contract GetContract(string abi, string contractAddress);
    }
}