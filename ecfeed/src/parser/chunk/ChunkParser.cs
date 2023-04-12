using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace EcFeed
{
    internal static class ChunkParser
    {
        internal static object[] ParseTestCaseToDataType(string line, DataSession session, StructureInitializer initializer)
        {
            var parsedData = JsonConvert.DeserializeObject<TestCase>(line);
            
            var queue = new Queue<string>();

            foreach (var element in parsedData.TestCaseArguments)
            {
                queue.Enqueue(element.Value.ToString());
            }

            var test = initializer.GetTestCase(session.MethodNameQualified, queue);


            if (session.BuildFeedback)
            {
                Array.Resize(ref test, test.Length + 1);
                test[^1] = new TestHandle(session, line, session.IncrementTestCasesTotal());
            }

            return test;
        }
    }

}