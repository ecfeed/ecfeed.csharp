using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Runtime;
using System.Collections.Generic;
using NUnit.Framework;
using EcFeed;

namespace exampleNUnit
{

    [TestFixture]
    public class NUnit3Default
    {
        public enum Gender { MALE, FEMALE }
        public enum Type { PASSPORT, DRIVERS_LICENSE, PERSONAL_ID }

        public (Gender, Type) DeconstructEnum(string gender, string type)
        {
            return
            (
                (Gender) Enum.Parse(typeof(Gender), gender),
                (Type) Enum.Parse(typeof(Type), type)
            );
        }

        static public IEnumerable SourceTestProviderNWise = ConfigDefault.GetTestProvider().GenerateNWise(ConfigDefault.F_LOAN_2);
        static public IEnumerable SourceTestProviderPairwise = ConfigDefault.GetTestProvider().GeneratePairwise(ConfigDefault.F_LOAN_2);
        static public IEnumerable SourceTestProviderCartesian = ConfigDefault.GetTestProvider().GenerateCartesian(ConfigDefault.F_LOAN_2);
        static public IEnumerable SourceTestProviderRandom = ConfigDefault.GetTestProvider().GenerateRandom(ConfigDefault.F_LOAN_2);
        static public IEnumerable SourceTestProviderStatic = ConfigDefault.GetTestProvider().GenerateStatic(ConfigDefault.F_LOAN_2);
        static public IEnumerable SourceTestProviderNWiseFeedback = ConfigDefault.GetTestProvider().GenerateNWise(ConfigDefault.F_LOAN_2, feedback: true, label: "C#");

        [TestCaseSource("SourceTestProviderNWise")]
        public void TestProviderNWise(string name, string firstName, string gender, int age, string id, string type)
        {
            (Gender dGender, Type dType) = DeconstructEnum(gender, type);
            NUnit.Framework.TestContext.Progress.WriteLine("name = " + name + ", firstName = " + firstName + ", gender = " + dGender + ", age = " + age + ", id = " + id + ", type = " + dType);
        }

        [TestCaseSource("SourceTestProviderPairwise")]
        public void TestProviderPairwise(string name, string firstName, string gender, int age, string id, string type)
        {
            (Gender dGender, Type dType) = DeconstructEnum(gender, type);
            NUnit.Framework.TestContext.Progress.WriteLine("name = " + name + ", firstName = " + firstName + ", gender = " + dGender + ", age = " + age + ", id = " + id + ", type = " + dType);
        }

        [TestCaseSource("SourceTestProviderCartesian")]
        public void TestProviderCartesian(string name, string firstName, string gender, int age, string id, string type)
        {
            (Gender dGender, Type dType) = DeconstructEnum(gender, type);
            NUnit.Framework.TestContext.Progress.WriteLine("name = " + name + ", firstName = " + firstName + ", gender = " + dGender + ", age = " + age + ", id = " + id + ", type = " + dType);
        }

        [TestCaseSource("SourceTestProviderRandom")]
        public void TestProviderRandom(string name, string firstName, string gender, int age, string id, string type)
        {
            (Gender dGender, Type dType) = DeconstructEnum(gender, type);
            NUnit.Framework.TestContext.Progress.WriteLine("name = " + name + ", firstName = " + firstName + ", gender = " + dGender + ", age = " + age + ", id = " + id + ", type = " + dType);
        }

        [TestCaseSource("SourceTestProviderStatic")]
        public void TestProviderStatic(string name, string firstName, string gender, int age, string id, string type)
        {
            (Gender dGender, Type dType) = DeconstructEnum(gender, type);
            NUnit.Framework.TestContext.Progress.WriteLine("name = " + name + ", firstName = " + firstName + ", gender = " + dGender + ", age = " + age + ", id = " + id + ", type = " + dType);
        }

        [TestCaseSource("SourceTestProviderNWiseFeedback")]
        public void TestProviderNWiseFeedback(string name, string firstName, string gender, int age, string id, string type, TestHandle testHandle)
        {
            (Gender dGender, Type dType) = DeconstructEnum(gender, type);
            NUnit.Framework.TestContext.Progress.WriteLine("name = " + name + ", firstName = " + firstName + ", gender = " + dGender + ", age = " + age + ", id = " + id + ", type = " + dType);

            if (dGender == Gender.FEMALE) {
                testHandle.AddFeedback(true, comment: "VALID");
            } else {
                testHandle.AddFeedback(false, comment: "INVALID");
            }

        }

