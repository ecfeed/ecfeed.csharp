# Integration with C#

## Introduction

The following tutorial is an introduction to the C# runner. Note, that it does not cover the ecFeed basics. Therefore, if you want to learn how to create a sample model and generate a personal keystore, visit the tutorial section on our [webpage](https://ecfeed.com/tutorials).

Prerequisites:
- Install the [.NET](https://dotnet.microsoft.com/download) framework.
- Download an IDE. For example [Visual Studio Code](https://code.visualstudio.com/).
- Create a test model on the ecFeed webpage.
- Generate a personal keystore named 'security.p12' and put it in the \~/.ecfeed/
 directory (Linux users) or in the \~/ecfeed/ directory (Windows users).  

For the full documentation check the source directly at [GitHub](https://github.com/ecfeed/ecfeed.csharp).

## Installation

Inside a new folder open a terminal console and type 'dotnet new console'. The command creates a new C# project.  

The ecFeed library can be found online in the [NuGet repository](https://www.nuget.org/packages/EcFeed). To include it in the project, type 'dotnet add package ecfeed'.  

## Examples

Methods, used in the tutorial, are available in a welcome model, created during the registration process at the 'ecfeed.com' webpage. It the model is missing (e.g. it has been deleted by the user), it can be imported from [here](https://s3-eu-west-1.amazonaws.com/resources.ecfeed.com/repo/tutorial/Welcome.ect).

```C#
using System;
using EcFeed;

namespace Example
{
    class Runner
    {
        public static void Main(string[] args)
        {
            TestProvider testProvider = new TestProvider("8489-0551-2472-1941-3375");   // The model ID.
            foreach(var element in testProvider.ExportNWise("QuickStart.test"))         // The method name.
            {
                Console.WriteLine(element);
            }
        }
    }
}
```
To execute the code, type 'dotnet run' in the terminal.  

Don't hesitate to experiment with the code and modify the welcome model. It can be recreated easily and there is no better way to learn than hands-on exercises.  

However, have in mind that the ID of every model is unique. If you want to copy and paste the example, be sure to change it accordingly.  

## NUnit

The provided library can be used to create test cases for NUnit, which is one of the most common C# testing framework. It is possible, because generation methods return the 'IEnumerable<object[]>' interface, which might be directly used as the data source.    

Inside a new folder open the  terminal and type 'dotnet new nunit'. This command creates a new C# test project.

```C#
using System.Collections;
using NUnit.Framework;
using EcFeed;

namespace exampleNUnit
{
    [TestFixture]
    public class UnitTest
    {
        static public IEnumerable DataInt = new TestProvider("8489-0551-2472-1941-3375").GenerateNWise("QuickStart.test");

        [TestCaseSource("DataInt")]
        public void TestInt(int a0, int a1)
        {
            // ...
        }
    }

}
```

To run tests, type 'dotnet test' in the terminal.

# TestProvider class API

The library provides connectivity with the ecFeed test generation service using the 'TestProvider' class. It requires the model ID, the keystore location, the keystore password, and the generator service address.

## Constructor

The 'TestProvider' constructor takes one required and three optional arguments.

- *model (required)* - The model ID. It is a 20 digit number (grouped by 4) that can be found in the 'My projects' page at 'ecfeed.com'. It can be also found in an URL of the model editor page. It's value can be changed later using the 'Model' property. For example:
```C#
testProvider.Model = "XXXXX-XXXXX-XXXXX-XXXXX";
```
- *keyStorePath* - The path to a keystore downloaded from 'ecfeed.com' webpage ('Settings' -> 'Security'). The keystore contains the user certificate which is needed to authenticate the user at the generator service. By default, the constructor looks for the keystore in \~/.ecfeed/security.p12, except for Windows, where the default path is \~/ecfeed/security.p12.
- *keyStorePassword* - The password for the keystore. The default value is 'changeit' and this is the password used to encrypt the keystore downloaded from the 'ecfeed.com' page.
- *generatorAddress* - The URL of the ecfeed generator service. By default it is 'gen.ecfeed.com'.

Creating a TestProvider object can look like this:
```C#
TestProvider testProvider = new TestProvider("8489-0551-2472-1941-3375");
```

## Generator calls

'TestProvider' can invoke four methods to access the ecFeed generator service. They produce parsed, and streamed data. 

### public IEnumerable<object[]> GenerateNWise( ... )

Generate test cases using the NWise algorithm.  

Arguments:
- *method (required)* - The full name of the method that will be used for generation (including the namespace). If the method is not overloaded, its parameters are not required. 
- *n* - The 'N' value required in the NWise algorithm. The default is 2 (pairwise).
- *coverage* - The percentage of N-tuples that the generator will try to cover. The default is 100.
- *choices* - A dictionary in which keys are names of method parameters. Their values define a list of choices that should be used during the generation process. If an argument is skipped, all choices are used. For example:
```C#
Dictionary<string, string[]> choices = new Dictionary<string, string[]>();
choices.Add("arg1", new string[] {"choice1", "choice2"});
choices.Add("arg2", new string[] {"choice1"});
```
- *constraints* - A list of constraints used for the generation. If not provided, all constraints are used. For example: 
```C#
string[] constraints = new string[] { "constraint" };
```
Additionally, two string values can be used instead:
```C#
string constraints = "ALL";
string constraints = "NONE";
```

### public IEnumerable<object[]> GenerateCartesian( ... )

Generate test cases using the Cartesian product.  

Arguments:
- *method (required)* - See 'GenerateNWise'.
- *choices* - See 'GenerateNWise'.
- *constraints* - See 'GenerateNWise'.

### public IEnumerable<object[]> GenerateRandom( ... )

Generate randomized test cases.   

Arguments:
- *method (required)* - See 'GenerateNWise'.
- *length* - The number of tests to be generated. The default is 1.
- *duplicates* - If two identical tests are allowed to be generated. If set to 'false', the generator will stop after creating all allowed combinations. The default is 'true'. 
- *adaptive* - If set to true, the generator will try to provide tests that are farthest (in the means of the Hamming distance) from the ones already generated. The default is 'false'.
- *choices* - See 'GenerateNWise'.
- *constraints* - See 'GenerateNWise'.

### public IEnumerable<object[]> GenerateStatic( ... )

Download generated test cases (do not use the generator).

Arguments:
- *method (required)* - See 'GenerateNWise'.
- *testSuites* - A list of test case names to be downloaded. For example:
```C#
string[] testSuites = new string[] { "default" };
```
Additionally, one string value can be used instead:
```C#
string constraints = "ALL";
```

## Export calls

Those methods look similarly to 'Generate' methods. However, they return the 'IEnumerable<string>' interface, do not parse the data, and generate the output using templates. For this reason, they require one more argument, namely 'template'. It is located at the end argument list and predefined values are: 'Template.XML', 'Template.JSON', 'Template.Gherkin', 'Template CSV', and 'Template.Stream'. The default value is 'Template.CSV'.   
    
Have in mind that it is also possible to define a custom template. The instruction on how to do it can be found on the ecFeed webpage.  

The methods are as follows:
```C#
public IEnumerable<string> ExportNWise( ... , Template template);
public IEnumerable<string> ExportCartesian( ... , Template template);
public IEnumerable<string> ExportRandom( ... , Template template);
public IEnumerable<string> ExportStatic( ... , Template template);
```

## Other methods

The following section describes non-crucial methods.

### public string ValidateConnection()

Verifies if the connection settings (including the keystore) are correct. If something is wrong, an exception is thrown.

### public string[] GetMethodTypes(string method)

Gets the types of the method parameters in the on-line model.

### public string[] GetMethodNames(string method)

Gets the names of the method parameters in the on-line model.

## Feedback

To send feedback, you need to have a BASIC account type or be a member of a TEAM.  

An example looks as follows:
```C#
static internal TestProvider testProvider = new TestProvider("LRXC-015K-GJB0-2A9F-CGA2");
static internal string method = "com.example.test.Playground.size_10x10";

static public IEnumerable Method1a = testProvider.GenerateNWise(method1, feedback:true);

[TestCaseSource("Method1a")]
public void MethodTest(string a, string b, string c, string d, string e, string f, string g, string h, string i, string j, TestHandle testHandle)
{
    Assert.AreNotEqual(a, "a0"));
}

[TearDown]
public void TearDown()
{
    TestHandle ecfeed = TestContext.CurrentContext.Test.Arguments[^1] as TestHandle; 
            
    ecfeed.addFeedback(
        TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed,
        comment:TestContext.CurrentContext.Result.Message
    );
}
```

To the generation method an additional argument, i.e. 'TestHandle testHandle', must be added. The class consists of one public method, namely 'addFeedback'. The required argument denotes the result of the test, everything else is optional.  

```C#
testHandle.addFeedback(True, comment: 'Passed', duration: 1000, custom)
```

_status_ - The result of the test.
_comment_ - The optional description of the execution.
_duration_ - The optional execution time in milliseconds.
_custom_ - The optional dictionary of custom key-value pairs.

Note, that each test must return a feedback, regardless whether it has passed or failed. One solution to overcome this problem is to create a 'tear down' method, as in the example. However, it can also be done manually. Only the first execution of the 'addFeedback' takes effect. All subsequent executions are neglected.  

Additionally, to the test generation method one optional argument can be added, namely 'label'. It provides a short description of the generated test suite.  