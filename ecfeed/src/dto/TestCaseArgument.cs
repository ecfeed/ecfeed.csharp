using Newtonsoft.Json;

namespace EcFeed
{
    public struct TestCaseArgument
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("value")] public object Value { get; set; }

        public override string ToString() => $"\t[ { Value.GetType() } : { Name } : { Value } ]";
    }

    static class TestCaseArgumentHelper { }
}