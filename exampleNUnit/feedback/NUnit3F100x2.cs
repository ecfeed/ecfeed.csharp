// using System;
// using System.Collections;
// using System.Collections.Generic;
// using NUnit.Framework;
// using EcFeed;

// namespace exampleNUnit
// {

//     [TestFixture]
//     public class NUnit3F100x2
//     {
//         static private IEnumerable GenRandomQuantitySingle = ConfigDefault.GetTestProvider().GenerateRandom(ConfigDefault.F_100x2, feedback:true,
//             length:1, label:"Random / Quantity - Single");
//         static private IEnumerable GenRandomQuantityShort = ConfigDefault.GetTestProvider().GenerateRandom(ConfigDefault.F_100x2, feedback:true, 
//             length:(int)(new Random().Next(100, 500)), label:"Random / Quantity - Short");
//         static private IEnumerable GenRandomQuantityLong = ConfigDefault.GetTestProvider().GenerateRandom(ConfigDefault.F_100x2, feedback:true, 
//             length:(int)(new Random().Next(1000, 5000)), label:"Random / Quantity - Long");
//         static private IEnumerable GenRandomCustom = ConfigDefault.GetTestProvider().GenerateRandom(ConfigDefault.F_100x2, feedback:true, 
//             length:1, label:"Random / Custom", custom:new Dictionary<string, string>{{"key1", "value1"}, {"key2", "value2"}});
//         static private IEnumerable GenNWise = ConfigDefault.GetTestProvider().GenerateNWise(ConfigDefault.F_100x2, feedback:true, 
//             label:"NWise");
//         static private IEnumerable GenNWiseFeedback = ConfigDefault.GetTestProvider().GenerateNWise(ConfigDefault.F_100x2, feedback:true, 
//             label:"NWise / Feedback");

//         [TearDown]
//         public void TearDown()
//         {
//             TestHandle ecfeed = TestContext.CurrentContext.Test.Arguments[^1] as TestHandle; 
            
//             ecfeed.addFeedback(
//                 TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed,
//                 comment:TestContext.CurrentContext.Result.Message
//             );
//         }

//         [TestCaseSource("GenRandomQuantitySingle")]
//         public void GenRandomQuantitySingleTest(string a, string b, TestHandle ecfeed)
//         {
//             Oracle.ValidateF100x2(a, b, ecfeed);
//         }

//        [TestCaseSource("GenRandomQuantityShort")]
//         public void GenRandomQuantityShortTest(string a, string b, TestHandle ecfeed)
//         {
//             Oracle.ValidateF100x2(a, b, ecfeed);
//         }

//         [TestCaseSource("GenRandomQuantityLong")]
//         public void GenRandomQuantityLongTest(string a, string b, TestHandle ecfeed)
//         {
//             Oracle.ValidateF100x2(a, b, ecfeed);
//         }

//         [TestCaseSource("GenRandomCustom")]
//         public void GenRandomCustomTest(string a, string b, TestHandle ecfeed)
//         {
//             Oracle.ValidateF100x2(a, b, ecfeed);
//         }

//         [TestCaseSource("GenNWise")]
//         public void GenNWiseTest(string a, string b, TestHandle ecfeed)
//         {
//             Oracle.ValidateF100x2(a, b, ecfeed);
//         }

//         [TestCaseSource("GenNWiseFeedback")]
//         public void GenNWiseFeedbackTest(string a, string b, TestHandle ecfeed)
//         {
//             Oracle.ValidateFeedbackF100x2(a, b, ecfeed);
//         }
//     }

// }