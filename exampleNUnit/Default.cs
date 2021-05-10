// using System;
// using System.Collections;
// using System.Collections.Generic;
// using NUnit.Framework;
// using EcFeed;

// // NUnit.Framework.TestContext.Progress.WriteLine("Test");

// namespace exampleNUnit
// {

//     public enum CSharpEnum { Uno, Dos, Tres }

//     [TestFixture]
//     public class UnitTest
//     {
//         static private TestProvider testProvider = new TestProvider("LRXC-015K-GJB0-2A9F-CGA2");
//         static private string method1 = "com.example.test.Playground.size_10x10";

//         static private IEnumerable Method1a = testProvider.GenerateNWise(method1, feedback:true);
//         static private IEnumerable Method1b = testProvider.GenerateRandom(method1, feedback:true, length:(int)(new Random().Next(100, 500)));
//         static private IEnumerable Method1c = testProvider.GenerateRandom(method1, feedback:true, length:(int)(new Random().Next(50, 100)), adaptive:false);
//         static private IEnumerable Method1d = testProvider.GenerateRandom(method1, feedback:true, length:(int)(new Random().Next(50, 100)), adaptive:true, duplicates:true);
//         static private IEnumerable Method1e = testProvider.GenerateRandom(method1, feedback:true, length:(int)(new Random().Next(2000, 5000)));

//         internal void ValidateMethod1(string a, string b, string c, string d, string e, string f, string g, string h, string i, string j, TestHandle ecfeed)
//         {
//             Assert.AreNotEqual(a, "a0", ecfeed.addFeedback(false, comment:"Failed1"));
//             Assert.AreNotEqual(b, "b1", ecfeed.addFeedback(false, comment:"Failed2"));
//             Assert.AreNotEqual(h, "h6", ecfeed.addFeedback(false, comment:"Failed3"));
 
//             ecfeed.addFeedback(true, comment:"OK");
//         }

//         [TearDown]
//         public void TearDown()
//         {
//             TestHandle ecfeed = TestContext.CurrentContext.Test.Arguments[^1] as TestHandle; 
            
//             ecfeed.addFeedback(
//                 TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed,
//                 comment:TestContext.CurrentContext.Result.Message
//             );
//         }

//         [TestCaseSource("Method1a")]
//         public void Method1aTest(string a, string b, string c, string d, string e, string f, string g, string h, string i, string j, TestHandle ecfeed)
//         {
//             ValidateMethod1(a, b, c, d, e, f, g, h, i, j, ecfeed);
//         }

//         [TestCaseSource("Method1b")]
//         public void Method1bTest(string a, string b, string c, string d, string e, string f, string g, string h, string i, string j, TestHandle ecfeed)
//         {
//             ValidateMethod1(a, b, c, d, e, f, g, h, i, j, ecfeed);
//         }

//         [TestCaseSource("Method1c")]
//         public void Method1cTest(string a, string b, string c, string d, string e, string f, string g, string h, string i, string j, TestHandle ecfeed)
//         {
//             ValidateMethod1(a, b, c, d, e, f, g, h, i, j, ecfeed);
//         }

//         [TestCaseSource("Method1d")]
//         public void Method1dTest(string a, string b, string c, string d, string e, string f, string g, string h, string i, string j, TestHandle ecfeed)
//         {
//             ValidateMethod1(a, b, c, d, e, f, g, h, i, j, ecfeed);
//         }

//         [TestCaseSource("Method1e")]
//         public void Method1eTest(string a, string b, string c, string d, string e, string f, string g, string h, string i, string j, TestHandle ecfeed)
//         {
//             ValidateMethod1(a, b, c, d, e, f, g, h, i, j, ecfeed);
//         }

//         static internal string method2 = "QuickStart.test";

