using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using EcFeed;

namespace exampleNUnit
{

    [TestFixture]
    public class NUnitTest
    {
        static private IEnumerable GenRandomQuantitySingle = ConfigDefault.TestProvider.GenerateRandom(ConfigDefault.F_TEST, feedback:true,
            length:1, label:"Random / Quality - Single");
        static private IEnumerable GenRandomQuantityShort = ConfigDefault.TestProvider.GenerateRandom(ConfigDefault.F_TEST, feedback:true, 
            length:(int)(new Random().Next(100, 500)), label:"Random / Quantity - Short");
        static private IEnumerable GenRandomQuantityLong = ConfigDefault.TestProvider.GenerateRandom(ConfigDefault.F_TEST, feedback:true, 
            length:(int)(new Random().Next(1000, 5000)), label:"Random / Quantity - Long");
        static private IEnumerable GenRandom = ConfigDefault.TestProvider.GenerateRandom(ConfigDefault.F_TEST, feedback:true, 
            label:"Random");
        static private IEnumerable GenRandomAdaptive = ConfigDefault.TestProvider.GenerateRandom(ConfigDefault.F_TEST, feedback:true, 
            length:10, adaptive:false, label:"Random - Adaptive");
        static private IEnumerable GenRandomDuplicates = ConfigDefault.TestProvider.GenerateRandom(ConfigDefault.F_TEST, feedback:true, 
            length:10, duplicates:true, label:"Random - Duplicates");
        static private IEnumerable GenNWise = ConfigDefault.TestProvider.GenerateNWise(ConfigDefault.F_TEST, feedback:true, 
            label:"NWise");
        static private IEnumerable GenNWiseN = ConfigDefault.TestProvider.GenerateNWise(ConfigDefault.F_TEST, feedback:true, 
            n:3, label:"NWise - N");
        static private IEnumerable GenNWiseCoverage = ConfigDefault.TestProvider.GenerateNWise(ConfigDefault.F_TEST, feedback:true, 
            coverage:50, label:"NWise - Coverage");
        static private IEnumerable GenNWiseConstraintsNone = ConfigDefault.TestProvider.GenerateNWise(ConfigDefault.F_TEST, feedback:true, 
            constraints:"NONE", label:"NWise / Constraints - None");
        static private IEnumerable GenNWiseConstraintsSelected = ConfigDefault.TestProvider.GenerateNWise(ConfigDefault.F_TEST, feedback:true, 
            constraints:new string[]{"constraint1", "constraint2"}, label:"NWise / Constraints - Selected");
        static private IEnumerable GenNWiseChoicesSelected = ConfigDefault.TestProvider.GenerateNWise(ConfigDefault.F_TEST, feedback:true, 
            choices:new Dictionary<string, string[]>{{"arg1", new string[]{"choice1", "choice2"}}, {"arg2", new string[]{"choice2", "choice2", "choice3"}}}, label:"NWise / Choices - Selected");
        static private IEnumerable GenCartesian = ConfigDefault.TestProvider.GenerateCartesian(ConfigDefault.F_TEST, feedback:true, 
            label:"Cartesian");
        static private IEnumerable GenStatic = ConfigDefault.TestProvider.GenerateStatic(ConfigDefault.F_TEST, feedback:true, 
            label:"Static");
        static private IEnumerable GenStaticAll = ConfigDefault.TestProvider.GenerateStatic(ConfigDefault.F_TEST, feedback:true, 
            testSuites:"ALL", label:"Static - All");
         static private IEnumerable GenStaticSelected = ConfigDefault.TestProvider.GenerateStatic(ConfigDefault.F_TEST, feedback:true, 
            testSuites:new string[]{"suite1"}, label:"Static - Selected");
        static private IEnumerable GenNWiseFeedback = ConfigDefault.TestProvider.GenerateNWise(ConfigDefault.F_TEST, feedback:true, 
            label:"NWise / Feedback");

        [TearDown]
        public void TearDown()
        {
            TestHandle ecfeed = TestContext.CurrentContext.Test.Arguments[^1] as TestHandle; 
            
            ecfeed.addFeedback(
                TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed,
                comment:TestContext.CurrentContext.Result.Message
            );
        }

        [TestCaseSource("GenRandomQuantitySingle")]
        public void GenRandomQuantitySingleTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
        {
            Oracle.ValidateFTest(arg1, arg2, arg3, ecfeed);
        }

        [TestCaseSource("GenRandomQuantityShort")]
        public void GenRandomQuantityShortTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
        {
            Oracle.ValidateFTest(arg1, arg2, arg3, ecfeed);
        }

        [TestCaseSource("GenRandomQuantityLong")]
        public void GenRandomQuantityLongTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
        {
            Oracle.ValidateFTest(arg1, arg2, arg3, ecfeed);
        }

        [TestCaseSource("GenRandom")]
        public void GenRandomTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
        {
            Oracle.ValidateFTest(arg1, arg2, arg3, ecfeed);
        }

        [TestCaseSource("GenRandomAdaptive")]
        public void GenRandomAdaptiveTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
        {
            Oracle.ValidateFTest(arg1, arg2, arg3, ecfeed);
        }

        [TestCaseSource("GenRandomDuplicates")]
        public void GenRandomDuplicatesTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
        {
            Oracle.ValidateFTest(arg1, arg2, arg3, ecfeed);
        }

        [TestCaseSource("GenNWise")]
        public void GenNWiseTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
        {
            Oracle.ValidateFTest(arg1, arg2, arg3, ecfeed);
        }

        [TestCaseSource("GenNWiseN")]
        public void GenNWiseNTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
        {
            Oracle.ValidateFTest(arg1, arg2, arg3, ecfeed);
        }

        [TestCaseSource("GenNWiseCoverage")]
        public void GenNWiseCoverageTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
        {
            Oracle.ValidateFTest(arg1, arg2, arg3, ecfeed);
        }

        [TestCaseSource("GenNWiseConstraintsNone")]
        public void GenNWiseConstraintsNoneTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
        {
            Oracle.ValidateFTest(arg1, arg2, arg3, ecfeed);
        }

        [TestCaseSource("GenNWiseConstraintsSelected")]
        public void GenNWiseConstraintsSelectedTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
        {
            Oracle.ValidateFTest(arg1, arg2, arg3, ecfeed);
        }

        [TestCaseSource("GenNWiseChoicesSelected")]
        public void GenNWiseChoicesSelectedTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
        {
            Oracle.ValidateFTest(arg1, arg2, arg3, ecfeed);
        }

        [TestCaseSource("GenCartesian")]
        public void GenCartesianTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
        {
            Oracle.ValidateFTest(arg1, arg2, arg3, ecfeed);
        }

        [TestCaseSource("GenStatic")]
        public void GenStaticTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
        {
            Oracle.ValidateFTest(arg1, arg2, arg3, ecfeed);
        }

        [TestCaseSource("GenStaticAll")]
        public void GenStaticAllTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
        {
            Oracle.ValidateFTest(arg1, arg2, arg3, ecfeed);
        }

        [TestCaseSource("GenStaticSelected")]
        public void GenStaticSelectedTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
        {
            Oracle.ValidateFTest(arg1, arg2, arg3, ecfeed);
        }

        [TestCaseSource("GenNWiseFeedback")]
        public void GenNWiseFeedbackTest(int arg1, int arg2, int arg3, TestHandle ecfeed)
        {
            Oracle.ValidateFeedbackFTest(arg1, arg2, arg3, ecfeed);
        }
    }

}