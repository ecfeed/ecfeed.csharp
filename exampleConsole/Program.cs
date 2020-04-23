
using System;
using System.Collections.Generic;
using EcFeed;

namespace Testify.EcFeed.Example
{
    class Runner
    {
        public static void Main(string[] args)
        {
            TestProvider testProvider = new TestProvider("8489-0551-2472-1941-3375");

            string method ="QuickStart.test";

            Console.WriteLine($"{ string.Join(", ", testProvider.GetMethodNames(method)) }");
            Console.WriteLine($"{ string.Join(", ", testProvider.GetMethodTypes(method)) }");

            Dictionary<string, string[]> testChoices = new Dictionary<string, string[]>();
            testChoices.Add("arg1", new string[] {"choice1", "choice2"});
            testChoices.Add("arg2", new string[] {"choice1"});

            string[] testConstraints = new string[] { "constraint" };

            string[] testSuites = new string[] { "default" };

            foreach(var element in testProvider.ExportCartesian(method, template: Template.JSON))
            {
                Console.WriteLine("HANDLER: {0}", element);
                // Console.WriteLine("HANDLER: [{0}]", string.Join(", ", element));
            }

        }

    }
}