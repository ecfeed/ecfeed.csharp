using Newtonsoft.Json;

namespace EcFeed
{
    internal struct MessageInfo
    {
        [JsonProperty("info", Required = Required.Always)] internal string Info { get; set; }

        public override string ToString() => $"Info: { Info }";
    }
    
}