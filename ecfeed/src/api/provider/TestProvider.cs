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
            GeneratorProperties additionalProperties = new GeneratorProperties();
            additionalProperties.Add(Parameter.Length, "1");

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.Add(Parameter.DataSource, Generator.Random.GetValue());

            IEnumerable<string[][]> queue = Process<string[][]>(method, additionalOptions, null, null, null, new Feedback(), Template.Stream);

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
            Template template = Default.ParameterTemplate)
        {
            generatorOptions.Add(Parameter.DataSource, generator.GetValue());

            return Process<string>(method, generatorOptions, null, null, model, new Feedback(), template);
        }

        public IEnumerable<string> ExportNWise(
            string method,
            int n = Default.ParameterN, 
            int coverage = Default.ParameterCoverage,
            Dictionary<string, string[]> choices = null,
            object constraints = null,
            string model = null,
            Template template = Default.ParameterTemplate)
        {
            GeneratorProperties additionalProperties = new GeneratorProperties();
            additionalProperties.Add(Parameter.N, "" + n);
            additionalProperties.Add(Parameter.Coverage, "" + coverage);

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.Add(Parameter.DataSource, Generator.NWise.GetValue());

            return Process<string>(method, additionalOptions, choices, constraints, model, new Feedback(), template);
        }

        public IEnumerable<string> ExportCartesian(
            string method,
            Dictionary<string, string[]> choices = null,
            object constraints = null,
            string model = null,
            Template template = Default.ParameterTemplate)
        {
            GeneratorProperties additionalProperties = new GeneratorProperties();

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.Add(Parameter.DataSource, Generator.Cartesian.GetValue());

            return Process<string>(method, additionalOptions, choices, constraints, model, new Feedback(), template);
        }

        public IEnumerable<string> ExportRandom(
            string method,
            int length = Default.ParameterLength, 
            bool duplicates = Default.ParameterDuplicates,
            bool adaptive = Default.ParameterAdaptive,
            Dictionary<string, string[]> choices = null,
            object constraints = null,
            string model = null,
            Template template = Default.ParameterTemplate)
        {
            GeneratorProperties additionalProperties = new GeneratorProperties();
            additionalProperties.Add(Parameter.Length, "" + length);
            additionalProperties.Add(Parameter.Duplicates, "" + duplicates);
            additionalProperties.Add(Parameter.Adaptive, "" + adaptive);

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.Add(Parameter.DataSource, Generator.Random.GetValue());

            return Process<string>(method, additionalOptions, choices, constraints, model, new Feedback(), template);
        }

        public IEnumerable<string> ExportStatic(
            string method,
            object testSuites = null,
            string model = null,
            Template template = Default.ParameterTemplate)
        {
            object updatedTestSuites = testSuites == null ? Default.ParameterTestSuite : testSuites;

            GeneratorProperties additionalProperties = new GeneratorProperties();

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.Add(Parameter.DataSource, Generator.Static.GetValue());
            additionalOptions.Add(Parameter.TestSuites, updatedTestSuites);
           
            return Process<string>(method, additionalOptions, null, null, model, new Feedback(), template);
        }

