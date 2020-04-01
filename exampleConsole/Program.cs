using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EcFeed;
using NUnit.Framework;

namespace Testify.EcFeed.Example
{
    class Runner
    {
        public static async Task<int> Main(string[] args)
        {
            // SELECT ONE 
            // return await Synchronous();
            // return await Asynchronous();
            return await Event();
            // ExampleTestQueue();
            // ExampleTestList();

            return 0;
        }

        public static async Task<int> Synchronous()
        {
            ITestProvider testProvider = new TestProvider();
            testProvider.Model = "7482-5194-2849-1943-2448";
            testProvider.Method = "com.example.test.Demo.typeString";
            
            Console.WriteLine(testProvider); // DEBUG

            Console.WriteLine(await testProvider.GenerateNWise(template: Template.CSV));

            return 0;
        }

        public static async Task<int> Asynchronous()
        {
            ITestProvider testProvider = new TestProvider();
            testProvider.Model = "7482-5194-2849-1943-2448";
            testProvider.Method = "com.example.test.Demo.typeString";
            testProvider.Settings = new Dictionary<string, object> { { "dataSource", "genNWise" }, { "constraints", "NONE" } };

            Console.WriteLine(testProvider); // DEBUG

            Task<string> response = testProvider.Generate(template: Template.CSV);

            Console.WriteLine("\nWaiting...\n"); // DEBUG

            Console.WriteLine(await response);

            return 0;
        }

        public static async Task<int> Event()
        {
            ITestProvider testProvider = new TestProvider();
            testProvider.Model = "7482-5194-2849-1943-2448";
            testProvider.Method = "com.example.test.Demo.typeString";
            
            testProvider.AddTestEventHandler(TestEventHandler);
            testProvider.AddStatusEventHandler(StatusEventHandler);

            await testProvider.GenerateNWise(template: Template.Stream);

            return 0;
        }

        public static void ExampleTestQueue()
        {
            ITestProvider testProvider = new TestProvider();
            testProvider.Model = "7482-5194-2849-1943-2448";
            testProvider.Method = "com.example.test.Demo.typeString";

            TestQueue queue = testProvider.QueueNWise();

            Console.WriteLine(queue); // DEBUG
            Thread.Sleep(2000); // DEBUG
            Console.WriteLine(queue); // DEBUG

            foreach(TestCaseData element in queue)
            {
                Console.WriteLine("HANDLER: [{0}]", string.Join(", ", element.Arguments));
            }

            Console.WriteLine(queue); // DEBUG
        }

        public static void ExampleTestList()
        {
            ITestProvider testProvider = new TestProvider();
            testProvider.Model = "7482-5194-2849-1943-2448";
            testProvider.Method = "com.example.test.Demo.typeString";

            TestList list = testProvider.ListNWise();

            Console.WriteLine(list); // DEBUG
            list.WaitUntilFinished(); // DEBUG
            Console.WriteLine(list); // DEBUG

            Console.WriteLine(list[0]); // DEBUG

            foreach(TestEventArgs element in list)
            {
                Console.WriteLine("HANDLER: [{0}]", string.Join(", ", element));
            }

            Console.WriteLine(list); // DEBUG
        }

        static void TestEventHandler(object sender, TestEventArgs args)
        {
        //    SELECT ONE 
        //    Console.WriteLine("TEST HANDLER: " + args);
        //    Console.WriteLine("TEST HANDLER: " + args.DataRaw);
        //    Console.WriteLine("TEST HANDLER: " + args.DataType);
        //    Console.WriteLine("TEST HANDLER: " + args.Schema);
           Console.WriteLine("TEST HANDLER: [{0}]", string.Join(", ", args.DataTest.Arguments));
        }

        static void StatusEventHandler(object sender, StatusEventArgs args)
        {
           Console.WriteLine("STATUS HANDLER: " + args.Schema);
        }
    }
}