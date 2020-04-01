using Newtonsoft.Json;

namespace EcFeed
{
    public struct StatusMessage
    {
        [JsonProperty("status")] public string Status { get; set; }

        public override string ToString() => $"Status: { Status }";
    }

    static class StatusMessageHelper { }
}