using System.IO;
using System.Text;
using System.Net;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace EcFeed
{
    public class TestProvider
    {
        static TestProvider()
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
        }

        public string Model { get; set; }

        public string KeyStorePath { get; private set; }
        public string KeyStorePassword { get; private set; }
        public string GeneratorAddress { get; private set; }

        private StructureInitializer Initializer { get; set; }

        public TestProvider(string model, string keyStorePath = null, string keyStorePassword = null, string generatorAddress = null)
        {
            Model = model;

            Initializer = Factory.GetStructureInitializer();

            KeyStorePath = SetDefaultKeyStorePath(keyStorePath);
            KeyStorePassword = string.IsNullOrEmpty(keyStorePassword) ? Default.KeyStorePassword : keyStorePassword;;
            GeneratorAddress = string.IsNullOrEmpty(generatorAddress) ? Default.GeneratorAddress : generatorAddress;
        }

        private string SetDefaultKeyStorePath(string keyStorePath)
        {
            if (!string.IsNullOrEmpty(keyStorePath))
            {
                if (File.Exists(keyStorePath))
                {
                    return keyStorePath;
                }
            }

            foreach (string path in Default.KeyStorePath)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            throw new TestProviderException("The keystore path could not be verified. In order to use the test generator, please provide a correct path.");
        }

//-------------------------------------------------------------------------------------------

        public string[] GetMethodTypes(string method)
        {
            return FetchMethodInfo(method)[0];
        }

        public string[] GetMethodNames(string method)
        {
            return FetchMethodInfo(method)[1];
        }

        private string[][] FetchMethodInfo(string method)
        {
            DataGeneratorProperties generatorProperties = new DataGeneratorProperties();
            generatorProperties.Set(RequestTestParameter.Length, "1");

            DataGenerator generatorOptions = new DataGenerator(generatorProperties);
            generatorOptions.Set(RequestTestParameter.DataSource, Generator.Random.GetValue());

            DataSession sessionData = new DataSession();
            sessionData.GeneratorData = generatorOptions;
            sessionData.ModelId = Model;
            sessionData.MethodName = method;

            IEnumerable<string[][]> queue = Process<string[][]>(sessionData);

            foreach(string[][] element in queue) 
            {
                return element;
            };

            return null;
        }

//-------------------------------------------------------------------------------------------

        internal IEnumerable<string> Export(
            string method,
            Generator generator,
            DataGenerator generatorOptions,
            string model = null,
            Dictionary<string, string> custom = null,
            string label = null,
            bool feedback = false,
            Template template = Default.ParameterTemplate)
        {
            generatorOptions.Set(RequestTestParameter.DataSource, generator.GetValue());

            DataSession sessionData = new DataSession();
            sessionData.GeneratorData = generatorOptions;
            sessionData.ModelId = model == null ? Model : model;
            sessionData.MethodName = method;
            sessionData.Template = template;
            sessionData.Custom = custom;
            sessionData.BuildFeedback = feedback;
            sessionData.TestSessionLabel = label;
            sessionData.GeneratorAddress = GeneratorAddress;

            return Process<string>(sessionData);
        }

        public IEnumerable<string> ExportNWise(
            string method,
            int n = Default.ParameterN, 
            int coverage = Default.ParameterCoverage,
            Dictionary<string, string[]> choices = null,
            object constraints = null,
            string model = null,
            Dictionary<string, string> custom = null,
            string label = null,
            bool feedback = false,
            Template template = Default.ParameterTemplate)
        {
            DataGeneratorProperties generatorProperties = new DataGeneratorProperties();
            generatorProperties.Set(RequestTestParameter.N, "" + n);
            generatorProperties.Set(RequestTestParameter.Coverage, "" + coverage);

            DataGenerator generatorOptions = new DataGenerator(generatorProperties);
            generatorOptions.Set(RequestTestParameter.DataSource, Generator.NWise.GetValue());
            generatorOptions.Set(RequestTestParameter.Choices, choices);
            generatorOptions.Set(RequestTestParameter.Constraints, constraints);

            DataSession sessionData = new DataSession();
            sessionData.GeneratorData = generatorOptions;
            sessionData.ModelId = model == null ? Model : model;
            sessionData.MethodName = method;
            sessionData.Template = template;
            sessionData.Custom = custom;
            sessionData.BuildFeedback = feedback;
            sessionData.TestSessionLabel = label;
            sessionData.Constraints = constraints;
            sessionData.Choices = choices;
            sessionData.GeneratorAddress = GeneratorAddress;

            return Process<string>(sessionData);
        }

        public IEnumerable<string> ExportCartesian(
            string method,
            Dictionary<string, string[]> choices = null,
            object constraints = null,
            string model = null,
            Dictionary<string, string> custom = null,
            string label = null,
            bool feedback = false,
            Template template = Default.ParameterTemplate)
        {
            DataGeneratorProperties generatorProperties = new DataGeneratorProperties();

            DataGenerator generatorOptions = new DataGenerator(generatorProperties);
            generatorOptions.Set(RequestTestParameter.DataSource, Generator.Cartesian.GetValue());
            generatorOptions.Set(RequestTestParameter.Choices, choices);
            generatorOptions.Set(RequestTestParameter.Constraints, constraints);

            DataSession sessionData = new DataSession();
            sessionData.GeneratorData = generatorOptions;
            sessionData.ModelId = model == null ? Model : model;
            sessionData.MethodName = method;
            sessionData.Template = template;
            sessionData.Custom = custom;
            sessionData.BuildFeedback = feedback;
            sessionData.TestSessionLabel = label;
            sessionData.Constraints = constraints;
            sessionData.Choices = choices;
            sessionData.GeneratorAddress = GeneratorAddress;

            return Process<string>(sessionData);
        }

        public IEnumerable<string> ExportRandom(
            string method,
            int length = Default.ParameterLength, 
            bool duplicates = Default.ParameterDuplicates,
            bool adaptive = Default.ParameterAdaptive,
            Dictionary<string, string[]> choices = null,
            object constraints = null,
            string model = null,
            Dictionary<string, string> custom = null,
            string label = null,
            bool feedback = false,
            Template template = Default.ParameterTemplate)
        {
            DataGeneratorProperties generatorProperties = new DataGeneratorProperties();
            generatorProperties.Set(RequestTestParameter.Length, "" + length);
            generatorProperties.Set(RequestTestParameter.Duplicates, "" + duplicates);
            generatorProperties.Set(RequestTestParameter.Adaptive, "" + adaptive);

            DataGenerator generatorOptions = new DataGenerator(generatorProperties);
            generatorOptions.Set(RequestTestParameter.DataSource, Generator.Random.GetValue());
            generatorOptions.Set(RequestTestParameter.Choices, choices);
            generatorOptions.Set(RequestTestParameter.Constraints, constraints);

            DataSession sessionData = new DataSession();
            sessionData.GeneratorData = generatorOptions;
            sessionData.ModelId = model == null ? Model : model;
            sessionData.MethodName = method;
            sessionData.Template = template;
            sessionData.Custom = custom;
            sessionData.BuildFeedback = feedback;
            sessionData.TestSessionLabel = label;
            sessionData.Constraints = constraints;
            sessionData.Choices = choices;
            sessionData.GeneratorAddress = GeneratorAddress;

            return Process<string>(sessionData);
        }

        public IEnumerable<string> ExportStatic(
            string method,
            object testSuites = null,
            string model = null,
            Dictionary<string, string> custom = null,
            string label = null,
            bool feedback = false,
            Template template = Default.ParameterTemplate)
        {
            object updatedTestSuites = testSuites == null ? Default.ParameterTestSuite : testSuites;

            DataGeneratorProperties generatorProperties = new DataGeneratorProperties();

            DataGenerator generatorOptions = new DataGenerator(generatorProperties);
            generatorOptions.Set(RequestTestParameter.DataSource, Generator.Static.GetValue());
            generatorOptions.Set(RequestTestParameter.TestSuites, updatedTestSuites);
           
            DataSession sessionData = new DataSession();
            sessionData.GeneratorData = generatorOptions;
            sessionData.ModelId = model == null ? Model : model;
            sessionData.MethodName = method;
            sessionData.Template = template;
            sessionData.Custom = custom;
            sessionData.BuildFeedback = feedback;
            sessionData.TestSessionLabel = label;
            sessionData.TestSuites = updatedTestSuites;
            sessionData.GeneratorAddress = GeneratorAddress;

            return Process<string>(sessionData);
        }

//-------------------------------------------------------------------------------------------

        internal IEnumerable<object[]> Generate(
            string method,
            Generator generator,
            DataGenerator generatorOptions,
            string model = null,
            Dictionary<string, string> custom = null,
            string label = null,
            bool feedback = false,
            Assembly assembly = null,
            string typesDefinitionsSource = null)
        {
            generatorOptions.Set(RequestTestParameter.DataSource, generator.GetValue());

            DataSession sessionData = new DataSession();
            sessionData.GeneratorData = generatorOptions;
            sessionData.ModelId = model == null ? Model : model;
            sessionData.MethodName = method;
            sessionData.Custom = custom;
            sessionData.BuildFeedback = feedback;
            sessionData.TestSessionLabel = label;
            sessionData.GeneratorAddress = GeneratorAddress;

            if (assembly != null && typesDefinitionsSource != null)
            {
                Initializer.Source(assembly, typesDefinitionsSource);
            }

            return Process<object[]>(sessionData);
        }

        public IEnumerable<object[]> GenerateNWise(
            string method,
            int n = Default.ParameterN, 
            int coverage = Default.ParameterCoverage,
            Dictionary<string, string[]> choices = null,
            object constraints = null,
            string model = null,
            Dictionary<string, string> custom = null,
            string label = null,
            bool feedback = false,
            Assembly assembly = null,
            string typesDefinitionsSource = null)
        {
            DataGeneratorProperties generatorProperties = new DataGeneratorProperties();
            generatorProperties.Set(RequestTestParameter.N, "" + n);
            generatorProperties.Set(RequestTestParameter.Coverage, "" + coverage);

            DataGenerator generatorOptions = new DataGenerator(generatorProperties);
            generatorOptions.Set(RequestTestParameter.DataSource, Generator.NWise.GetValue());
            generatorOptions.Set(RequestTestParameter.Choices, choices);
            generatorOptions.Set(RequestTestParameter.Constraints, constraints);

            DataSession sessionData = new DataSession();
            sessionData.GeneratorData = generatorOptions;
            sessionData.ModelId = model == null ? Model : model;
            sessionData.MethodName = method;
            sessionData.Custom = custom;
            sessionData.BuildFeedback = feedback;
            sessionData.TestSessionLabel = label;
            sessionData.Constraints = constraints;
            sessionData.Choices = choices;
            sessionData.GeneratorAddress = GeneratorAddress;

            if (assembly != null && typesDefinitionsSource != null)
            {
                Initializer.Source(assembly, typesDefinitionsSource);
            }

            return Process<object[]>(sessionData);
        }

        public IEnumerable<object[]> GenerateCartesian(
            string method,
            Dictionary<string, string[]> choices = null,
            object constraints = null, 
            string model = null,
            Dictionary<string, string> custom = null,
            string label = null,
            bool feedback = false,
            Assembly assembly = null,
            string typesDefinitionsSource = null)
        {
            DataGeneratorProperties generatorProperties = new DataGeneratorProperties();

            DataGenerator generatorOptions = new DataGenerator(generatorProperties);
            generatorOptions.Set(RequestTestParameter.DataSource, Generator.Cartesian.GetValue());
            generatorOptions.Set(RequestTestParameter.Choices, choices);
            generatorOptions.Set(RequestTestParameter.Constraints, constraints);

            DataSession sessionData = new DataSession();
            sessionData.GeneratorData = generatorOptions;
            sessionData.ModelId = model == null ? Model : model;
            sessionData.MethodName = method;
            sessionData.Custom = custom;
            sessionData.BuildFeedback = feedback;
            sessionData.TestSessionLabel = label;
            sessionData.Constraints = constraints;
            sessionData.Choices = choices;
            sessionData.GeneratorAddress = GeneratorAddress;

            if (assembly != null && typesDefinitionsSource != null)
            {
                Initializer.Source(assembly, typesDefinitionsSource);
            }

            return Process<object[]>(sessionData);
        }

        public IEnumerable<object[]> GenerateRandom(
            string method,
            int length = Default.ParameterLength, 
            bool duplicates = Default.ParameterDuplicates,
            bool adaptive = Default.ParameterAdaptive,
            Dictionary<string, string[]> choices = null,
            object constraints = null,
            string model = null,
            Dictionary<string, string> custom = null,
            string label = null,
            bool feedback = false,
            Assembly assembly = null,
            string typesDefinitionsSource = null)
        {
            DataGeneratorProperties generatorProperties = new DataGeneratorProperties();
            generatorProperties.Set(RequestTestParameter.Length, "" + length);
            generatorProperties.Set(RequestTestParameter.Duplicates, "" + duplicates);
            generatorProperties.Set(RequestTestParameter.Adaptive, "" + adaptive);

            DataGenerator generatorOptions = new DataGenerator(generatorProperties);
            generatorOptions.Set(RequestTestParameter.DataSource, Generator.Random.GetValue());
            generatorOptions.Set(RequestTestParameter.Choices, choices);
            generatorOptions.Set(RequestTestParameter.Constraints, constraints);

            DataSession sessionData = new DataSession();
            sessionData.GeneratorData = generatorOptions;
            sessionData.ModelId = model == null ? Model : model;
            sessionData.MethodName = method;
            sessionData.Custom = custom;
            sessionData.BuildFeedback = feedback;
            sessionData.TestSessionLabel = label;
            sessionData.Constraints = constraints;
            sessionData.Choices = choices;
            sessionData.GeneratorAddress = GeneratorAddress;

            if (assembly != null && typesDefinitionsSource != null)
            {
                Initializer.Source(assembly, typesDefinitionsSource);
            }

            return Process<object[]>(sessionData);
        }

        public IEnumerable<object[]> GenerateStatic(
            string method,
            object testSuites = null,
            string model = null,
            Dictionary<string, string> custom = null,
            string label = null,
            bool feedback = false,
            Assembly assembly = null,
            string typesDefinitionsSource = null)
        {
            object updatedTestSuites = testSuites == null ? Default.ParameterTestSuite : testSuites;

            DataGeneratorProperties generatorProperties = new DataGeneratorProperties();

            DataGenerator generatorOptions = new DataGenerator(generatorProperties);
            generatorOptions.Set(RequestTestParameter.DataSource, Generator.Static.GetValue());
            generatorOptions.Set(RequestTestParameter.TestSuites, updatedTestSuites);
           
            DataSession sessionData = new DataSession();
            sessionData.GeneratorData = generatorOptions;
            sessionData.ModelId = model == null ? Model : model;
            sessionData.MethodName = method;
            sessionData.Custom = custom;
            sessionData.BuildFeedback = feedback;
            sessionData.TestSessionLabel = label;
            sessionData.TestSuites = updatedTestSuites;
            sessionData.GeneratorAddress = GeneratorAddress;

            if (assembly != null && typesDefinitionsSource != null)
            {
                Initializer.Source(assembly, typesDefinitionsSource);
            }

            return Process<object[]>(sessionData);
        }

//-------------------------------------------------------------------------------------------

        private IEnumerable<T> Process<T>(DataSession sessionData)
        {
            sessionData.KeyStorePath = KeyStorePath;
            sessionData.KeyStorePassword = KeyStorePassword;

            string requestURL = RequestHelper.GenerateRequestURL(sessionData, GeneratorAddress);

            return ProcessResponse<T>(sessionData, RequestHelper.SendRequest(requestURL, KeyStorePath, KeyStorePassword));
        }

//-------------------------------------------------------------------------------------------  

        private IEnumerable<T> ProcessResponse<T>(DataSession sessionData, HttpWebResponse response) 
        {
            if (!response.StatusCode.ToString().Equals("OK"))
            {
                throw new TestProviderException(response.StatusDescription);
            }
            else
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8)) {
                    string line;
                    
                    while ((line = reader.ReadLine()) != null) {
                        T element = ProcessResponseLine<T>(line, ref sessionData);
                            
                        if (element != null)
                        {
                            yield return element;
                        }
                    }
                }
            }
        }

        private T ProcessResponseLine<T>(string line, ref DataSession sessionData)
        {
            if (sessionData.Template == Template.Stream)
            {
                if (line.Contains("\"testCase\""))
                {
                    return ProcessResponseDataLine<T>(line, sessionData);
                }
                if (line.Contains("\"status\""))
                {
                    return ProcessResponseStatusLine<T>(line, sessionData);
                }
                if (line.Contains("\"info\""))
                {
                    return ProcessResponseInfoLine<T>(line, ref sessionData);
                }
                
                return default(T);
            }
            else
            {
                return GenerateTestEvent<T>(line, sessionData);
            }
        }

        private T ProcessResponseDataLine<T>(string line, DataSession sessionData)
        {
            try
            {
                TestCase testCase = JsonConvert.DeserializeObject<TestCase>(line);
                return GenerateTestEvent<T>(line, sessionData);
            }
            catch (JsonReaderException e) 
            { 
                DebugHelper.PrintTrace("PARSE DATA - READ", e.StackTrace);
            }
            catch (JsonSerializationException e) 
            { 
                DebugHelper.PrintTrace("PARSE DATA - SERIALIZATION", e.StackTrace);
            }
  
            return default(T);
        }

        private T ProcessResponseStatusLine<T>(string line, DataSession sessionData)
        {
            try
            {
                MessageStatus messageStatus = JsonConvert.DeserializeObject<MessageStatus>(line);

                if (HelperMessageStatus.IsTransmissionFinished(messageStatus))
                {
                    sessionData.FinishTransmission();
                }

                DebugHelper.PrintTrace("STATUS", messageStatus.Status);
            }
            catch (JsonReaderException e) 
            { 
                DebugHelper.PrintTrace("PARSE STATUS - READ", e.StackTrace);
            }
            catch (JsonSerializationException e) 
            { 
                DebugHelper.PrintTrace("PARSE STATUS - SERIALIZATION", e.StackTrace);
            }

            return default(T);
        }

        private T ProcessResponseInfoLine<T>(string line, ref DataSession sessionData)
        {
            try
            {    
                HelperMessageInfo.ParseInfoMessage(line, ref sessionData, Initializer);
                DebugHelper.PrintTrace("INFO", string.Join(", ", sessionData.MethodArgumentTypes));
            }
            catch (JsonReaderException e) 
            { 
                DebugHelper.PrintTrace("PARSE INFO - READ", e.StackTrace);
            }
            catch (JsonSerializationException e) 
            { 
                DebugHelper.PrintTrace("PARSE INFO - SERIALIZATION", e.StackTrace);
            }

            return default(T);
        }

        private T GenerateTestEvent<T>(string data, DataSession sessionData)
        {
            if (typeof(T).ToString().Equals("System.Object[]"))
            {
                return (T)(object)ChunkParser.ParseTestCaseToDataType(data, sessionData, Initializer);
            }

            if (typeof(T).ToString().Equals("System.String[][]"))
            {
                string[][] response = { sessionData.MethodArgumentTypes, sessionData.MethodArgumentNames };
                return (T)(object)response;
            }

            if (typeof(T).ToString().Equals("System.String"))
            {
                return (T)(object)data;
            }
   
            throw new TestProviderException("Unknown type.");
        }

//------------------------------------------------------------------------------------------- 

        public override string ToString()
        { 
            return
                $"TestProvider:\n" +
                $"\t[KeyStorePath: '{ Path.GetFullPath(KeyStorePath) }']\n" +
                $"\t[KeyStorePassword: '{ KeyStorePassword }']\n" +
                $"\t[GeneratorAddress: '{ GeneratorAddress }']\n" +
                $"\t[Model: '{ Model }']";
        }
    }
}