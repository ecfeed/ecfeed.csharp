using System.Collections;
using NUnit.Framework;
using EcFeed;

namespace exampleNUnit
{

    public enum CSharpEnum { Uno, Dos, Tres }

    [TestFixture]
    public class UnitTest
    {
        static public IEnumerable DataString = new TestProvider("7482-5194-2849-1943-2448").GenerateNWise("com.example.test.Demo.typeString");
        static public IEnumerable DataInt = new TestProvider("7482-5194-2849-1943-2448").GenerateNWise("com.example.test.Demo.typeInt");
        static public IEnumerable DataEnum = new TestProvider("7482-5194-2849-1943-2448").GenerateNWise("com.example.test.Demo.typeEnum", 1);
        static public IEnumerable DataMix = new TestProvider("7482-5194-2849-1943-2448").GenerateNWise("com.example.test.Demo.typeMix");

        [TestCaseSource("DataString")]
        public void TestString(string a0, string a1, string a2, string a3, string a4, string a5, string a6, string a7, string a8, string a9, string a10)
        {
            // Assert.That(false, Is.True);
        }

        [TestCaseSource("DataInt")]
        public void TestInt(int a0, int a1)
        {
            // Assert.That(false, Is.True);
        }

        [TestCaseSource("DataMix")]
        public void TestMix(byte a0, short a1, int a3, long a4, float a5, double a6, bool a7)
        {
            // Assert.That(false, Is.True);
        }

        [TestCaseSource("DataEnum")]
        public void TestEnum(CSharpEnum a0)
        {
            // Assert.That(false, Is.True);
        }
    }

}