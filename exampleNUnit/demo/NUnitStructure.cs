using System;
using System.Collections;
using NUnit.Framework;
using EcFeed;
using Source;

namespace exampleNUnit
{
    [TestFixture]
    public class NUnitStructureTest
    {
        static private IEnumerable GenRandomQuantitySingle = ConfigDefault.GetTestProvider(ConfigDefault.PROD).GenerateRandom(ConfigDefault.F_TEST, feedback:true,
            length:1, label:"Random / Quality - Single");

        static private IEnumerable GenRandomQualityStructureSingle = ConfigDefault.GetTestProvider(ConfigDefault.DEVELOP).GenerateRandom(ConfigDefault.F_STRUCTURE, feedback:true,
            length:1, typesDefinitionsSource: "Source");

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
            Console.WriteLine($"{arg1} {arg2} {arg3}");
            NUnit.Framework.TestContext.Progress.WriteLine($"{arg1} {arg2} {arg3}");
        }

        [TestCaseSource("GenRandomQualityStructureSingle")]
        public void GenRandomQuantitySingleStructureTest(Data a, int b, TestHandle ecfeed)
        {
            Console.WriteLine(a + ", " + b);
            NUnit.Framework.TestContext.Progress.WriteLine(a + ", " + b);
        }
    }

}