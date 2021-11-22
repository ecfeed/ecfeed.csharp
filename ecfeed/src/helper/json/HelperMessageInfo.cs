using Newtonsoft.Json;
using System.Linq;

namespace EcFeed
{
    internal static class HelperMessageInfo 
    { 
        internal static void ParseInfoMessage(string line, ref DataSession feedback)
        {
            MessageInfo infoMessage = JsonConvert.DeserializeObject<MessageInfo>(line);
                
            feedback.MethodArgumentNames = HelperMessageInfo.ExtractArgumentNames(infoMessage);
            feedback.MethodArgumentTypes = HelperMessageInfo.ExtractArgumentTypes(infoMessage);
            feedback.MethodNameQualified = HelperMessageInfo.ExtractMethodName(infoMessage);
            feedback.Timestamp = HelperMessageInfo.ExtractTimestamp(infoMessage);
            feedback.TestSessionId = HelperMessageInfo.ExtractTestSessionId(infoMessage);
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
}