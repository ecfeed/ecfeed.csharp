using System.Collections;
using NUnit.Framework;
using EcFeed;

using System.IO;
using System;
using System.Diagnostics;

namespace exampleNUnit
{

    public enum CSharpEnum { Uno, Dos, Tres }

    [TestFixture]
    public class UnitTest
    {
        static public IEnumerable DataString = new TestProvider("IMHL-K0DU-2U0I-J532-25J9").GenerateNWise("com.example.test.Playground.size_10x10");
        // static public IEnumerable DataInt = new TestProvider("IMHL-K0DU-2U0I-J532-25J9").GenerateNWise("com.example.test.Demo.typeInt");
        // static public IEnumerable DataEnum = new TestProvider("IMHL-K0DU-2U0I-J532-25J9").GenerateNWise("com.example.test.Demo.typeEnum", 1);
        // static public IEnumerable DataMix = new TestProvider("IMHL-K0DU-2U0I-J532-25J9").GenerateNWise("com.example.test.Demo.typeMix");

        [TestCaseSource("DataString")]
        public void TestString(string a, string b, string c, string d, string e, string f, string g, string h, string i, string j, TestData ecfeed)
        {
            ecfeed.feedback(true);
            // NUnit.Framework.TestContext.Progress.WriteLine(ecfeed.TestCasesParsed +"/" + ecfeed.TestCasesTotal);
            // Assert.That(false, Is.True);
        }

        // [TestCaseSource("DataInt")]
        // public void TestInt(int a0, int a1)
        // {
        //     // Assert.That(false, Is.True);
        // }

        // [TestCaseSource("DataMix")]
        // public void TestMix(byte a0, short a1, int a3, long a4, float a5, double a6, bool a7)
        // {
        //     // Assert.That(false, Is.True);
        // }

        // [TestCaseSource("DataEnum")]
        // public void TestEnum(CSharpEnum a0)
        // {
        //     // Assert.That(false, Is.True);
        // }
    }

}