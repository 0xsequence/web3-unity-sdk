using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SequenceSharp
{
    public class JsonRpcProvider
    {
        private readonly HttpClient _client;
        private readonly string _endpoint;

        public JsonRpcProvider(string endpoint)
        {
            _client = new HttpClient();
            _endpoint = endpoint;
        }

        public JsonRpcProvider(string url, string endpoint)
        {
            _client = new HttpClient();
            _endpoint = endpoint;
        }

        public async Task<T> Send<T>(RpcRequest request)
        {
            var jsonRequest = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(_endpoint, content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            var rpcResponse = JsonConvert.DeserializeObject<RpcResponse<T>>(jsonResponse);

            if (rpcResponse.Error != null)
            {
                throw new Exception(rpcResponse.Error.Message);
            }

            return rpcResponse.Result;
        }
        public async Task<T> Send<T>(string method, object[] parameters)
        {
            var request = new
            {
                jsonrpc = "2.0",
                method = method,
                @params = parameters,
                id = 1
            };

            var jsonRequest = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(_endpoint, content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            var rpcResponse = JsonConvert.DeserializeObject<RpcResponse<T>>(jsonResponse);

            if (rpcResponse.Error != null)
            {
                throw new Exception(rpcResponse.Error.Message);
            }

            return rpcResponse.Result;
        }
    }
}