        [Test]
        public void TestProviderExportTypeRaw()
        {
            foreach (string chunk in ConfigDefault.GetTestProvider().ExportNWise(ConfigDefault.F_LOAN_2, template: Template.Raw))
            {
                NUnit.Framework.TestContext.Progress.WriteLine(chunk);
            }
        }

        [Test]
        public void TestProviderExportTypeJson()
        {
            foreach (string chunk in ConfigDefault.GetTestProvider().ExportNWise(ConfigDefault.F_LOAN_2, template: Template.JSON))
            {
                NUnit.Framework.TestContext.Progress.WriteLine(chunk);
            }
        }

        [Test]
        public void TestProviderExportTypeXml()
        {
            foreach (string chunk in ConfigDefault.GetTestProvider().ExportNWise(ConfigDefault.F_LOAN_2, template: Template.XML))
            {
                NUnit.Framework.TestContext.Progress.WriteLine(chunk);
            }
        }

        [Test]
        public void TestProviderExportTypeCsv()
        {
            foreach (string chunk in ConfigDefault.GetTestProvider().ExportNWise(ConfigDefault.F_LOAN_2, template: Template.CSV))
            {
                NUnit.Framework.TestContext.Progress.WriteLine(chunk);
            }
        }

        [Test]
        public void TestProviderExportTypeGherkin()
        {
            foreach (string chunk in ConfigDefault.GetTestProvider().ExportNWise(ConfigDefault.F_LOAN_2, template: Template.Gherkin))
            {
                NUnit.Framework.TestContext.Progress.WriteLine(chunk);
            }
        }

        [Test]
        public void TestNWiseParam()
        {
            string[] constraints = new string[]{ "gender" };

            Dictionary<string, string[]> choices = new Dictionary<string, string[]>();
            choices["firstName"] = new string[]{ "male:short" };

            foreach (object[] chunk in ConfigDefault.GetTestProvider().GenerateNWise(ConfigDefault.F_LOAN_2, constraints: constraints, choices: choices, coverage: 100, n: 3))
            {
                NUnit.Framework.TestContext.Progress.WriteLine(chunk.Select(e => e.ToString()).Aggregate("", (acc, x) => acc + "," + x).Substring(1));
            }

            foreach (string chunk in ConfigDefault.GetTestProvider().ExportNWise(ConfigDefault.F_LOAN_2, constraints: constraints, choices: choices, coverage: 100, n: 3, template: Template.CSV))
            {
                NUnit.Framework.TestContext.Progress.WriteLine(chunk);
            }
        }

        [Test]
        public void TestPairwiseParam()
        {
            string[] constraints = new string[]{ "gender" };

            Dictionary<string, string[]> choices = new Dictionary<string, string[]>();
            choices["firstName"] = new string[]{ "male:short" };

            foreach (object[] chunk in ConfigDefault.GetTestProvider().GeneratePairwise(ConfigDefault.F_LOAN_2, constraints: constraints, choices: choices, coverage: 100))
            {
                NUnit.Framework.TestContext.Progress.WriteLine(chunk.Select(e => e.ToString()).Aggregate("", (acc, x) => acc + "," + x).Substring(1));
            }

            foreach (string chunk in ConfigDefault.GetTestProvider().ExportPairwise(ConfigDefault.F_LOAN_2, constraints: constraints, choices: choices, coverage: 100, template: Template.CSV))
            {
                NUnit.Framework.TestContext.Progress.WriteLine(chunk);
            }
        }

        [Test]
        public void TestPairwiseCartesian()
        {
            string[] constraints = new string[]{ "gender" };

            Dictionary<string, string[]> choices = new Dictionary<string, string[]>();
            choices["firstName"] = new string[]{ "male:short" };

            foreach (object[] chunk in ConfigDefault.GetTestProvider().GenerateCartesian(ConfigDefault.F_LOAN_2, constraints: constraints, choices: choices))
            {
                NUnit.Framework.TestContext.Progress.WriteLine(chunk.Select(e => e.ToString()).Aggregate("", (acc, x) => acc + "," + x).Substring(1));
            }

            foreach (string chunk in ConfigDefault.GetTestProvider().ExportCartesian(ConfigDefault.F_LOAN_2, constraints: constraints, choices: choices, template: Template.CSV))
            {
                NUnit.Framework.TestContext.Progress.WriteLine(chunk);
            }
        }

