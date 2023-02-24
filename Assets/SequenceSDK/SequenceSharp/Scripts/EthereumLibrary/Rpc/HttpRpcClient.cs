using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
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

        public IEnumerator SendRequest(string method, object[] parameters)
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
            yield return unityRequest.SendWebRequest();

            if (unityRequest.error != null)
            {
                Debug.Log("RequestError: " + unityRequest.error);
            }
            else
            {
                byte[] results = unityRequest.downloadHandler.data;
                var responseJson = Encoding.UTF8.GetString(results);
                Debug.Log("Response: " + responseJson);
            }

            unityRequest.Dispose();
        }

        public IEnumerator SendRequest(RpcRequest rpcRequest)
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
            yield return unityRequest.SendWebRequest();

            if (unityRequest.error != null)
            {
                Debug.Log("RequestError: " + unityRequest.error);
            }
            else
            {
                byte[] results = unityRequest.downloadHandler.data;
                var responseJson = Encoding.UTF8.GetString(results);
                Debug.Log("Response: " + responseJson);
            }

            unityRequest.Dispose();
        }
    }
}


