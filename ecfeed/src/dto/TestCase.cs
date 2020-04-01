using System.Text;
using Newtonsoft.Json;

namespace EcFeed
{
    public struct TestCase
    {
        [JsonProperty("testCase")] public TestCaseArgument[] TestCaseArguments { get; set; }

        public override string ToString() => TestCaseHelper.ParseToString(ref this);
    }

    static class TestCaseHelper
    {
        internal static string ParseToString(ref TestCase schema)
        {
            if (schema.TestCaseArguments == null)
            {
                return "NOT PARSABLE";
            }

            StringBuilder description = new StringBuilder("");

            description.AppendLine($"Number of arguments: { schema.TestCaseArguments.Length }.");

            foreach (TestCaseArgument testArgument in schema.TestCaseArguments)
            {
                description.AppendLine(testArgument.ToString());
            }

            return description.ToString();
        }
    }
}