using Newtonsoft.Json;
using System.Linq;
using System;

namespace EcFeed
{
    internal static class HelperMessageInfo 
    { 
        internal static void ParseInfoMessage(string line, ref DataSession session, StructureInitializer initializer)
        {
            var infoMessage = JsonConvert.DeserializeObject<MessageInfo>(line);
            dynamic info = JsonConvert.DeserializeObject(infoMessage.Info);
                
            if (ParseInfoMessageFieldExists(info, "method"))
            {
                ParseInfoMessageMethod(info, ref session);
                return;
            }
           
            if (ParseInfoMessageFieldExists(info, "signature"))
            {
                ParseInfoMessageAddSignature(info, initializer);
                return;
            }
        }

        private static bool ParseInfoMessageFieldExists(dynamic data, string field)
        {
            try
            {
                var tmp = data[field];
                return tmp != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void ParseInfoMessageMethod(dynamic data, ref DataSession session)
        {
            session.MethodArgumentNames = HelperMessageInfo.ExtractArgumentNames(data.method.ToString());
            session.MethodArgumentTypes = HelperMessageInfo.ExtractArgumentTypes(data.method.ToString());
            session.MethodNameQualified = data.method;
            session.Timestamp = data.timestamp;
            session.TestSessionId = data.testSessionId;
        }

        private static string[] ExtractArgumentNames(string method)
        {
            int leftBracket = method.IndexOf("(");
            int rightBracket = method.IndexOf(")");
            string argumentsString = method.Substring(leftBracket + 1, (rightBracket - leftBracket - 1)) + ", TestData ecfeed";
            
            return argumentsString.Split(",").Select(argument => argument.Trim().Split(" ")[1]).ToArray();
        }

        private static string[] ExtractArgumentTypes(string method)
        {
            int leftBracket = method.IndexOf("(");
            int rightBracket = method.IndexOf(")");
            string argumentsString = method.Substring(leftBracket + 1, (rightBracket - leftBracket)) + ", TestData ecfeed";
            
            return argumentsString.Split(",").Select(argument => argument.Trim().Split(" ")[0]).ToArray();
        }

        private static void ParseInfoMessageAddSignature(dynamic data, StructureInitializer initializer)
        {
            var signatures = new string[1];
            signatures[0] = data.signature.ToString();

            initializer.Activate(signatures);
        }
    }
}