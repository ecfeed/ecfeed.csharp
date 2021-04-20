﻿
using System;
using System.Collections.Generic;
using EcFeed;

namespace Testify.EcFeed.Example
{
    class Runner
    {
        public static void Main(string[] args)
        {
            TestProvider testProvider = new TestProvider("3243-2107-7355-4290-3889");
            
            string method ="QuickStart.test";

            Console.WriteLine($"{ string.Join(", ", testProvider.GetMethodNames(method)) }");
            Console.WriteLine($"{ string.Join(", ", testProvider.GetMethodTypes(method)) }");

            Dictionary<string, string[]> testChoices = new Dictionary<string, string[]>();
            testChoices.Add("arg1", new string[] {"choice1", "choice2"});
            testChoices.Add("arg2", new string[] {"choice1"});

            string[] testConstraints = new string[] { "constraint" };

            string[] testSuites = new string[] { "default" };

            // foreach(var element in testProvider.ExportStatic(method, testSuites: testSuites, template: Template.JSON))
            foreach(object[] element in testProvider.GenerateNWise(method, feedback:true))
            {
                // Console.WriteLine("HANDLER: {0}", element);
                TestData testData = (TestData)element[element.Length - 1];
                testData.feedback(true);
                Console.WriteLine("HANDLER: [{0}]", string.Join(", ", element));
                
            }

        }

    }
}