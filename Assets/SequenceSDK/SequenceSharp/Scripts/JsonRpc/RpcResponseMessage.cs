
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace SequenceSharp
{

    [JsonObject]
    public class RpcResponseMessage
    {
        public RpcResponseMessage(object id, RpcError error) { }
        public RpcResponseMessage(object id, JToken result) { }
        [JsonConstructor]
        protected RpcResponseMessage() { }
        protected RpcResponseMessage(object id) { }

        [JsonProperty("id", Required = Required.Default)]
        public object Id { get; }
        [JsonProperty("jsonrpc", Required = Required.Always)]
        public string JsonRpcVersion { get; }
        [JsonProperty("result", Required = Required.Default)]
        public JToken Result { get; }
        [JsonProperty("error", Required = Required.Default)]
        public RpcError Error { get; protected set; }
        [JsonIgnore]
        public bool HasError { get; }

        public T GetResult<T>( bool returnDefaultIfNull = true, JsonSerializerSettings settings = null)
        {
            if (Result == null)
            {
                if (!returnDefaultIfNull && default(T) != null)
                {
                    throw new Exception("Unable to convert the result (null) to type " + typeof(T));
                }
                return default(T);
            }
            try
            {
                if (settings == null)
                {
                    return Result.ToObject<T>();
                }
                else
                {
                    JsonSerializer jsonSerializer = JsonSerializer.Create(settings);
                    return Result.ToObject<T>(jsonSerializer);
                }
            }
            catch (FormatException ex)
            {
                throw new FormatException("Invalid format when trying to convert the result to type " + typeof(T), ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to convert the result to type " + typeof(T), ex);
            }
        }
    }
}