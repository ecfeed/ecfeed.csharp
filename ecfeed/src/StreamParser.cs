using Newtonsoft.Json;
using System;

namespace EcFeed
{
    internal static class StreamParser
    {
        internal static object[] ParseTestCaseToDataType(string data, SessionData sessionData)
        {
            TestCase parsedData = JsonConvert.DeserializeObject<TestCase>(data);
            object[] result = new object[parsedData.TestCaseArguments.Length + 1];

            for (int i=0 ; i < parsedData.TestCaseArguments.Length ; i++)
            {
                result[i] = CastType(parsedData.TestCaseArguments[i].Value, sessionData.MethodArgumentTypes[i]);
            }

            result[result.Length - 1] = new TestData(sessionData, data, sessionData.IncrementTestCasesTotal());

            return result;
        }

        private static object CastType(object value, string type)
        {
            try
            {
                switch (type)
                {
                    case "byte": return Convert.ToByte(value);
                    case "short": return Convert.ToInt16(value);
                    case "int": return Convert.ToInt32(value);
                    case "long": return Convert.ToInt64(value);
                    case "float": return Convert.ToSingle(value);
                    case "double": return Convert.ToDouble(value);
                    case "boolean": return Convert.ToBoolean(value);
                }

                foreach (Type userType in Dependencies.UserType)
                {   
                    if (userType.FullName.EndsWith(type))
                    {
                        string[] enumElements = userType.GetEnumNames();
                        return Enum.ToObject(userType, Array.IndexOf(enumElements, value));
                    }
                }
                
                return value;
            }
            catch (Exception)
            {
                return value;
            }
            
        }

    }

}