using Newtonsoft.Json;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace SequenceSharp.RPC
{

    public class HttpRpcClient
    {
        private readonly string _url;
        public HttpRpcClient(string url)
        {
            _url = url;
            Debug.Log("http rpc client");
        }

        public async Task<RpcResponse> SendRequest(RpcRequest rpcRequest)
        {

            var request = new
            {
                jsonrpc = "2.0",
                id = rpcRequest.id,
                method = rpcRequest.method,
                @params = rpcRequest.rawParameters

            };

            var rpcRequestJson = JsonConvert.SerializeObject(request);
            var unityRequest = UnityWebRequest.Put(_url, rpcRequestJson);
            unityRequest.SetRequestHeader("Content-Type", "application/json");
            unityRequest.SetRequestHeader("Accept", "application/json");
            unityRequest.method = UnityWebRequest.kHttpVerbPOST;
            await unityRequest.SendWebRequest();

            if (unityRequest.error != null)
            {
                Debug.Log("RequestError: " + unityRequest.error);
                unityRequest.Dispose();
                return default;
            }
            else
            {
                byte[] results = unityRequest.downloadHandler.data;
                var responseJson = Encoding.UTF8.GetString(results);
                RpcResponse result = JsonConvert.DeserializeObject<RpcResponse>(responseJson);
                unityRequest.Dispose();
                return result;
            }

        }

        public async Task<RpcResponse> SendRequest(string method, object[] parameters)
        {
            
            var request = new
            {
                jsonrpc = "2.0",
                id = 1,
                method = method,
                @params = parameters
                
            };

            var rpcRequestJson = JsonConvert.SerializeObject(request);

            var unityRequest = UnityWebRequest.Put(_url, rpcRequestJson);

            unityRequest.SetRequestHeader("Content-Type", "application/json");
            unityRequest.SetRequestHeader("Accept", "application/json");
            unityRequest.method = UnityWebRequest.kHttpVerbPOST;
            await unityRequest.SendWebRequest();

            if (unityRequest.error != null)
            {
                Debug.Log("RequestError: " + unityRequest.error);
                unityRequest.Dispose();
                return default;
            }
            else
            {
                byte[] results = unityRequest.downloadHandler.data;
                var responseJson = Encoding.UTF8.GetString(results);
                RpcResponse result = JsonConvert.DeserializeObject<RpcResponse>(responseJson);
                unityRequest.Dispose();
                return result;
            }

            
        }

    }
}


