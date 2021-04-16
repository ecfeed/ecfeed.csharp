using System.IO;
using System.Text;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;

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

        public TestProvider(string model, string keyStorePath = null, string keyStorePassword = null, string generatorAddress = null)
        {
            Model = model;

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
            GeneratorProperties generatorProperties = new GeneratorProperties();
            generatorProperties.Add(Parameter.Length, "1");

            GeneratorOptions generatorOptions = new GeneratorOptions(generatorProperties);
            generatorOptions.Add(Parameter.DataSource, Generator.Random.GetValue());

            SessionData sessionData = new SessionData();
            sessionData.GeneratorOptions = generatorOptions;
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
            GeneratorOptions generatorOptions,
            string model = null,
            Dictionary<string, string> custom = null,
            string label = null,
            bool feedback = false,
            Template template = Default.ParameterTemplate)
        {
            generatorOptions.Add(Parameter.DataSource, generator.GetValue());

            SessionData sessionData = new SessionData();
            sessionData.GeneratorOptions = generatorOptions;
            sessionData.ModelId = model == null ? Model : model;
            sessionData.MethodName = method;
            sessionData.Template = template;
            sessionData.Custom = custom;
            sessionData.Feedback = feedback;
            sessionData.TestSessionLabel = label;

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
            GeneratorProperties generatorProperties = new GeneratorProperties();
            generatorProperties.Add(Parameter.N, "" + n);
            generatorProperties.Add(Parameter.Coverage, "" + coverage);

            GeneratorOptions generatorOptions = new GeneratorOptions(generatorProperties);
            generatorOptions.Add(Parameter.DataSource, Generator.NWise.GetValue());
            generatorOptions.Add(Parameter.Choices, choices);
            generatorOptions.Add(Parameter.Constraints, constraints);

            SessionData sessionData = new SessionData();
            sessionData.GeneratorOptions = generatorOptions;
            sessionData.ModelId = model == null ? Model : model;
            sessionData.MethodName = method;
            sessionData.Template = template;
            sessionData.Custom = custom;
            sessionData.Feedback = feedback;
            sessionData.TestSessionLabel = label;

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
            GeneratorProperties generatorProperties = new GeneratorProperties();

            GeneratorOptions generatorOptions = new GeneratorOptions(generatorProperties);
            generatorOptions.Add(Parameter.DataSource, Generator.Cartesian.GetValue());
            generatorOptions.Add(Parameter.Choices, choices);
            generatorOptions.Add(Parameter.Constraints, constraints);

            SessionData sessionData = new SessionData();
            sessionData.GeneratorOptions = generatorOptions;
            sessionData.ModelId = model == null ? Model : model;
            sessionData.MethodName = method;
            sessionData.Template = template;
            sessionData.Custom = custom;
            sessionData.Feedback = feedback;
            sessionData.TestSessionLabel = label;

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
            GeneratorProperties generatorProperties = new GeneratorProperties();
            generatorProperties.Add(Parameter.Length, "" + length);
            generatorProperties.Add(Parameter.Duplicates, "" + duplicates);
            generatorProperties.Add(Parameter.Adaptive, "" + adaptive);

            GeneratorOptions generatorOptions = new GeneratorOptions(generatorProperties);
            generatorOptions.Add(Parameter.DataSource, Generator.Random.GetValue());
            generatorOptions.Add(Parameter.Choices, choices);
            generatorOptions.Add(Parameter.Constraints, constraints);

            SessionData sessionData = new SessionData();
            sessionData.GeneratorOptions = generatorOptions;
            sessionData.ModelId = model == null ? Model : model;
            sessionData.MethodName = method;
            sessionData.Template = template;
            sessionData.Custom = custom;
            sessionData.Feedback = feedback;
            sessionData.TestSessionLabel = label;

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

            GeneratorProperties generatorProperties = new GeneratorProperties();

            GeneratorOptions generatorOptions = new GeneratorOptions(generatorProperties);
            generatorOptions.Add(Parameter.DataSource, Generator.Static.GetValue());
            generatorOptions.Add(Parameter.TestSuites, updatedTestSuites);
           
            SessionData sessionData = new SessionData();
            sessionData.GeneratorOptions = generatorOptions;
            sessionData.ModelId = model == null ? Model : model;
            sessionData.MethodName = method;
            sessionData.Template = template;
            sessionData.Custom = custom;
            sessionData.Feedback = feedback;
            sessionData.TestSessionLabel = label;

            return Process<string>(sessionData);
        }

