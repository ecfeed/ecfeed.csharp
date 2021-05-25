using System.Text;

namespace EcFeed
{
    internal static class HelperTestCase
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