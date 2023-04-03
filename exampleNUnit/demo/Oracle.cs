using System;
using System.Collections.Generic;
using NUnit.Framework;
using EcFeed;

// NUnit.Framework.TestContext.Progress.WriteLine("Test");

namespace exampleNUnit
{
    public class Oracle
    {
        private static Random rand = new Random();
    
        internal static void ValidateF10x10(string a, string b, string c, string d, string e, string f, string g, string h, string i, string j, TestHandle ecfeed)
        {
            Assert.AreNotEqual(a, "a0");
            Assert.AreNotEqual(b, "b1");
            Assert.AreNotEqual(h, "h6");
        }

        internal static void ValidateFeedbacF10x10(string a, string b, string c, string d, string e, string f, string g, string h, string i, string j, TestHandle ecfeed)
        {
            try
            {   
                Assert.AreNotEqual(a, "a0");
            } catch
            {
                ecfeed.addFeedback(false, duration:GetDuration(), comment:"Failed - a", custom:GetCustom());
            }
            try
            {
                Assert.AreNotEqual(b, "b1");
            } catch
            {
                 ecfeed.addFeedback(false, duration:GetDuration(), comment:"Failed - b", custom:GetCustom());
            }
            try
            {
                Assert.AreNotEqual(h, "h6");
            } catch
            {
                 ecfeed.addFeedback(false, duration:GetDuration(), comment:"Failed - c", custom:GetCustom());
            }
        }

        internal static void ValidateF100x2(string a, string b, TestHandle ecfeed)
        {
            Assert.AreNotEqual(a, "a00");
            Assert.AreNotEqual(b, "b00");
        }

        internal static void ValidateFeedbackF100x2(string a, string b, TestHandle ecfeed)
        {
            try
            {   
                Assert.AreNotEqual(a, "a00");
            } catch
            {
                ecfeed.addFeedback(false, duration:GetDuration(), comment:"Failed - a", custom:GetCustom());
            }
            try
            {
                Assert.AreNotEqual(b, "b00");
            } catch
            {
                 ecfeed.addFeedback(false, duration:GetDuration(), comment:"Failed - b", custom:GetCustom());
            }
        }

        internal static void ValidateFTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
        {
            Assert.IsTrue(arg1 >= arg2);
            Assert.IsTrue(arg1 >= arg3);
        }

        internal static void ValidateFeedbackFTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
        {
            try
            {   
                 Assert.IsTrue(arg1 >= arg2);
            } catch
            {
                ecfeed.addFeedback(false, duration:GetDuration(), comment:"Failed - arg1 < arg2", custom:GetCustom());
            }
             try
            {   
                 Assert.IsTrue(arg1 >= arg3);
            } catch
            {
                ecfeed.addFeedback(false, duration:GetDuration(), comment:"Failed - arg1 < arg3", custom:GetCustom());
            }
        }

        private static int GetDuration()
        {
            return rand.Next(100, 2001);
        }

        private static Dictionary<string, string> GetCustom()
        {
            Dictionary<string, string> custom = new Dictionary<string, string>();
            
            for (int i = 0; i < rand.Next(2, 10); i++)
            {
                custom.Add("key " + i, "value " + i);
            }

            return custom;
        }
    }

}