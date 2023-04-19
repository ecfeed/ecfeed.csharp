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
                return data[field] != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void ParseInfoMessageMethod(dynamic data, ref DataSession session)
        {
            string method = data.method.ToString();

            int leftBracket = method.IndexOf("(");
            int rightBracket = method.IndexOf(")");
            string argumentsString = method.Substring(leftBracket + 1, (rightBracket - leftBracket - 1)) + ", TestData ecfeed";
            string[] argumentsList = argumentsString.Split(",").Select(e => e.Trim()).ToArray();

            session.MethodArgumentNames = ExtractArgumentNames(argumentsList);
            session.MethodArgumentTypes = ExtractArgumentTypes(argumentsList);
            session.MethodNameQualified = data.method;
            session.Timestamp = data.timestamp;
            session.TestSessionId = data.testSessionId;
        }

        private static string[] ExtractArgumentNames(string[] argumentsList)
        {
            if (argumentsList.Length == 0)
            {
                return new string[0];
            }

            if (argumentsList[0].Contains(":"))
            {
                return ExtractArgumentNamesNew(argumentsList);
            }
            else
            {
                return ExtractArgumentNamesOld(argumentsList);
            }
            
        }

        private static string[] ExtractArgumentNamesOld(string[] argumentsList)
        {
            
            return argumentsList.Select(argument => argument.Split(" ")[1]).ToArray();
        }

        private static string[] ExtractArgumentNamesNew(string[] argumentsList)
        {
            
            return argumentsList.Select(argument => argument.Split(" ")[0]).ToArray();
        }

        private static string[] ExtractArgumentTypes(string[] argumentsList)
        {
            if (argumentsList.Length == 0)
            {
                return new string[0];
            }

            if (argumentsList[0].Contains(":"))
            {
                return ExtractArgumentTypesNew(argumentsList);
            }
            else
            {
                return ExtractArgumentTypesOld(argumentsList);
            }
            
        }

        private static string[] ExtractArgumentTypesOld(string[] argumentsList)
        {
            var results = new string[argumentsList.Length];

            for (var i = 0 ; i < argumentsList.Length ; i++)
            {
                var elements = argumentsList[i].Split(" ");
                var name = elements[1];
                var type = elements[0];

                if (type == "Structure")
                {
                    results[i] = name;
                }
                else
                {
                    results[i] = type;
                }
            }
            
            return results;
        }

        private static string[] ExtractArgumentTypesNew(string[] argumentsList)
        {
            var results = new string[argumentsList.Length];

            for (var i = 0 ; i < argumentsList.Length ; i++)
            {
                var elements = argumentsList[i].Split(" ");
                var name = elements[0];
                var type = elements[^1];

                if (type == "Structure")
                {
                    results[i] = name;
                }
                else
                {
                    results[i] = type;
                }
            }
            
            return results;
        }

        private static void ParseInfoMessageAddSignature(dynamic data, StructureInitializer initializer)
        {
            var signatures = new string[1];
            signatures[0] = data.signature.ToString();

            initializer.Activate(signatures);
        }
    }
}