//-------------------------------------------------------------------------------------------

        internal IEnumerable<object[]> Generate(
            string method,
            Generator generator,
            GeneratorOptions generatorOptions,
            string model = null,
            Dictionary<string, string> custom = null,
            string label = null,
            bool feedback = false)
        {
            generatorOptions.Add(Parameter.DataSource, generator.GetValue());

            SessionData sessionData = new SessionData();
            sessionData.GeneratorOptions = generatorOptions;
            sessionData.ModelId = model == null ? Model : model;
            sessionData.MethodName = method;
            sessionData.Custom = custom;
            sessionData.Feedback = feedback;
            sessionData.TestSessionLabel = label;

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
            bool feedback = false)
        {
            GeneratorProperties generatorProperties = new GeneratorProperties();
            generatorProperties.Add(Parameter.N, "" + n);
            generatorProperties.Add(Parameter.Coverage, "" + coverage);

            GeneratorOptions generatorOptions = new GeneratorOptions(generatorProperties);
            generatorOptions.Add(Parameter.DataSource, Generator.NWise.GetValue());
            generatorOptions.Add(Parameter.Choices, choices);
            generatorOptions.Add(Parameter.Constraints, constraints);

            SessionData sessionData = new SessionData();
            sessionData.GeneratorOptions = generatorOptions;
            sessionData.ModelId = model == null ? Model : model;
            sessionData.MethodName = method;
            sessionData.Custom = custom;
            sessionData.Feedback = feedback;
            sessionData.TestSessionLabel = label;

            return Process<object[]>(sessionData);
        }

        public IEnumerable<object[]> GenerateCartesian(
            string method,
            Dictionary<string, string[]> choices = null,
            object constraints = null, 
            string model = null,
            Dictionary<string, string> custom = null,
            string label = null,
            bool feedback = false)
        {
            GeneratorProperties generatorProperties = new GeneratorProperties();

            GeneratorOptions generatorOptions = new GeneratorOptions(generatorProperties);
            generatorOptions.Add(Parameter.DataSource, Generator.Cartesian.GetValue());
            generatorOptions.Add(Parameter.Choices, choices);
            generatorOptions.Add(Parameter.Constraints, constraints);

            SessionData sessionData = new SessionData();
            sessionData.GeneratorOptions = generatorOptions;
            sessionData.ModelId = model == null ? Model : model;
            sessionData.MethodName = method;
            sessionData.Custom = custom;
            sessionData.Feedback = feedback;
            sessionData.TestSessionLabel = label;

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
            bool feedback = false)
        {
            GeneratorProperties generatorProperties = new GeneratorProperties();
            generatorProperties.Add(Parameter.Length, "" + length);
            generatorProperties.Add(Parameter.Duplicates, "" + duplicates);
            generatorProperties.Add(Parameter.Adaptive, "" + adaptive);

            GeneratorOptions generatorOptions = new GeneratorOptions(generatorProperties);
            generatorOptions.Add(Parameter.DataSource, Generator.Random.GetValue());
            generatorOptions.Add(Parameter.Choices, choices);
            generatorOptions.Add(Parameter.Constraints, constraints);

            SessionData sessionData = new SessionData();
            sessionData.GeneratorOptions = generatorOptions;
            sessionData.ModelId = model == null ? Model : model;
            sessionData.MethodName = method;
            sessionData.Custom = custom;
            sessionData.Feedback = feedback;
            sessionData.TestSessionLabel = label;

            return Process<object[]>(sessionData);
        }

        public IEnumerable<object[]> GenerateStatic(
            string method,
            object testSuites = null,
            string model = null,
            Dictionary<string, string> custom = null,
            string label = null,
            bool feedback = false)
        {
            object updatedTestSuites = testSuites == null ? Default.ParameterTestSuite : testSuites;

            GeneratorProperties generatorProperties = new GeneratorProperties();

            GeneratorOptions generatorOptions = new GeneratorOptions(generatorProperties);
            generatorOptions.Add(Parameter.DataSource, Generator.Static.GetValue());
            generatorOptions.Add(Parameter.TestSuites, updatedTestSuites);
           
            SessionData sessionData = new SessionData();
            sessionData.GeneratorOptions = generatorOptions;
            sessionData.ModelId = model == null ? Model : model;
            sessionData.MethodName = method;
            sessionData.Custom = custom;
            sessionData.Feedback = feedback;
            sessionData.TestSessionLabel = label;

            return Process<object[]>(sessionData);
        }

