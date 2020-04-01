# Overview

The following tutorial is an introduction to the C# runner. Note, that it does not cover the ecFeed basics. Therefore, if you want to learn how to create a  sample model and generate a personal keystore, visit the tutorial section on our [webpage](https://ecfeed.com/tutorials).

Prerequisites:
- Install the [.NET](https://dotnet.microsoft.com/download) framework.
- Download an IDE. For example [Visual Studio Code](https://code.visualstudio.com/).
- Create a test model on the ecFeed webpage.
- Generate a personal keystore named 'security.p12' and put it in the '~/.ecfeed/' directory.

# Data generation

Inside a new folder open a terminal console and type 'dotnet new console'. This command creates a new C# project.  

The ecFeed library can be found online in the [NuGet repository](https://www.nuget.org/packages/EcFeed). To include it in the project, type 'dotnet add package ecfeed --version 1.0.0-beta'.  

## Synchronous provider

The easiest way to generate tests is to use the synchronous approach. Edit the default 'Program.cs' file which should look as follows:
```C#
using System;
using System.Threading.Tasks;
using EcFeed;

namespace Example
{
    class Runner
    {
        static async Task<int> Main(string[] args)
        {
// It is possible to change the connection settings, e.g. the generator address, using named parameters.
// It can be also done afterwards with predefined getters and setters.
            ITestProvider testProvider = new TestProvider();
// Provide the model ID (required).
            testProvider.Model = "7482-5194-2849-1943-2448";
// Provide the method name (required). It is not necessary to specify the argument list if the method is not overloaded.
            testProvider.Method = "com.example.test.Demo.typeString";
// Generate tests. Various generator types (with different named parameters) and templates are available.
            Console.WriteLine(await testProvider.GenerateNWise(template: Template.CSV));

            return 0;
        }
    }
}
```
To execute the code, type 'dotnet run' in the terminal.

## Asynchronous provider

The code is similar to the previous one. The only difference is that the generator runs in the background. Also, new methods are introduced (which are entirely optional).
```C#
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using EcFeed;

namespace Example
{
    class Runner
    {
        static async Task<int> Main(string[] args)
        {
            ITestProvider testProvider = new TestProvider();
            testProvider.Model = "7482-5194-2849-1943-2448";
            testProvider.Method = "com.example.test.Demo.typeString";
// It is possible to define custom generator settings instead of using predefined methods.
// The arguments are the same for every runner and their list can be found on the official webpage in the 'tutorials' section.
            testProvider.Settings = new Dictionary<string, object> { { "dataSource", "genNWise" }, { "constraints", "NONE" } };
// Validate connection settings. Invoking this method is optional and recommended only after changing the default connection settings.
            testProvider.ValidateConnectionSettings();
// Generate tests in the background.
            Task<string> response = testProvider.Generate(template: Template.CSV);
// Continue with the code execution.
            Console.WriteLine("\Running...\n");
// Wait for the generation to finish.
            Console.WriteLine(await response);

            return 0;
        }
    }
}
```

## Event handler

This approach uses event handlers what makes the code much more flexible.
```C#
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using EcFeed;

namespace Example
{
    class Runner
    {
        static async Task<int> Main(string[] args)
        {
            ITestProvider testProvider = new TestProvider();
            testProvider.Model = "7482-5194-2849-1943-2448";
            testProvider.Method = "com.example.test.Demo.typeString";
// Add a test handler.            
            testProvider.AddTestEventHandler(TestEventHandler);
// Add an optional status handler. Use it if you want to process detailed information about the test metadata.
            testProvider.AddStatusEventHandler(StatusEventHandler);
// The 'Stream' template ensures that each test is handled immediately after generation.
            await testProvider.GenerateNWise(template: Template.Stream);

            return 0;
        }

        static void TestEventHandler(object sender, TestEventArgs args)
        {
// The 'TestEventArgs' structure contains the test data in various formats.
           Console.WriteLine("TEST HANDLER: [{0}]", string.Join(", ", args.DataTest.Arguments));
        }

        static void StatusEventHandler(object sender, StatusEventArgs args)
        {
           Console.WriteLine("STATUS HANDLER: " + args.Schema);
        }
    }
}
```

## Test queue

The object represents a blocking (FIFO) queue, which can be easily integrated with the NUnit framework. It is destructive, what means the 'foreach' loop can be executed only once, i.e. each test case can be accessed only once.
```C#
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using EcFeed;

namespace Example
{
    class Runner
    {
        static async Task<int> Main(string[] args)
        {
            ITestProvider testProvider = new TestProvider();
            testProvider.Model = "7482-5194-2849-1943-2448";
            testProvider.Method = "com.example.test.Demo.typeString";
// Create the queue and start the generation process.
// Similarly to the 'Generate' method. You can use various predefined generators, e.g. 'QueueCartesian()', with named parameters.
            TestQueue queue = testProvider.QueueNWise();
// The object can be used in the 'foreach' loop. The code waits for each test that is not ready yet.
            foreach(TestCaseData element in queue)
            {
                Console.WriteLine("HANDLER: [{0}]", string.Join(", ", element.Arguments));
            }
        }
    }
}
```

## Test list

The object is very similar to the queue, but it is not destructive and does not remove test cases after use. It should be used as a collection of test cases (which size can dynamically increase over time).
```C#
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using EcFeed;

namespace Example
{
    class Runner
    {
        static async Task<int> Main(string[] args)
        {
            ITestProvider testProvider = new TestProvider();
            testProvider.Model = "7482-5194-2849-1943-2448";
            testProvider.Method = "com.example.test.Demo.typeString";

            TestList list = testProvider.ListNWise();
// Wait until all tests are generated. The invocation of the method is optional.
            list.WaitUntilFinished();
// Access an element.
            Console.WriteLine(list[0]);

            foreach(TestEventArgs element in list)
            {
                Console.WriteLine("HANDLER: [{0}]", string.Join(", ", element));
            }
        }
    }
}
```

# Integration with NUnit

Inside a new folder open a terminal console and type 'dotnet new nunit'. This command creates a new C# project.  

The ecFeed library can be found online in the [NuGet repository](https://www.nuget.org/packages/EcFeed). To include it in the project, type 'dotnet add package ecfeed --version 1.0.0-beta'.  

Modify the UnitTest1.cs file:
```C#
using System.Collections;
using NUnit.Framework;
using EcFeed;

namespace exampleNUnit
{
    [TestFixture]
    public class UnitTest
    {
        [TestCaseSource(typeof(FeedDefault))]
        public void Test(string a0, string a1, string a2, string a3, string a4, string a5, string a6, string a7, string a8, string a9, string a10)
        {
            // ...
        }
    }

    class FeedDefault : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
// Create a new test provider.
            ITestProvider testProvider = new TestProvider();
            testProvider.Model = "7482-5194-2849-1943-2448";
            testProvider.Method = "com.example.test.Demo.typeString";
// Create a new queue and return its enumerator.
            return testProvider.QueueNWise().GetEnumerator();
        }
    }
}
```
To execute tests, type 'dotnet test' in the terminal.