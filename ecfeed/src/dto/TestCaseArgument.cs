using Newtonsoft.Json;

namespace EcFeed
{
    public struct TestCaseArgument
    {
        [JsonProperty("name", Required = Required.AllowNull)] public string Name { get; set; }
        [JsonProperty("value", Required = Required.AllowNull)] public object Value { get; set; }

        public override string ToString() => $"\t[ { Value.GetType() } : { Name } : { Value } ]";
    }

    static class TestCaseArgumentHelper { }
}