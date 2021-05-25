using Newtonsoft.Json;

namespace EcFeed
{
    internal struct TestCase
    {
        [JsonProperty("testCase", Required = Required.Always)] internal TestCaseArgument[] TestCaseArguments { get; set; }

        public override string ToString() => HelperTestCase.ParseToString(ref this);
    }
}