//         static public IEnumerable Method2a = testProvider.GenerateRandom(method2, feedback:true, length:20);
//         static public IEnumerable Method2b = testProvider.GenerateCartesian(method2, feedback:true);
//         static public IEnumerable Method2c = testProvider.GenerateNWise(method2, feedback:true);
//         static public IEnumerable Method2d = testProvider.GenerateNWise(method2, feedback:true, label:"Test session label", constraints:"ALL", custom:new Dictionary<string, string>{{"first", "uno"}, {"second", "dos"}});
//         static public IEnumerable Method2e = testProvider.GenerateNWise(method2, feedback:true, label:"Test session label", constraints:new string[]{"constraint1", "constraint2"}, choices:new Dictionary<string, string[]>{{"arg1", new string[]{"choice1", "choice2"}}, {"arg2", new string[]{"choice2", "choice2", "choice3"}}});
//         static public IEnumerable Method2f = testProvider.GenerateStatic(method2, feedback:true, testSuites:"ALL");
//         static public IEnumerable Method2g = testProvider.GenerateStatic(method2, feedback:true, testSuites:new string[]{"suite1"});
//         static public IEnumerable Method2h = testProvider.GenerateNWise(method2, feedback:true, label:"Lorem ipsum dolor sit amet.");

//         internal void ValidateMethod2(int arg1, int arg2, int arg3, TestHandle ecfeed)
//         {
//             try
//             {
//                 Assert.Less(arg1, 2);
//             }
//             catch (AssertionException ex)
//             {
//                 ecfeed.addFeedback(false, comment:"Failed", duration:(int)(new Random().Next(10, 1000)));
//                 throw ex;
//             }

//             ecfeed.addFeedback(true, comment:"OK");
//         }

//         [TestCaseSource("Method2a")]
//         public void Method2aTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
//         {
//             ValidateMethod2(arg1, arg2, arg3, ecfeed);
//         }

//         [TestCaseSource("Method2b")]
//         public void Method2bTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
//         {
//             ValidateMethod2(arg1, arg2, arg3, ecfeed);
//         }

//         [TestCaseSource("Method2c")]
//         public void Method2cTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
//         {
//             ValidateMethod2(arg1, arg2, arg3, ecfeed);
//         }

//         [TestCaseSource("Method2d")]
//         public void Method2dTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
//         {
//             ValidateMethod2(arg1, arg2, arg3, ecfeed);
//         }

//         [TestCaseSource("Method2e")]
//         public void Method2eTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
//         {
//             ValidateMethod2(arg1, arg2, arg3, ecfeed);
//         }

//         [TestCaseSource("Method2f")]
//         public void Method2fTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
//         {
//             ValidateMethod2(arg1, arg2, arg3, ecfeed);
//         }

//         [TestCaseSource("Method2g")]
//         public void Method2gTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
//         {
//             ValidateMethod2(arg1, arg2, arg3, ecfeed);
//         }

//         [TestCaseSource("Method2h")]
//         public void Method2hTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
//         {
//             try
//             {
//                 Assert.Less(arg1, 2);
//             }
//             catch (AssertionException ex)
//             {
//                 ecfeed.addFeedback(false, comment:"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.", duration:(int)(new Random().Next(10, 1000)), custom:new Dictionary<string, string>{{"first", "uno"}, {"second", "dos"}});
//                 throw ex;
//             }

//             ecfeed.addFeedback(true, comment:"OK");
//         }

//         static internal string method3 = "com.example.test.Playground.size_100x2";

//         static public IEnumerable Method3a = testProvider.GenerateNWise(method3, feedback:true);
//         static public IEnumerable Method3b = testProvider.GenerateRandom(method3, feedback:true, length:(int)(new Random().Next(1000, 10000)));
//         static public IEnumerable Method3c = testProvider.GenerateRandom(method3, feedback:true, length:100);

//         internal void ValidateMethod3(string a, string b, TestHandle ecfeed)
//         {
//             try
//             {
//                 Assert.AreNotEqual(a, "a00");
//                 Assert.AreNotEqual(b, "b00");
//             }
//             catch (AssertionException ex)
//             {
//                 ecfeed.addFeedback(false, comment:"Failed", duration:(int)(new Random().Next(10, 1000)));
//                 throw ex;
//             }

//             ecfeed.addFeedback(true, comment:"OK");
//         }

//         [TestCaseSource("Method3a")]
//         public void Method3aTest(string a, string b, TestHandle ecfeed)
//         {
//             ValidateMethod3(a, b, ecfeed);
//         }

//         [TestCaseSource("Method3b")]
//         public void Method3bTest(string a, string b, TestHandle ecfeed)
//         {
//             ValidateMethod3(a, b, ecfeed);
//         }

//         [TestCaseSource("Method3c")]
//         public void Method3cTest(string a, string b, TestHandle ecfeed)
//         {
//             ValidateMethod3(a, b, ecfeed);
//         }
//     }

// }