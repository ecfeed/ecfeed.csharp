using Newtonsoft.Json;

namespace EcFeed
{
    internal struct MessageStatus
    {
        [JsonProperty("status", Required = Required.Always)] internal string Status { get; set; }

        public override string ToString() => $"Status: { Status }";
    }
}