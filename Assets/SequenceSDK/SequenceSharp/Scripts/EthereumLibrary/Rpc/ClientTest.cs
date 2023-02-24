
using UnityEngine;
using SequenceSharp.RPC;
public class ClientTest : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        Debug.Log("start");
        HttpRpcClient client = new HttpRpcClient("http://localhost:9090/");
        RpcRequest request = new RpcRequest(1, "sum",new object[] { 1, 2, 3 });
         RpcResponse result = await client.SendRequest(request);
        Debug.Log("result : " + result.result);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
