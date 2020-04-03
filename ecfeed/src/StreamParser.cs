using Newtonsoft.Json;
using NUnit.Framework;
using System;

namespace EcFeed
{
    static class StreamParser
    {
        public static TestCase ParseTestCase(string data)
        {
            return JsonConvert.DeserializeObject<TestCase>(data);
        }

        public static StatusMessage ParseStatusMessage(string data)
        {
            return JsonConvert.DeserializeObject<StatusMessage>(data);
        }

        public static InfoMessage ParseInfoMessage(string data)
        {
            return JsonConvert.DeserializeObject<InfoMessage>(data);
        }

        public static object[] ParseTestCaseToDataType(string data, string[] type)
        {
            return ParseTestCaseToDataType(ParseTestCase(data), type);
        }
        public static object[] ParseTestCaseToDataType(TestCase data, string[] type)
        {
            object[] result = new object[data.TestCaseArguments.Length];

            for (int i=0 ; i < data.TestCaseArguments.Length ; i++)
            {
                result[i] = CastType(data.TestCaseArguments[i].Value, type[i]);
            }

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

        public static TestCaseData ParseTestToNUnit(string data, string[] type)
        {
            return ParseTestToNUnit(ParseTestCase(data), type);
        }

        public static TestCaseData ParseTestToNUnit(TestCase data, string[] type)
        {
            return new TestCaseData(ParseTestCaseToDataType(data, type));
        }

    }

}