// using System;
// using System.Collections;
// using System.Collections.Generic;
// using NUnit.Framework;
// using EcFeed;

// namespace exampleNUnit
// {

//     [TestFixture]
//     public class NUnit5F10x10
//     {
//         static private IEnumerable GenRandomQuantitySingle = ConfigDefault.TestProvider.GenerateRandom(ConfigDefault.F_10x10, feedback:true,
//             length:1, label:"Random / Quality - Single");
//         static private IEnumerable GenRandomQuantityShort = ConfigDefault.TestProvider.GenerateRandom(ConfigDefault.F_10x10, feedback:true, 
//             length:(int)(new Random().Next(100, 500)), label:"Random / Quantity - Short");
//         static private IEnumerable GenRandomQuantityLong = ConfigDefault.TestProvider.GenerateRandom(ConfigDefault.F_10x10, feedback:true, 
//             length:(int)(new Random().Next(1000, 5000)), label:"Random / Quantity - Long");
//         static private IEnumerable GenRandomCustom = ConfigDefault.TestProvider.GenerateRandom(ConfigDefault.F_10x10, feedback:true, 
//             length:1, label:"Random / Custom", custom:new Dictionary<string, string>{{"key1", "value1"}, {"key2", "value2"}});
//         static private IEnumerable GenNWise = ConfigDefault.TestProvider.GenerateNWise(ConfigDefault.F_10x10, feedback:true, 
//             label:"NWise");
//         static private IEnumerable GenNWiseFeedback = ConfigDefault.TestProvider.GenerateNWise(ConfigDefault.F_10x10, feedback:true, 
//             label:"NWise / Feedback");

//         [TearDown]
//         public void TearDown()
//         {
//             TestHandle ecfeed = TestContext.CurrentContext.Test.Arguments[^1] as TestHandle; 
            
//             ecfeed.AddFeedback(
//                 TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed,
//                 comment:TestContext.CurrentContext.Result.Message
//             );
//         }

//         [TestCaseSource("GenRandomQuantitySingle")]
//         public void GenRandomQuantitySingleTest(string a, string b, string c, string d, string e, string f, string g, string h, string i, string j, TestHandle ecfeed)
//         {
//             Oracle.ValidateF10x10(a, b, c, d, e, f, g, h, i, j, ecfeed);
//         }

//         [TestCaseSource("GenRandomQuantityShort")]
//         public void GenRandomQuantityShortTest(string a, string b, string c, string d, string e, string f, string g, string h, string i, string j, TestHandle ecfeed)
//         {
//             Oracle.ValidateF10x10(a, b, c, d, e, f, g, h, i, j, ecfeed);
//         }

//         [TestCaseSource("GenRandomQuantityLong")]
//         public void GenRandomQuantityLongTest(string a, string b, string c, string d, string e, string f, string g, string h, string i, string j, TestHandle ecfeed)
//         {
//             Oracle.ValidateF10x10(a, b, c, d, e, f, g, h, i, j, ecfeed);
//         }

//         [TestCaseSource("GenRandomCustom")]
//         public void GenRandomCustomTest(string a, string b, string c, string d, string e, string f, string g, string h, string i, string j, TestHandle ecfeed)
//         {
//             Oracle.ValidateF10x10(a, b, c, d, e, f, g, h, i, j, ecfeed);
//         }

//         [TestCaseSource("GenNWise")]
//         public void GenNWiseTest(string a, string b, string c, string d, string e, string f, string g, string h, string i, string j, TestHandle ecfeed)
//         {
//             Oracle.ValidateF10x10(a, b, c, d, e, f, g, h, i, j, ecfeed);
//         }

//         [TestCaseSource("GenNWiseFeedback")]
//         public void GenNWiseFeedbackTest(string a, string b, string c, string d, string e, string f, string g, string h, string i, string j, TestHandle ecfeed)
//         {
//             Oracle.ValidateFeedbacF10x10(a, b, c, d, e, f, g, h, i, j, ecfeed);
//         }
       
//     }

// }