//-------------------------------------------------------------------------------------------

        private IEnumerable<T> Process<T>(SessionData sessionData)
        {
            string requestURL = HelperRequest.GenerateRequestURL(sessionData, GeneratorAddress);

            return ProcessResponse<T>(sessionData, HelperRequest.SendRequest(requestURL, KeyStorePath, KeyStorePassword));
        }

//-------------------------------------------------------------------------------------------  

        private IEnumerable<T> ProcessResponse<T>(SessionData sessionData, HttpWebResponse response) 
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

        private T ProcessResponseLine<T>(string line, ref SessionData sessionData)
        {
            if (sessionData.Template == Template.Stream)
            {
                if (line.Contains("\"testCase\""))
                {
                    return ProcessResponseDataLine<T>(line, sessionData);
                }
                if (line.Contains("\"status\""))
                {
                    return ProcessResponseStatusLine<T>(line);
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

        private T ProcessResponseDataLine<T>(string line, SessionData sessionData)
        {
            try
            {
                TestCase testCase = JsonConvert.DeserializeObject<TestCase>(line);
                return GenerateTestEvent<T>(line, sessionData);
            }
            catch (JsonReaderException e) 
            { 
                HelperDebug.PrintTrace("PARSE DATA - READ", e.StackTrace);
            }
            catch (JsonSerializationException e) 
            { 
                HelperDebug.PrintTrace("PARSE DATA - SERIALIZATION", e.StackTrace);
            }
  
            return default(T);
        }

        private T ProcessResponseStatusLine<T>(string line)
        {
            try
            {
                MessageStatus messageStatus = JsonConvert.DeserializeObject<MessageStatus>(line);
                HelperDebug.PrintTrace("STATUS", messageStatus.Status);
            }
            catch (JsonReaderException e) 
            { 
                HelperDebug.PrintTrace("PARSE STATUS - READ", e.StackTrace);
            }
            catch (JsonSerializationException e) 
            { 
                HelperDebug.PrintTrace("PARSE STATUS - SERIALIZATION", e.StackTrace);
            }

            return default(T);
        }

        private T ProcessResponseInfoLine<T>(string line, ref SessionData sessionData)
        {
            try
            {    
                MessageInfoHelper.ParseInfoMessage(line, ref sessionData);
                HelperDebug.PrintTrace("INFO", string.Join(", ", sessionData.MethodArgumentTypes));
            }
            catch (JsonReaderException e) 
            { 
                HelperDebug.PrintTrace("PARSE INFO - READ", e.StackTrace);
            }
            catch (JsonSerializationException e) 
            { 
                HelperDebug.PrintTrace("PARSE INFO - SERIALIZATION", e.StackTrace);
            }

            return default(T);
        }

        private T GenerateTestEvent<T>(string data, SessionData sessionData)
        {
            if (typeof(T).ToString().Equals("System.Object[]"))
            {
                return (T)(object)StreamParser.ParseTestCaseToDataType(data, sessionData);
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