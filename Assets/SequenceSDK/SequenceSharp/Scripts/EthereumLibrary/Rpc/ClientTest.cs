
using UnityEngine;
using SequenceSharp.RPC;
using SequenceSharp.ABI;
using System.Numerics;
using Org.BouncyCastle.Crypto;
using System.Globalization;
using System.Text;

public class ClientTest : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {


        string address = "0x8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9";
        if (address.StartsWith("0x"))
        {

            address.Remove(0, 2);
        }
        Debug.Log("address" + address);
        BigInteger number = BigInteger.Parse("8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9", NumberStyles.AllowHexSpecifier);//new BigInteger(-2324254325223);
        var hex = "8e3E38fe7367dd3b52D1e281E4e8400447C8d8B9";// number.ToString("x");
        string encodedString = new string('0', 64 - hex.Length) + hex;
        Debug.Log(encodedString);

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