        [Test]
        public void TestPairwiseRandom()
        {
            string[] constraints = new string[]{ "gender" };

            Dictionary<string, string[]> choices = new Dictionary<string, string[]>();
            choices["firstName"] = new string[]{ "male:short" };

            foreach (object[] chunk in ConfigDefault.GetTestProvider().GenerateRandom(ConfigDefault.F_LOAN_2, constraints: constraints, choices: choices, length: 25, adaptive: true, duplicates: true))
            {
                NUnit.Framework.TestContext.Progress.WriteLine(chunk.Select(e => e.ToString()).Aggregate("", (acc, x) => acc + "," + x).Substring(1));
            }

            foreach (string chunk in ConfigDefault.GetTestProvider().ExportRandom(ConfigDefault.F_LOAN_2, constraints: constraints, choices: choices, length: 25, adaptive: true, duplicates: true, template: Template.CSV))
            {
                NUnit.Framework.TestContext.Progress.WriteLine(chunk);
            }
        }

        [Test]
        public void TestPairwiseStatic()
        {
            string[] testSuites = new string[]{ "default suite" };

            foreach (object[] chunk in ConfigDefault.GetTestProvider().GenerateStatic(ConfigDefault.F_LOAN_2, testSuites: testSuites))
            {
                NUnit.Framework.TestContext.Progress.WriteLine(chunk.Select(e => e.ToString()).Aggregate("", (acc, x) => acc + "," + x).Substring(1));
            }

            foreach (string chunk in ConfigDefault.GetTestProvider().ExportStatic(ConfigDefault.F_LOAN_2, testSuites: testSuites, template: Template.CSV))
            {
                NUnit.Framework.TestContext.Progress.WriteLine(chunk);
            }
        }

        [Test]
        public void TestGetMethodTypes()
        {
            NUnit.Framework.TestContext.Progress.WriteLine(ConfigDefault.GetTestProvider().GetMethodTypes(ConfigDefault.F_LOAN_2).Aggregate("", (acc, x) => acc + "," + x).Substring(1));
        }

        [Test]
        public void TestGetMethodNames()
        {
            NUnit.Framework.TestContext.Progress.WriteLine(ConfigDefault.GetTestProvider().GetMethodNames(ConfigDefault.F_LOAN_2).Aggregate("", (acc, x) => acc + "," + x).Substring(1));
        }

        [Test]
        public void TestValidate()
        {
            ConfigDefault.GetTestProvider().Validate();
        }

        [Test]
        public void GetModel()
        {
            Assert.IsTrue(ConfigDefault.GetTestProvider().Model.Equals(ConfigDefault.MODEL_DEVELOP) || ConfigDefault.GetTestProvider().Model.Equals(ConfigDefault.MODEL_PROD));
        }

        [Test]
        public void GetModelCustom()
        {
            var testProvider = new TestProvider("TestModel");

            Assert.AreEqual(testProvider.Model, "TestModel");
        }

        [Test]
        public void GetGeneratorAddress()
        {
            Assert.IsTrue("https://gen.ecfeed.com".Equals(ConfigDefault.GetTestProvider().GeneratorAddress) || "https://develop-gen.ecfeed.com".Equals(ConfigDefault.GetTestProvider().GeneratorAddress));
        }

        [Test]
        public void GetGeneratorAddressCustom()
        {
            var testProvider = new TestProvider(ConfigDefault.MODEL_DUMMY, generatorAddress: "testAddress");

            Assert.AreEqual(testProvider.GeneratorAddress, "testAddress");
        }

        [Test]
        public void GetKeyStore()
        {
            Assert.IsTrue(ConfigDefault.GetTestProvider().KeyStorePath.EndsWith(".p12"));
        }

        [Test]
        public void GetKeyStorePathCustom()
        {
            var testProvider = new TestProvider(ConfigDefault.MODEL_DUMMY, keyStorePath: "testKeyStorePath");

            Assert.AreEqual(testProvider.KeyStorePath, "testKeyStorePath");
        }

