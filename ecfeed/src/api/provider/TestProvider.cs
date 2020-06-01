// #define VERBOSE

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
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

        public void ValidateConnection() 
        {
            HttpWebResponse response = SendRequest(GenerateHealthCheckURL(GeneratorAddress));
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8)) {
                string line;
                    
                while ((line = reader.ReadLine()) != null) {
                    PrintTrace("VALIDATE", line);
                }
               
            }
            
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
            additionalProperties.AddProperty(Parameter.Length, "1");

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.AddOption(Parameter.DataSource, Generator.Random.GetValue());

            IEnumerable<string[][]> queue = Process<string[][]>(method, additionalOptions, null, null, null, Template.Stream);

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
            generatorOptions.AddOption(Parameter.DataSource, generator.GetValue());

            return Process<string>(method, generatorOptions, null, null, model, template);
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
            additionalProperties.AddProperty(Parameter.N, "" + n);
            additionalProperties.AddProperty(Parameter.Coverage, "" + coverage);

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.AddOption(Parameter.DataSource, Generator.NWise.GetValue());

            return Process<string>(method, additionalOptions, choices, constraints, model, template);
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
            additionalOptions.AddOption(Parameter.DataSource, Generator.Cartesian.GetValue());

            return Process<string>(method, additionalOptions, choices, constraints, model, template);
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
            additionalProperties.AddProperty(Parameter.Length, "" + length);
            additionalProperties.AddProperty(Parameter.Duplicates, "" + duplicates);
            additionalProperties.AddProperty(Parameter.Adaptive, "" + adaptive);

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.AddOption(Parameter.DataSource, Generator.Random.GetValue());

            return Process<string>(method, additionalOptions, choices, constraints, model, template);
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
            additionalOptions.AddOption(Parameter.DataSource, Generator.Static.GetValue());
            additionalOptions.AddOption(Parameter.TestSuites, updatedTestSuites);
           
            return Process<string>(method, additionalOptions, null, null, model, template);
        }

//-------------------------------------------------------------------------------------------

        internal IEnumerable<object[]> Generate(
            string method,
            Generator generator,
            GeneratorOptions generatorOptions,
            string model = null)
        {
            generatorOptions.AddOption(Parameter.DataSource, generator.GetValue());

            return Process<object[]>(method, generatorOptions, null, null, model);
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
            additionalProperties.AddProperty(Parameter.N, "" + n);
            additionalProperties.AddProperty(Parameter.Coverage, "" + coverage);

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.AddOption(Parameter.DataSource, Generator.NWise.GetValue());

            return Process<object[]>(method, additionalOptions, choices, constraints, model);
        }

        public IEnumerable<object[]> GenerateCartesian(
            string method,
            Dictionary<string, string[]> choices = null,
            object constraints = null, 
            string model = null)
        {
            GeneratorProperties additionalProperties = new GeneratorProperties();

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.AddOption(Parameter.DataSource, Generator.Cartesian.GetValue());

            return Process<object[]>(method, additionalOptions, choices, constraints, model);
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
            additionalProperties.AddProperty(Parameter.Length, "" + length);
            additionalProperties.AddProperty(Parameter.Duplicates, "" + duplicates);
            additionalProperties.AddProperty(Parameter.Adaptive, "" + adaptive);

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.AddOption(Parameter.DataSource, Generator.Random.GetValue());

            return Process<object[]>(method, additionalOptions, choices, constraints, null);
        }

        public IEnumerable<object[]> GenerateStatic(
            string method,
            object testSuites = null,
            string model = null)
        {
            object updatedTestSuites = testSuites == null ? Default.ParameterTestSuite : testSuites;

            GeneratorProperties additionalProperties = new GeneratorProperties();

            GeneratorOptions additionalOptions = new GeneratorOptions(additionalProperties);
            additionalOptions.AddOption(Parameter.DataSource, Generator.Static.GetValue());
            additionalOptions.AddOption(Parameter.TestSuites, updatedTestSuites);
           
            return Process<object[]>(method, additionalOptions, null, null, model);
        }

