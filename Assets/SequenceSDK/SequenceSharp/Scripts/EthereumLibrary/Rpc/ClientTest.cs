using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("start");
        SequenceSharp.RPC.HttpRpcClient client = new SequenceSharp.RPC.HttpRpcClient("http://localhost:9090/");
        SequenceSharp.RPC.RpcRequest rquest = new SequenceSharp.RPC.RpcRequest(1, "sum",new object[] { 1, 2, 3 });
        StartCoroutine(client.SendRequest(rquest));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