//-------------------------------------------------------------------------------------------

        internal IEnumerable<object[]> Generate(
            string method,
            Generator generator,
            GeneratorOptions generatorOptions,
            string model = null)
        {
            generatorOptions.Add(Parameter.DataSource, generator.GetValue());

            Feedback feedback = new Feedback();
            feedback.GeneratorType = generator.GetValue();

            return Process<object[]>(method, generatorOptions, null, null, model, feedback);
        }

        public IEnumerable<object[]> GenerateNWise(
            string method,
            int n = Default.ParameterN, 
            int coverage = Default.ParameterCoverage,
            Dictionary<string, string[]> choices = null,
            object constraints = null,
            string model = null)
        {
            GeneratorProperties additionalProperties = new GeneratorProperties();
            additionalProperties.Add(Parameter.N, "" + n);
            additionalProperties.Add(Parameter.Coverage, "" + coverage);

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.Add(Parameter.DataSource, Generator.NWise.GetValue());

            Feedback feedback = new Feedback();
            feedback.GeneratorOptions = additionalProperties.List();
            feedback.GeneratorType = Generator.NWise.GetValue();

            return Process<object[]>(method, additionalOptions, choices, constraints, model, feedback);
        }

        public IEnumerable<object[]> GenerateCartesian(
            string method,
            Dictionary<string, string[]> choices = null,
            object constraints = null, 
            string model = null)
        {
            GeneratorProperties additionalProperties = new GeneratorProperties();

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.Add(Parameter.DataSource, Generator.Cartesian.GetValue());

            Feedback feedback = new Feedback();
            feedback.GeneratorOptions = additionalProperties.List();
            feedback.GeneratorType = Generator.Cartesian.GetValue();

            return Process<object[]>(method, additionalOptions, choices, constraints, model, feedback);
        }

        public IEnumerable<object[]> GenerateRandom(
            string method,
            int length = Default.ParameterLength, 
            bool duplicates = Default.ParameterDuplicates,
            bool adaptive = Default.ParameterAdaptive,
            Dictionary<string, string[]> choices = null,
            object constraints = null,
            string model = null)
        {
            GeneratorProperties additionalProperties = new GeneratorProperties();
            additionalProperties.Add(Parameter.Length, "" + length);
            additionalProperties.Add(Parameter.Duplicates, "" + duplicates);
            additionalProperties.Add(Parameter.Adaptive, "" + adaptive);

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.Add(Parameter.DataSource, Generator.Random.GetValue());

            Feedback feedback = new Feedback();
            feedback.GeneratorOptions = additionalProperties.List();
            feedback.GeneratorType = Generator.Random.GetValue();

            return Process<object[]>(method, additionalOptions, choices, constraints, null, feedback);
        }

        public IEnumerable<object[]> GenerateStatic(
            string method,
            object testSuites = null,
            string model = null)
        {
            object updatedTestSuites = testSuites == null ? Default.ParameterTestSuite : testSuites;

            GeneratorProperties additionalProperties = new GeneratorProperties();

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.Add(Parameter.DataSource, Generator.Static.GetValue());
            additionalOptions.Add(Parameter.TestSuites, updatedTestSuites);
           

            Feedback feedback = new Feedback();
            feedback.GeneratorOptions = additionalProperties.List();
            feedback.GeneratorType = Generator.Static.GetValue();
            feedback.TestSuites = updatedTestSuites;

            return Process<object[]>(method, additionalOptions, null, null, model, feedback);
        }

//-------------------------------------------------------------------------------------------

        private IEnumerable<T> Process<T>(
            string method, 
            GeneratorOptions options, 
            Dictionary<string, string[]> choices = null,
            object constraints = null,
            string model = null,
            Feedback feedback = null,
            Template template = Template.Stream
            )
        {
            if (choices != null)
            {
                options.Add(Parameter.Choices, choices);
            }

            if (constraints != null)
            {
                options.Add(Parameter.Constraints, constraints);
            }

            feedback.Framework = "C#";
            feedback.MethodName = method;
            feedback.ModelId = model == null ? Model : model;
            feedback.Choices = choices;
            feedback.Constraints = constraints;
            feedback.GeneratorType = (string)options.Get(Parameter.DataSource);

            string requestURL = HelperRequest.GenerateRequestURL(options, model, method, template.GetValue(), GeneratorAddress, Model);

            return ProcessResponse<T>(HelperRequest.SendRequest(requestURL, KeyStorePath, KeyStorePassword), template, feedback);
        }

//-------------------------------------------------------------------------------------------  

        private IEnumerable<T> ProcessResponse<T>(HttpWebResponse response, Template template, Feedback feedback) 
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
                        T element = ProcessResponseLine<T>(line, template, ref feedback);
                            
                        if (element != null)
                        {
                            yield return element;
                        }
                    }
                }
            }
        }

        private T ProcessResponseLine<T>(string line, Template template, ref Feedback feedback)
        {
            if (template == Template.Stream)
            {
                if (line.Contains("\"testCase\""))
                {
                    return ProcessResponseDataLine<T>(line, feedback);
                }
                if (line.Contains("\"status\""))
                {
                    return ProcessResponseStatusLine<T>(line);
                }
                if (line.Contains("\"info\""))
                {
                    return ProcessResponseInfoLine<T>(line, ref feedback);
                }
                
                return default(T);
            }
            else
            {
                return GenerateTestEvent<T>(line, feedback);
            }
        }

        private T ProcessResponseDataLine<T>(string line, Feedback feedback)
        {
            try
            {
                TestCase testCase = JsonConvert.DeserializeObject<TestCase>(line);
                return GenerateTestEvent<T>(line, feedback);
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

        private T ProcessResponseInfoLine<T>(string line, ref Feedback feedback)
        {
            try
            {    
                MessageInfoHelper.ParseInfoMessage(line, ref feedback);
                HelperDebug.PrintTrace("INFO", string.Join(", ", feedback.MethodArgumentTypes));
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

        private T GenerateTestEvent<T>(string data, Feedback feedback)
        {
            if (typeof(T).ToString().Equals("System.Object[]"))
            {
                return (T)(object)StreamParser.ParseTestCaseToDataType(data, feedback);
            }

            if (typeof(T).ToString().Equals("System.String[][]"))
            {
                string[][] response = { feedback.MethodArgumentTypes, feedback.MethodArgumentNames };
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