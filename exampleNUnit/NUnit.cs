using System.Collections;
using NUnit.Framework;
using EcFeed;

namespace exampleNUnit
{
    [TestFixture]
    public class UnitTest
    {
        // [TestCaseSource(typeof(FeedString))]
        [TestCaseSource(nameof(MethodTestProvider), new object[] {"", ""}) ]
        public void TestString(string a0, string a1, string a2, string a3, string a4, string a5, string a6, string a7, string a8, string a9, string a10)
        {
            Assert.That(false, Is.True);
        }

        // [TestCaseSource(typeof(FeedInt))]
        // public void TestInt(int a0, int a1)
        // {
        //     Assert.That(false, Is.True);
        // }

        // [TestCaseSource(typeof(FeedMix))]
        // public void TestMix(byte a0, short a1, int a3, long a4, float a5, double a6, bool a7)
        // {
        //     Assert.That(false, Is.True);
        // }

        static IEnumerable MethodTestProvider(string model, string method)
        {
            ITestProvider testProvider = new TestProvider();
            testProvider.Model = "7482-5194-2849-1943-2448";
            testProvider.Method = "com.example.test.Demo.typeString";

            return testProvider.QueueNWise();
        }
    }

    class FeedString : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            ITestProvider testProvider = new TestProvider();
            testProvider.Model = "7482-5194-2849-1943-2448";
            testProvider.Method = "com.example.test.Demo.typeString";

            return testProvider.QueueNWise().GetEnumerator();
        }
    }

    class FeedInt : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            ITestProvider testProvider = new TestProvider();
            testProvider.Model = "7482-5194-2849-1943-2448";
            testProvider.Method = "com.example.test.Demo.typeInt";

            return testProvider.QueueNWise().GetEnumerator();
        }
    }

    class FeedMix : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            ITestProvider testProvider = new TestProvider();
            testProvider.Model = "7482-5194-2849-1943-2448";
            testProvider.Method = "com.example.test.Demo.typeMix";

            return testProvider.QueueNWise().GetEnumerator();
        }
    }

}