        [Test]
        public void GetKeyStorePassword()
        {
            Assert.AreEqual(ConfigDefault.GetTestProvider().KeyStorePassword, "changeit");
        }

        [Test]
        public void GetKeyStorePasswordCustom()
        {
            var testProvider = new TestProvider(ConfigDefault.MODEL_DUMMY, keyStorePassword: "testKeyStorePassword");

            Assert.AreEqual(testProvider.KeyStorePassword, "testKeyStorePassword");
        }

        [Test]
        public void ErrorModelCustom()
        {
            var testProvider = new TestProvider("testModel");

            Assert.That(() => 
            {
                foreach (object[] chunk in testProvider.GeneratePairwise(ConfigDefault.F_LOAN_2))
                {
                    NUnit.Framework.TestContext.Progress.WriteLine(chunk.Select(e => e.ToString()).Aggregate("", (acc, x) => acc + "," + x).Substring(1));
                }
            }, Throws.Exception);
        }

        [Test]
        public void ErrorGeneratorAddressCustom()
        {
            var testProvider = new TestProvider(ConfigDefault.MODEL_DUMMY, generatorAddress: "testAddress");

            Assert.That(() => 
            {
                foreach (object[] chunk in testProvider.GeneratePairwise(ConfigDefault.F_LOAN_2))
                {
                    NUnit.Framework.TestContext.Progress.WriteLine(chunk.Select(e => e.ToString()).Aggregate("", (acc, x) => acc + "," + x).Substring(1));
                }
            }, Throws.Exception);
        }

        [Test]
        public void ErrorKeyStorePathCustom()
        {
            var testProvider = new TestProvider(ConfigDefault.MODEL_DUMMY, keyStorePath: "testKeyStorePath");

            Assert.That(() => 
            {
                foreach (object[] chunk in testProvider.GeneratePairwise(ConfigDefault.F_LOAN_2))
                {
                    NUnit.Framework.TestContext.Progress.WriteLine(chunk.Select(e => e.ToString()).Aggregate("", (acc, x) => acc + "," + x).Substring(1));
                }
            }, Throws.Exception);
        }

        [Test]
        public void ErrorKeyStorePasswordCustom()
        {
            var testProvider = new TestProvider(ConfigDefault.MODEL_DUMMY, keyStorePassword: "testKeyStorePassword");

            Assert.That(() => 
            {
                foreach (object[] chunk in testProvider.GeneratePairwise(ConfigDefault.F_LOAN_2))
                {
                    NUnit.Framework.TestContext.Progress.WriteLine(chunk.Select(e => e.ToString()).Aggregate("", (acc, x) => acc + "," + x).Substring(1));
                }
            }, Throws.Exception);
        }

        [Test]
        public void ErrorMethodCustom()
        {
            Assert.That(() => 
            {
                foreach (object[] chunk in ConfigDefault.GetTestProvider().GeneratePairwise("testMethod"))
                {
                    NUnit.Framework.TestContext.Progress.WriteLine(chunk.Select(e => e.ToString()).Aggregate("", (acc, x) => acc + "," + x).Substring(1));
                }
            }, Throws.Exception);
        }

        [Test]
        public void ErrorClientCertificate()
        {
            var testProvider = new TestProvider(ConfigDefault.MODEL_DEVELOP, keyStorePath: @"../../../resources/securityNoClient.p12");

            Assert.That(() => 
            {
                foreach (object[] chunk in testProvider.GeneratePairwise(ConfigDefault.F_LOAN_2))
                {
                    NUnit.Framework.TestContext.Progress.WriteLine(chunk.Select(e => e.ToString()).Aggregate("", (acc, x) => acc + "," + x).Substring(1));
                }
            }, Throws.Exception);
        }

        [Test]
        public void ErrorServerCertificate()
        {
            var testProvider = new TestProvider(ConfigDefault.MODEL_DEVELOP, keyStorePath: @"../../../resources/securityNoServer.p12");

            Assert.That(() => 
            {
                foreach (object[] chunk in testProvider.GeneratePairwise(ConfigDefault.F_LOAN_2))
                {
                    NUnit.Framework.TestContext.Progress.WriteLine(chunk.Select(e => e.ToString()).Aggregate("", (acc, x) => acc + "," + x).Substring(1));
                }
            }, Throws.Exception);
        }
       
    }

}