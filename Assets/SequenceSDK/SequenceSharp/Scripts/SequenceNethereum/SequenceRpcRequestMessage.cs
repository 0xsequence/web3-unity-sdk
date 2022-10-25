using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;

namespace SequenceSharp
{
    public class SequenceRpcRequestMessage : RpcRequestMessage
    {

        public SequenceRpcRequestMessage(object id, string method, string from, params object[] parameterList) : base(id, method,
            parameterList)
        {
            From = from;
        }

        [JsonProperty("from")]
        public string From { get; private set; }
    }
}