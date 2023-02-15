using System;


namespace SequenceSharp
{
    public class RpcResponseException : Exception
    {
        public RpcResponseException(RpcError rpcError) { }

        public RpcError RpcError { get; }
    }

}