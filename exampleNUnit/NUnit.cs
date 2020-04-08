using System.Collections;
using NUnit.Framework;
using EcFeed;

namespace exampleNUnit
{
    [TestFixture]
    public class UnitTest
    {
        // [TestCaseSource(typeof(FeedString))]
        // public void TestString(string a0, string a1, string a2, string a3, string a4, string a5, string a6, string a7, string a8, string a9, string a10)
        // {
        //     Assert.That(false, Is.True);
        // }

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

        [TestCaseSource(typeof(FeedEnum))]
        public void TestEnum(CSharpEnum a0)
        {
            // Assert.That(false, Is.True);
        }
    }

    class FeedString : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            TestProvider testProvider = new TestProvider();
            testProvider.Model = "7482-5194-2849-1943-2448";

            return testProvider.GenerateNWise("com.example.test.Demo.typeString").GetEnumerator();
        }
    }

    class FeedInt : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            ITestProvider testProvider = new TestProvider();
            testProvider.Model = "7482-5194-2849-1943-2448";

            return testProvider.GenerateNWise("com.example.test.Demo.typeInt").GetEnumerator();
        }
    }

    class FeedMix : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            ITestProvider testProvider = new TestProvider();
            testProvider.Model = "7482-5194-2849-1943-2448";

            return testProvider.GenerateNWise("com.example.test.Demo.typeMix").GetEnumerator();
        }
    }

    class FeedEnum : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            ITestProvider testProvider = new TestProvider();
            testProvider.Model = "7482-5194-2849-1943-2448";

            return testProvider.GenerateNWise("com.example.test.Demo.typeEnum", 1).GetEnumerator();
        }
    }

    public enum CSharpEnum { Uno, Dos, Tres }
}