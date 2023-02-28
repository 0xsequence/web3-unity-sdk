
using UnityEngine;
using SequenceSharp.RPC;
using System.Numerics;
using Org.BouncyCastle.Crypto;

public class ClientTest : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        Debug.Log("start");
        var valueBytes = ((BigInteger)10).ToByteArray();

        Debug.Log("testbyte length:" + valueBytes.Length);
        foreach (var b in valueBytes)
        {
            Debug.Log("byte :" + b);
        }
        

        /*HttpRpcClient client = new HttpRpcClient("http://localhost:9090/");
        RpcRequest request = new RpcRequest(1, "sum",new object[] { 1, 2, 3 });
         RpcResponse result = await client.SendRequest(request);
        Debug.Log("result : " + result.result);*/
    }


}
