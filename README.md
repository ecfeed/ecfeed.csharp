# Integration with C#

## Introduction

The following tutorial is an introduction to the C# runner. Note, that it does not cover the ecFeed basics. Therefore, if you want to learn how to create a sample model and generate a personal keystore, visit the tutorial section on our [webpage](https://ecfeed.com/tutorials).

Prerequisites:
- Install the [.NET](https://dotnet.microsoft.com/download) framework.
- Download an IDE. For example [Visual Studio Code](https://code.visualstudio.com/).
- Create a test model on the ecFeed webpage.
- Generate a personal keystore named 'security.p12' and put it in the '~/.ecfeed/' directory (Linux) or int the '~/ecfeed/' directory (Windows).  

For the full documentation check the source directly at [GitHub](https://github.com/ecfeed/ecfeed.csharp).

## Installation

Inside a new folder open a terminal console and type 'dotnet new console'. The command creates a new C# project.  

The ecFeed library can be found online in the [NuGet repository](https://www.nuget.org/packages/EcFeed). To include it in the project, type 'dotnet add package ecfeed'.  

## Examples

The methods, used in the tutorial, should be available in a welcome model, created during the registration process at 'ecfeed.com'. It the model is not there, it can be imported from [here](https://s3-eu-west-1.amazonaws.com/resources.ecfeed.com/repo/tutorial/Welcome.ect).

```C#
using System;
using EcFeed;

namespace Example
{
    class Runner
    {
        public static void Main(string[] args)
        {
// The required argument is the model ID. 
            TestProvider testProvider = new TestProvider("8489-0551-2472-1941-3375");
// The required argument is the method name.
            foreach(var element in testProvider.ExportNWise("QuickStart.test"))
            {
                Console.WriteLine(element);
            }
        }
    }
}
```
To execute the code, type 'dotnet run' in the terminal.  

To check the connection settings and validate the keystore, you can use the following code. If something is wrong, an exception with a proper message will be thrown.
```C#
testProvider.ValidateConnection();
```

To check types or names of the method arguments, you can use the following snippet:
```C#
Console.WriteLine($"{ string.Join(", ", testProvider.GetMethodNames("QuickStart.test")) }");
Console.WriteLine($"{ string.Join(", ", testProvider.GetMethodTypes("QuickStart.test")) }");
```

## NUnit

The C# library can be used to create test cases for the NUnit testing framework.  

Inside a new folder open a terminal console and type 'dotnet new nunit'. This command creates a new C# project.

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

To execute tests, type 'dotnet test' in the terminal.

# TestProvider class API

The library provides connectivity with the ecFeed test generation service with the 'TestProvider' class. It requires the model ID (generated at the 'ecfeed.com webpage), the keystore (generated at the 'ecfeed.com' webpage), the keystore password, and the generator address.

## Constructor

The 'TestProvider' constructor takes one required argument and three optional arguments.

- *model (required)* - The model ID. It is a 20 digit number (grouped by 4) that can be found in the 'My projects' page at 'ecfeed.com'. It is also in an URL of the model editor page. It's value can be changed later using the 'Model' property.
- *keyStorePath* - The path to a keystore downloaded from 'ecfeed.com' webpage ('Settings' -> 'Security'). The keystore contains user's certificate that is used to identify and authenticate the user at the generator service. Also, it contains the generator's public key to validate its certificate. By default, the constructor looks for the keystore in '~/.ecfeed/security.p12', except for Windows, where the default path is '~/ecfeed/security.p12'.
- *keyStorePassword* - The keystore password. The default value is 'changeit' and this is the password used to encrypt the keystore downloaded from the 'ecfeed.com' page. Therefore, if it wasn't changed, the default value should be fine.
- *generatorAddress* - The URL to the ecfeed generator service. By default it is 'gen.ecfeed.com' and this should work with most cases.

An example call to construct a TestProvider object can look like this:
```C#
TestProvider testProvider = new TestProvider("8489-0551-2472-1941-3375");
```

## Generator calls

'TestProvider' can invoke four functions to access the ecFeed generator service. They produce parsed, and streamed data. 

### public IEnumerable<object[]> GenerateNWise( ... )

Generate test cases using the NWise algorithm.  

Arguments:
- *method (required)* - The full name of the method that will be used for generation. If it is not overloaded, the parameters are not required. 
- *n* - The 'N' value required in the NWise algorithm. The default is 2.
- *coverage* - The percentage of N-tuples that the generator will try to cover. The default is 100.
- *choices* - A dictionary. The keys are names of method parameters. The values define a list of choices that will be used during the generation process. If an argument is skipped in the dictionary, all defined choices will be used. For example:
```C#
Dictionary<string, string[]> choices = new Dictionary<string, string[]>();
choices.Add("arg1", new string[] {"choice1", "choice2"});
choices.Add("arg2", new string[] {"choice1"});
```
- *constraints* - A list of constraints used for the generation. If not provided, all constraints will be used. For example: 
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
- *duplicates* - If two identical tests are allowed to be generated. If set to 'false', the generator will stop after all allowed. The default is 'true'. 
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

Those methods look similarly to 'Generate' methods. However, they return the 'IEnumerable<object[]>' interface, do not parse the data, and generate the output using custom templates. For this reason, they require one more argument, namely 'template'. It is located on the last position in the argument list and predefined values are: 'Template.XML', 'Template.JSON', 'Template.Gherkin', 'Template CSV', and 'Template.Stream'. The default value is 'Template.CSV'.  

The methods are as follows:
```C#
public IEnumerable<string> ExportNWise( ... , Template template);
public IEnumerable<string> ExportCartesian( ... , Template template);
public IEnumerable<string> ExportRandom( ... , Template template);
public IEnumerable<string> ExportStatic( ... , Template template);
```

## Other methods

### public string ValidateConnection()

Verifies if the connection settings (including the keystore) are correct. If something is wrong, an exception is thrown. The resulting string defines the build number of the generator and can be used for debugging purposes.

### public string[] GetMethodTypes(string method)

Gets the types of the method parameters in the on-line model.

### public string[] GetMethodNames(string method)

Gets the names of the method parameters in the on-line model.