//-------------------------------------------------------------------------------------------

        private IEnumerable<T> Process<T>(
            string method, 
            GeneratorOptions options, 
            Dictionary<string, string[]> choices = null,
            object constraints = null,
            string model = null,
            Template template = Template.Stream)
        {
            if (choices != null)
            {
                options.AddOption(Parameter.Choices, choices);
            }

            if (constraints != null)
            {
                options.AddOption(Parameter.Constraints, constraints);
            }

            string requestURL = GenerateRequestURL(options, model, method, template.GetValue());

            return ProcessResponse<T>(SendRequest(requestURL), template);
        }

//-------------------------------------------------------------------------------------------

        private string GenerateHealthCheckURL(string address)
        {
            string request = $"{ address }/{ Endpoint.HealthCheck }";

            request = Uri.EscapeUriString(request).Replace("[", "%5B").Replace("]", "%5D");

            PrintTrace("HEALTH CHECK REQUEST", request);

            return request;
        }

        private string GenerateRequestURL(GeneratorOptions options, string model, string method, string template)
        {
            string requestData = $"{ SerializeRequestData(options, model, method, template) }";
            string requestType = template.Equals(Template.Stream.GetValue()) ? Request.Data : Request.Export;
            string request = $"{ GeneratorAddress }/{ Endpoint.Generator }?requestType={ requestType }&request={ requestData }";
Console.WriteLine(request);
            request = Uri.EscapeUriString(request).Replace("[", "%5B").Replace("]", "%5D");

            PrintTrace("DATA REQUEST", request);

            return request;
        }

        private string SerializeRequestData(GeneratorOptions options, string model, string method, string template)
        {
            if (string.IsNullOrEmpty(Model))
            {
                throw new TestProviderException("The model ID is not defined and the default value cannot be used.");
            }

            if (string.IsNullOrEmpty(method))
            {
                throw new TestProviderException("The method is not defined and the default value cannot be used.");
            }

            var parsedRequest = new
            {
                model = string.IsNullOrEmpty(model) ? Model : model,
                method = method,
                template = template,
                userData = options.ToString()
            };

            return JsonConvert.SerializeObject(parsedRequest);
        }

        private HttpWebResponse SendRequest(String request)
        {
            X509Certificate2 certificate = null;

            try 
                {
                    certificate = new X509Certificate2(KeyStorePath, KeyStorePassword);

                    HttpWebRequest httpWebRequest = (HttpWebRequest) HttpWebRequest.Create(request);
                    httpWebRequest.ServerCertificateValidationCallback = ValidateServerCertificate;
                    httpWebRequest.ClientCertificates.Add(certificate);

                    return (HttpWebResponse) httpWebRequest.GetResponse();
                }
                catch (CryptographicException e)
                {
                    string message = $"The keystore password is incorrect. Keystore path: '{ Path.GetFullPath(KeyStorePath) }'";
                    throw new TestProviderException(message, e);
                }
                catch (WebException e)
                {
                    string message = $"The connection could not be established.";
                    throw new TestProviderException(message, e);
                }
                finally
                {
                    certificate.Dispose();
                }
        }

        private bool ValidateServerCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors) 
        {
            foreach(X509ChainElement certificate in chain.ChainElements) {
                if (certificate.Certificate.IssuerName.Name.Contains("C=NO, L=Oslo, O=EcFeed AS, OU=EcFeed, CN=ecfeed.com")) {
                    return true;
                }
            }
    
            return false;
        }

