using Newtonsoft.Json;
using NUnit.Framework;

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

        public static object[] ParseTestCaseToDataType(string data)
        {
            return ParseTestCaseToDataType(ParseTestCase(data));
        }
        public static object[] ParseTestCaseToDataType(TestCase data)
        {
            object[] result = new object[data.TestCaseArguments.Length];

            for (int i=0 ; i < data.TestCaseArguments.Length ; i++)
            {
                result[i] = data.TestCaseArguments[i].Value;
            }

            return result;
        }

        public static TestCaseData ParseTestToNUnit(string data)
        {
            return ParseTestToNUnit(ParseTestCase(data));
        }

        public static TestCaseData ParseTestToNUnit(TestCase data)
        {
            return new TestCaseData(ParseTestCaseToDataType(data));
        }

    }

}