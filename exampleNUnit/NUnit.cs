using System.Collections;
using NUnit.Framework;
using EcFeed;

namespace exampleNUnit
{
    [Ignore("Some other time")]
    [TestFixture]
    public class UnitTest
    {
        [TestCaseSource(typeof(FeedDefault))]
        public void Test(string a0, string a1, string a2, string a3, string a4, string a5, string a6, string a7, string a8, string a9, string a10)
        {
            Assert.That(false, Is.True);
        }
    }

    class FeedDefault : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            ITestProvider testProvider = new TestProvider();
            testProvider.Model = "7482-5194-2849-1943-2448";
            testProvider.Method = "com.example.test.Demo.typeString";

            return testProvider.QueueNWise().GetEnumerator();
        }
    }
}