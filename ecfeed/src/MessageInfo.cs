using Newtonsoft.Json;
using System.Linq;

namespace EcFeed
{
    internal struct MessageInfo
    {
        [JsonProperty("info", Required = Required.Always)] internal string Info { get; set; }

        public override string ToString() => $"Info: { Info }";
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
        internal static string[] ExtractArgumentNames(MessageInfo schema)
        {
            string method = ExtractMethodName(schema);
            int leftBracket = method.IndexOf("(");
            int rightBracket = method.IndexOf(")");
            string argumentsString = method.Substring(leftBracket + 1, (rightBracket - leftBracket - 1)) + ", TestHeader ecfeed";
            
            return argumentsString.Split(",").Select(argument => argument.Trim().Split(" ")[1]).ToArray();
        }

        internal static string[] ExtractArgumentTypes(MessageInfo schema)
        {
            string method = ExtractMethodName(schema);
            int leftBracket = method.IndexOf("(");
            int rightBracket = method.IndexOf(")");
            string argumentsString = method.Substring(leftBracket + 1, (rightBracket - leftBracket)) + ", TestHeader ecfeed";
            
            return argumentsString.Split(",").Select(argument => argument.Trim().Split(" ")[0]).ToArray();
        }

        internal static string ExtractMethodName(MessageInfo schema)
        {
            dynamic data = JsonConvert.DeserializeObject(schema.Info);

            return data.method;
        }

        internal static int ExtractTimestamp(MessageInfo schema)
        {
            dynamic data = JsonConvert.DeserializeObject(schema.Info);

            return data.timestamp;
        }

        internal static string ExtractTestSessionId(MessageInfo schema)
        {
            dynamic data = JsonConvert.DeserializeObject(schema.Info);

            return data.testSessionId;
        }
    }
}