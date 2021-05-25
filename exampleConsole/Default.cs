
using System;
using System.Collections.Generic;
using EcFeed;

namespace Testify.EcFeed.Example
{
    class Runner
    {
        public static void Main(string[] args)
        {
            Console.WriteLine($"{ string.Join(", ", ConfigDefault.TestProvider.GetMethodNames(ConfigDefault.F_TEST)) }");
            Console.WriteLine($"{ string.Join(", ", ConfigDefault.TestProvider.GetMethodTypes(ConfigDefault.F_TEST)) }");

            Dictionary<string, string[]> testChoices = new Dictionary<string, string[]>();
            testChoices.Add("arg1", new string[] {"choice1", "choice2"});
            testChoices.Add("arg2", new string[] {"choice1"});

            string[] testConstraints = new string[] { "constraint" };

            string[] testSuites = new string[] { "default" };

            // foreach(var element in ConfigDefault.TestProvider.ExportStatic(ConfigDefault.F_TEST, testSuites: testSuites, template: Template.JSON))
            foreach(object[] element in ConfigDefault.TestProvider.GenerateNWise(ConfigDefault.F_TEST, feedback:true))
            {
                // NUnit.Framework.TestContext.Progress.WriteLine(httpWebResponse.StatusDescription);
                TestHandle testData = (TestHandle)element[element.Length - 1];
                testData.addFeedback(true, custom:new Dictionary<string, string>{{"first", "uno"}, {"second", "dos"}});
                Console.WriteLine("HANDLER: [{0}]", string.Join(", ", element));
                
            }

        }

    }
}