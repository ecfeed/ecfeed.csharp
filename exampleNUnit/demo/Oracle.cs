using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using EcFeed;

// NUnit.Framework.TestContext.Progress.WriteLine("Test");

namespace exampleNUnit
{
    public class Oracle
    {
    
        internal static void ValidateF10x10(string a, string b, string c, string d, string e, string f, string g, string h, string i, string j, TestHandle ecfeed)
        {
            Assert.AreNotEqual(a, "a0");
            Assert.AreNotEqual(b, "b1");
            Assert.AreNotEqual(h, "h6");
        }

        internal static void ValidateF100x2(string a, string b, TestHandle ecfeed)
        {
            Assert.AreNotEqual(a, "a00");
            Assert.AreNotEqual(b, "b00");
        }

        internal static void ValidateFTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
        {
            Assert.IsTrue(arg1 < 2);
        }
    }

}