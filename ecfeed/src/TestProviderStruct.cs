using Newtonsoft.Json;
using System.Linq;
using System.Text;

namespace EcFeed
{
    internal struct MessageInfo
    {
        [JsonProperty("info", Required = Required.Always)] internal string Info { get; set; }

        public override string ToString() => $"Info: { Info }";
    }
    internal struct MessageStatus
    {
        [JsonProperty("status", Required = Required.Always)] internal string Status { get; set; }

        public override string ToString() => $"Status: { Status }";
    }
    internal struct TestCase
    {
        [JsonProperty("testCase", Required = Required.Always)] internal TestCaseArgument[] TestCaseArguments { get; set; }

        public override string ToString() => TestCaseHelper.ParseToString(ref this);
    }
    internal struct TestCaseArgument
    {
        [JsonProperty("name", Required = Required.AllowNull)] internal string Name { get; set; }
        [JsonProperty("value", Required = Required.AllowNull)] internal object Value { get; set; }

        public override string ToString() => $"\t[ { Value.GetType() } : { Name } : { Value } ]";
    }
    
    internal static class MessageInfoHelper 
    { 
        internal static void ParseInfoMessage(string line, ref SessionData feedback)
        {
            MessageInfo infoMessage = JsonConvert.DeserializeObject<MessageInfo>(line);
                
            feedback.MethodArgumentNames = MessageInfoHelper.ExtractArgumentNames(infoMessage);
            feedback.MethodArgumentTypes = MessageInfoHelper.ExtractArgumentTypes(infoMessage);
            feedback.MethodNameQualified = MessageInfoHelper.ExtractMethodName(infoMessage);
            feedback.Timestamp = MessageInfoHelper.ExtractTimestamp(infoMessage);
            feedback.TestSessionId = MessageInfoHelper.ExtractTestSessionId(infoMessage);
        }
        private static string[] ExtractArgumentNames(MessageInfo schema)
        {
            string method = ExtractMethodName(schema);
            int leftBracket = method.IndexOf("(");
            int rightBracket = method.IndexOf(")");
            string argumentsString = method.Substring(leftBracket + 1, (rightBracket - leftBracket - 1)) + ", TestData ecfeed";
            
            return argumentsString.Split(",").Select(argument => argument.Trim().Split(" ")[1]).ToArray();
        }

        private static string[] ExtractArgumentTypes(MessageInfo schema)
        {
            string method = ExtractMethodName(schema);
            int leftBracket = method.IndexOf("(");
            int rightBracket = method.IndexOf(")");
            string argumentsString = method.Substring(leftBracket + 1, (rightBracket - leftBracket)) + ", TestData ecfeed";
            
            return argumentsString.Split(",").Select(argument => argument.Trim().Split(" ")[0]).ToArray();
        }

        private static string ExtractMethodName(MessageInfo schema)
        {
            dynamic data = JsonConvert.DeserializeObject(schema.Info);

            return data.method;
        }

        private static int ExtractTimestamp(MessageInfo schema)
        {
            dynamic data = JsonConvert.DeserializeObject(schema.Info);

            return data.timestamp;
        }

        private static string ExtractTestSessionId(MessageInfo schema)
        {
            dynamic data = JsonConvert.DeserializeObject(schema.Info);

            return data.testSessionId;
        }
    }
    internal static class MessageStatusHelper
    {
        internal static bool IsTransmissionFinished(MessageStatus messageStatus)
        {
            return messageStatus.Status.Contains("END_DATA");
        }
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