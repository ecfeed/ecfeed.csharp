using Newtonsoft.Json;
using System;

namespace EcFeed
{
    internal static class StreamParser
    {
        internal static object[] ParseTestCaseToDataType(string data, SessionData feedback)
        {
            return ParseTestCaseToDataType(JsonConvert.DeserializeObject<TestCase>(data), feedback);
        }
        internal static object[] ParseTestCaseToDataType(TestCase data, SessionData feedback)
        {
            object[] result = new object[data.TestCaseArguments.Length + 1];

            for (int i=0 ; i < data.TestCaseArguments.Length ; i++)
            {
                result[i] = CastType(data.TestCaseArguments[i].Value, feedback.MethodArgumentTypes[i]);
            }

            result[result.Length - 1] = feedback;

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