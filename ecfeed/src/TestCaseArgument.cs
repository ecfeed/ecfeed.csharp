using Newtonsoft.Json;

namespace EcFeed
{
    internal struct TestCaseArgument
    {
        [JsonProperty("name", Required = Required.AllowNull)] internal string Name { get; set; }
        [JsonProperty("value", Required = Required.AllowNull)] internal object Value { get; set; }

        public override string ToString() => $"\t[ { Value.GetType() } : { Name } : { Value } ]";
    }

}