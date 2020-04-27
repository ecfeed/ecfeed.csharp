using System.Text;
using Newtonsoft.Json;

namespace EcFeed
{
    internal struct TestCase
    {
        [JsonProperty("testCase", Required = Required.Always)] internal TestCaseArgument[] TestCaseArguments { get; set; }

        public override string ToString() => TestCaseHelper.ParseToString(ref this);
    }

    internal static class TestCaseHelper
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