//-------------------------------------------------------------------------------------------  

        private IEnumerable<T> ProcessResponse<T>(HttpWebResponse response, Template template) 
        {
            string[] methodArgumentNames = null;
            string[] methodArgumentTypes = null;

            if (!response.StatusCode.ToString().Equals("OK"))
            {
                throw new TestProviderException(response.StatusDescription);
            }
            else
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8)) {
                    string line;
                    
                    while ((line = reader.ReadLine()) != null) {
                        T element = ProcessResponseLine<T>(line, template, ref methodArgumentTypes, ref methodArgumentNames);
                            
                        if (element != null)
                        {
                            yield return element;
                        }
                    }
                }
            }
        }

        private T ProcessResponseLine<T>(string line, Template template, ref string[] methodArgumentTypes, ref string[] methodArgumentNames)
        {
            if (template == Template.Stream)
            {
                if (line.Contains("\"testCase\""))
                {
                    return ProcessResponseDataLine<T>(line, methodArgumentTypes, methodArgumentNames);
                }
                if (line.Contains("\"status\""))
                {
                    return ProcessResponseStatusLine<T>(line);
                }
                if (line.Contains("\"info\""))
                {
                    return ProcessResponseInfoLine<T>(line, ref methodArgumentTypes, ref methodArgumentNames);
                }
                
                return default(T);
            }
            else
            {
                return GenerateTestEvent<T>(line);
            }
        }

        private T ProcessResponseDataLine<T>(string line, string[] methodArgumentTypes, string[] methodArgumentNames)
        {
            try
            {
                TestCase testCase = JsonConvert.DeserializeObject<TestCase>(line);
                return GenerateTestEvent<T>(line, methodArgumentTypes, methodArgumentNames);
            }
            catch (JsonReaderException e) 
            { 
                PrintTrace("PARSE DATA - READ", e.StackTrace);
            }
            catch (JsonSerializationException e) 
            { 
                PrintTrace("PARSE DATA - SERIALIZATION", e.StackTrace);
            }
  
            return default(T);
        }

        private T ProcessResponseStatusLine<T>(string line)
        {
            try
            {
                StatusMessage statusMessage = JsonConvert.DeserializeObject<StatusMessage>(line);
                PrintTrace("STATUS", statusMessage.Status);
            }
            catch (JsonReaderException e) 
            { 
                PrintTrace("PARSE STATUS - READ", e.StackTrace);
            }
            catch (JsonSerializationException e) 
            { 
                PrintTrace("PARSE STATUS - SERIALIZATION", e.StackTrace);
            }

            return default(T);
        }

        private T ProcessResponseInfoLine<T>(string line, ref string[] methodArgumentTypes, ref string[] methodArgumentNames)
        {
            try
            {
                InfoMessage infoMessage = JsonConvert.DeserializeObject<InfoMessage>(line);
                methodArgumentNames = InfoMessageHelper.ExtractArgumentNames(infoMessage);
                methodArgumentTypes = InfoMessageHelper.ExtractArgumentTypes(infoMessage);
                PrintTrace("INFO", string.Join(", ", methodArgumentTypes));
            }
            catch (JsonReaderException e) 
            { 
                PrintTrace("PARSE INFO - READ", e.StackTrace);
            }
            catch (JsonSerializationException e) 
            { 
                PrintTrace("PARSE INFO - SERIALIZATION", e.StackTrace);
            }

            return default(T);
        }

        private T GenerateTestEvent<T>(string data, string[] methodArgumentTypes = null, string[] methodArgumentNames = null)
        {
            if (typeof(T).ToString().Equals("System.Object[]"))
            {
                return (T)(object)StreamParser.ParseTestCaseToDataType(data, methodArgumentTypes);
            }

            if (typeof(T).ToString().Equals("System.String[][]"))
            {
                string[][] response = { methodArgumentTypes, methodArgumentNames };
                return (T)(object)response;
            }

            if (typeof(T).ToString().Equals("System.String"))
            {
                return (T)(object)data;
            }
   
            throw new TestProviderException("Unknown type.");
        }

//------------------------------------------------------------------------------------------- 

        [Conditional("VERBOSE")]
        private void PrintTrace(string header, string trace)
        {
            Console.WriteLine($"{ DateTime.Now.ToString()} - { header }\n{ trace }\n");
        }

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